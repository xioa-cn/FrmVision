using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using SQLite;

namespace FrmServices.Services.UserManagement
{
    public sealed class UserService
    {
        public const int MinimumPasswordLength = 5;
        private const int PasswordIterations = 100000;
        private const int PasswordSaltLength = 16;
        private const int PasswordHashLength = 32;
        private static readonly Lazy<UserService> DefaultInstance =
            new Lazy<UserService>(() => new UserService());
        private static int _sqliteInitialized;

        private readonly SQLiteAsyncConnection _database;
        private readonly SemaphoreSlim _initializeLock = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1, 1);
        private bool _initialized;

        public UserService(string databasePath = null)
        {
            EnsureSqliteInitialized();
            DatabasePath = string.IsNullOrWhiteSpace(databasePath)
                ? Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "FrmVision", "users.db3")
                : Path.GetFullPath(databasePath);

            string directory = Path.GetDirectoryName(DatabasePath);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            _database = new SQLiteAsyncConnection(new SQLiteConnectionString(
                DatabasePath, true));
        }

        public static UserService Default => DefaultInstance.Value;
        public string DatabasePath { get; }

        public async Task InitializeAsync()
        {
            if (_initialized) return;

            await _initializeLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (_initialized) return;
                await _database.CreateTableAsync<UserAccount>().ConfigureAwait(false);
                await _database.ExecuteAsync(
                        "UPDATE Users SET Role = ? WHERE Role = ?",
                        UserRoles.Employee, UserRoles.LegacyOperator)
                    .ConfigureAwait(false);
                await _database.EnableWriteAheadLoggingAsync()
                    .ConfigureAwait(false);
                _initialized = true;
            }
            finally
            {
                _initializeLock.Release();
            }
        }

        public async Task<bool> HasUsersAsync()
        {
            await InitializeAsync().ConfigureAwait(false);
            return await _database.Table<UserAccount>().CountAsync()
                .ConfigureAwait(false) > 0;
        }

        public Task CloseAsync()
        {
            return _database.CloseAsync();
        }

        public async Task<UserInfo> RegisterAsync(string userName,
            string displayName, string password)
        {
            string normalizedUserName = NormalizeUserName(userName);
            string normalizedDisplayName = NormalizeDisplayName(displayName,
                normalizedUserName);
            ValidatePassword(password);
            await InitializeAsync().ConfigureAwait(false);

            await _writeLock.WaitAsync().ConfigureAwait(false);
            try
            {
                UserAccount existing = await FindByUserNameAsync(normalizedUserName)
                    .ConfigureAwait(false);
                if (existing != null)
                    throw new UserOperationException("用户名已存在。");

                int userCount = await _database.Table<UserAccount>().CountAsync()
                    .ConfigureAwait(false);
                CreatePasswordHash(password, out string salt, out string hash);
                var account = new UserAccount
                {
                    UserName = normalizedUserName,
                    DisplayName = normalizedDisplayName,
                    PasswordSalt = salt,
                    PasswordHash = hash,
                    Role = userCount == 0
                        ? UserRoles.Administrator
                        : UserRoles.Employee,
                    IsEnabled = true,
                    CreatedAt = DateTime.Now
                };

                try
                {
                    await _database.InsertAsync(account).ConfigureAwait(false);
                }
                catch (SQLiteException ex) when (ex.Result == SQLite3.Result.Constraint)
                {
                    throw new UserOperationException("用户名已存在。");
                }

                return ToInfo(account);
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public async Task<UserInfo> AuthenticateAsync(string userName, string password)
        {
            string normalizedUserName = NormalizeUserName(userName);
            if (string.IsNullOrEmpty(password))
                throw new UserOperationException("请输入密码。");
            await InitializeAsync().ConfigureAwait(false);

            UserAccount account = await FindByUserNameAsync(normalizedUserName)
                .ConfigureAwait(false);
            if (account == null || !VerifyPassword(password, account))
                throw new UserOperationException("用户名或密码错误。");
            if (!account.IsEnabled)
                throw new UserOperationException("该用户已被停用。");

            account.LastLoginAt = DateTime.Now;
            await _database.UpdateAsync(account).ConfigureAwait(false);
            return ToInfo(account);
        }

        public async Task<IReadOnlyList<UserInfo>> GetUsersAsync()
        {
            await InitializeAsync().ConfigureAwait(false);
            List<UserAccount> accounts = await _database.Table<UserAccount>()
                .OrderBy(item => item.Id).ToListAsync().ConfigureAwait(false);
            return accounts.Select(ToInfo).ToArray();
        }

        public async Task<UserInfo> UpdateUserAsync(int id, string displayName,
            string role, bool isEnabled, int currentUserId)
        {
            ValidateRole(role);
            await InitializeAsync().ConfigureAwait(false);
            await _writeLock.WaitAsync().ConfigureAwait(false);
            try
            {
                UserAccount account = await FindByIdAsync(id).ConfigureAwait(false);
                if (account == null)
                    throw new UserOperationException("用户不存在或已被删除。");

                if (account.Id == currentUserId &&
                    (!isEnabled || !string.Equals(account.Role, role,
                        StringComparison.Ordinal)))
                    throw new UserOperationException("不能停用当前用户或修改当前用户角色。");

                if (IsRemovingAdministrator(account, role, isEnabled) &&
                    await CountEnabledAdministratorsAsync().ConfigureAwait(false) <= 1)
                    throw new UserOperationException("系统必须保留至少一个启用的管理员。");

                account.DisplayName = NormalizeDisplayName(displayName,
                    account.UserName);
                account.Role = role;
                account.IsEnabled = isEnabled;
                await _database.UpdateAsync(account).ConfigureAwait(false);
                return ToInfo(account);
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public async Task ResetPasswordAsync(int id, string password)
        {
            ValidatePassword(password);
            await InitializeAsync().ConfigureAwait(false);
            await _writeLock.WaitAsync().ConfigureAwait(false);
            try
            {
                UserAccount account = await FindByIdAsync(id).ConfigureAwait(false);
                if (account == null)
                    throw new UserOperationException("用户不存在或已被删除。");

                CreatePasswordHash(password, out string salt, out string hash);
                account.PasswordSalt = salt;
                account.PasswordHash = hash;
                await _database.UpdateAsync(account).ConfigureAwait(false);
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public async Task DeleteUserAsync(int id, int currentUserId)
        {
            await InitializeAsync().ConfigureAwait(false);
            await _writeLock.WaitAsync().ConfigureAwait(false);
            try
            {
                UserAccount account = await FindByIdAsync(id).ConfigureAwait(false);
                if (account == null) return;
                if (account.Id == currentUserId)
                    throw new UserOperationException("不能删除当前登录用户。");
                if (string.Equals(account.Role, UserRoles.Administrator,
                        StringComparison.Ordinal) && account.IsEnabled &&
                    await CountEnabledAdministratorsAsync().ConfigureAwait(false) <= 1)
                    throw new UserOperationException("系统必须保留至少一个启用的管理员。");

                await _database.DeleteAsync(account).ConfigureAwait(false);
            }
            finally
            {
                _writeLock.Release();
            }
        }

        private Task<UserAccount> FindByIdAsync(int id)
        {
            return _database.Table<UserAccount>()
                .Where(item => item.Id == id).FirstOrDefaultAsync();
        }

        private Task<UserAccount> FindByUserNameAsync(string userName)
        {
            return _database.Table<UserAccount>()
                .Where(item => item.UserName == userName).FirstOrDefaultAsync();
        }

        private Task<int> CountEnabledAdministratorsAsync()
        {
            return _database.Table<UserAccount>().Where(item =>
                item.Role == UserRoles.Administrator && item.IsEnabled).CountAsync();
        }

        private static bool IsRemovingAdministrator(UserAccount account,
            string newRole, bool isEnabled)
        {
            return account.IsEnabled &&
                   string.Equals(account.Role, UserRoles.Administrator,
                       StringComparison.Ordinal) &&
                   (!isEnabled || !string.Equals(newRole,
                       UserRoles.Administrator, StringComparison.Ordinal));
        }

        private static string NormalizeUserName(string userName)
        {
            string value = (userName ?? string.Empty).Trim();
            if (value.Length < 3 || value.Length > 32)
                throw new UserOperationException("用户名长度必须为 3 到 32 个字符。");
            if (value.Any(character => !(char.IsLetterOrDigit(character) ||
                                         character == '_' || character == '-' ||
                                         character == '.')))
                throw new UserOperationException(
                    "用户名只能包含字母、数字、下划线、短横线和点。");
            return value;
        }

        private static string NormalizeDisplayName(string displayName,
            string fallback)
        {
            string value = (displayName ?? string.Empty).Trim();
            if (value.Length == 0) value = fallback;
            if (value.Length > 50)
                throw new UserOperationException("显示名称不能超过 50 个字符。");
            return value;
        }

        private static void ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password) ||
                password.Length < MinimumPasswordLength)
                throw new UserOperationException("密码至少需要 5 个字符。");
            if (password.Length > 128)
                throw new UserOperationException("密码不能超过 128 个字符。");
        }

        private static void ValidateRole(string role)
        {
            if (!UserRoles.All.Contains(role))
                throw new UserOperationException("用户角色无效。");
        }

        private static void CreatePasswordHash(string password,
            out string saltValue, out string hashValue)
        {
            var salt = new byte[PasswordSaltLength];
            using (var random = RandomNumberGenerator.Create())
                random.GetBytes(salt);

            using (var derive = new Rfc2898DeriveBytes(password, salt,
                       PasswordIterations, HashAlgorithmName.SHA256))
            {
                saltValue = Convert.ToBase64String(salt);
                hashValue = Convert.ToBase64String(
                    derive.GetBytes(PasswordHashLength));
            }
        }

        private static bool VerifyPassword(string password, UserAccount account)
        {
            try
            {
                byte[] salt = Convert.FromBase64String(account.PasswordSalt);
                byte[] expected = Convert.FromBase64String(account.PasswordHash);
                byte[] actual;
                using (var derive = new Rfc2898DeriveBytes(password, salt,
                           PasswordIterations, HashAlgorithmName.SHA256))
                    actual = derive.GetBytes(expected.Length);
                return FixedTimeEquals(actual, expected);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static bool FixedTimeEquals(byte[] left, byte[] right)
        {
            if (left == null || right == null || left.Length != right.Length)
                return false;
            int difference = 0;
            for (int index = 0; index < left.Length; index++)
                difference |= left[index] ^ right[index];
            return difference == 0;
        }

        private static UserInfo ToInfo(UserAccount account)
        {
            return new UserInfo
            {
                Id = account.Id,
                UserName = account.UserName,
                DisplayName = account.DisplayName,
                Role = UserRoles.Normalize(account.Role),
                IsEnabled = account.IsEnabled,
                CreatedAt = account.CreatedAt,
                LastLoginAt = account.LastLoginAt
            };
        }

        private static void EnsureSqliteInitialized()
        {
            if (Interlocked.Exchange(ref _sqliteInitialized, 1) == 0)
                SQLitePCL.Batteries_V2.Init();
        }
    }
}
