using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using FrmCommon.ConfigUtils;

namespace FrmServices.Services.UserManagement
{
    public sealed class LoginPreferences
    {
        public LoginPreferences(string userName, string password,
            bool rememberPassword, bool autoLogin)
        {
            UserName = userName ?? string.Empty;
            Password = password ?? string.Empty;
            RememberPassword = rememberPassword;
            AutoLogin = rememberPassword && autoLogin;
        }

        public string UserName { get; }
        public string Password { get; }
        public bool RememberPassword { get; }
        public bool AutoLogin { get; }
    }

    public interface ILoginPreferenceStore
    {
        LoginPreferences Load();
        void Save(string userName, string password, bool rememberPassword,
            bool autoLogin);
    }

    public sealed class LoginPreferenceService : ILoginPreferenceStore
    {
        private static readonly byte[] AdditionalEntropy = Encoding.UTF8.GetBytes(
            "FrmVision.LoginPreferences.v1");
        private static readonly Lazy<LoginPreferenceService> DefaultInstance =
            new Lazy<LoginPreferenceService>(() => new LoginPreferenceService());
        private readonly object _syncRoot = new object();
        private readonly string _filePath;

        public LoginPreferenceService(string filePath = null)
        {
            _filePath = string.IsNullOrWhiteSpace(filePath)
                ? Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData),
                    "FrmVision", "login-preferences.json")
                : Path.GetFullPath(filePath);
        }

        public static LoginPreferenceService Default => DefaultInstance.Value;
        public string FilePath => _filePath;

        public LoginPreferences Load()
        {
            lock (_syncRoot)
            {
                try
                {
                    LoginPreferenceData data =
                        _filePath.ReadJson<LoginPreferenceData>();
                    if (data == null)
                        return new LoginPreferences(string.Empty, string.Empty,
                            false, false);

                    string password = data.RememberPassword
                        ? Decrypt(data.ProtectedPassword)
                        : string.Empty;
                    bool rememberPassword = data.RememberPassword &&
                                            password.Length > 0;
                    return new LoginPreferences(data.UserName, password,
                        rememberPassword,
                        rememberPassword && data.AutoLogin);
                }
                catch (Exception)
                {
                    return new LoginPreferences(string.Empty, string.Empty,
                        false, false);
                }
            }
        }

        public void Save(string userName, string password,
            bool rememberPassword, bool autoLogin)
        {
            string normalizedUserName = (userName ?? string.Empty).Trim();
            string normalizedPassword = password ?? string.Empty;
            bool shouldRemember = rememberPassword &&
                                  normalizedPassword.Length > 0;
            var data = new LoginPreferenceData
            {
                UserName = normalizedUserName,
                ProtectedPassword = shouldRemember
                    ? Encrypt(normalizedPassword)
                    : string.Empty,
                RememberPassword = shouldRemember,
                AutoLogin = shouldRemember && autoLogin
            };

            lock (_syncRoot)
                data.WriteJson(_filePath);
        }

        private static string Encrypt(string value)
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(value);
            byte[] protectedBytes = ProtectedData.Protect(plainBytes,
                AdditionalEntropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(protectedBytes);
        }

        private static string Decrypt(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            byte[] protectedBytes = Convert.FromBase64String(value);
            byte[] plainBytes = ProtectedData.Unprotect(protectedBytes,
                AdditionalEntropy, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(plainBytes);
        }

        [DataContract]
        private sealed class LoginPreferenceData
        {
            [DataMember(Order = 1)]
            public string UserName { get; set; }

            [DataMember(Order = 2)]
            public string ProtectedPassword { get; set; }

            [DataMember(Order = 3)]
            public bool RememberPassword { get; set; }

            [DataMember(Order = 4)]
            public bool AutoLogin { get; set; }
        }
    }
}
