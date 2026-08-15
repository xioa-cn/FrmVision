using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FrmCommon.Mvvm;
using FrmServices.Services.UserManagement;

namespace FrmServices.ViewModel
{
    public sealed class UserManagerViewModel : ReactiveObject
    {
        private readonly UserService _userService;
        private UserInfo _selectedUser;
        private string _editDisplayName = string.Empty;
        private string _selectedRole = UserRoles.Employee;
        private bool _editEnabled = true;
        private string _newPassword = string.Empty;
        private string _statusMessage = string.Empty;
        private bool _isBusy;

        public UserManagerViewModel(UserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            Users = new BindingList<UserInfo>();
            RefreshCommand = new AsyncRelayCommand(RefreshAsync, () => !IsBusy);
            SaveCommand = new AsyncRelayCommand(SaveAsync, CanEdit);
            ResetPasswordCommand = new AsyncRelayCommand(ResetPasswordAsync,
                CanResetPassword);
            DeleteCommand = new RelayCommand(
                () => DeleteConfirmationRequested?.Invoke(this, EventArgs.Empty),
                CanDelete);
            CloseCommand = new RelayCommand(() => CloseRequested?.Invoke(this, EventArgs.Empty));
        }

        public event EventHandler DeleteConfirmationRequested;
        public event EventHandler CloseRequested;

        public BindingList<UserInfo> Users { get; }
        public string[] RoleOptions => UserRoles.All;
        public IAsyncRelayCommand RefreshCommand { get; }
        public IAsyncRelayCommand SaveCommand { get; }
        public IAsyncRelayCommand ResetPasswordCommand { get; }
        public IRelayCommand DeleteCommand { get; }
        public IRelayCommand CloseCommand { get; }

        public UserInfo SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (!SetProperty(ref _selectedUser, value)) return;
                EditDisplayName = value?.DisplayName ?? string.Empty;
                SelectedRole = value?.Role ?? UserRoles.Employee;
                EditEnabled = value?.IsEnabled ?? true;
                NewPassword = string.Empty;
                OnPropertyChanged(nameof(CanEditSecurity));
                NotifyCommands();
            }
        }

        public string EditDisplayName
        {
            get => _editDisplayName;
            set => SetProperty(ref _editDisplayName, value ?? string.Empty);
        }

        public string SelectedRole
        {
            get => _selectedRole;
            set => SetProperty(ref _selectedRole, value ?? UserRoles.Employee);
        }

        public bool EditEnabled
        {
            get => _editEnabled;
            set => SetProperty(ref _editEnabled, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set
            {
                if (SetProperty(ref _newPassword, value ?? string.Empty))
                    ResetPasswordCommand.NotifyCanExecuteChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value ?? string.Empty);
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (!SetProperty(ref _isBusy, value)) return;
                NotifyCommands();
                RefreshCommand.NotifyCanExecuteChanged();
            }
        }

        public bool CanEditSecurity
        {
            get
            {
                UserInfo current = UserSession.CurrentUser;
                return SelectedUser != null && current != null &&
                       SelectedUser.Id != current.Id;
            }
        }

        public Task InitializeAsync()
        {
            return RefreshAsync();
        }

        public async Task DeleteSelectedAsync()
        {
            if (!CanDelete()) return;
            UserInfo current = UserSession.CurrentUser;
            int selectedId = SelectedUser.Id;
            await RunBusyAsync(async () =>
            {
                await _userService.DeleteUserAsync(selectedId, current.Id);
                StatusMessage = "用户已删除";
                await LoadUsersAsync(null);
            });
        }

        private async Task RefreshAsync()
        {
            await RunBusyAsync(async () =>
            {
                int? selectedId = SelectedUser?.Id;
                await LoadUsersAsync(selectedId);
                StatusMessage = Users.Count + " 个用户";
            });
        }

        private async Task SaveAsync()
        {
            if (!CanEdit()) return;
            int selectedId = SelectedUser.Id;
            UserInfo current = UserSession.CurrentUser;
            await RunBusyAsync(async () =>
            {
                UserInfo updated = await _userService.UpdateUserAsync(
                    selectedId, EditDisplayName, SelectedRole, EditEnabled,
                    current.Id);
                UserSession.Refresh(updated);
                StatusMessage = "用户信息已保存";
                await LoadUsersAsync(selectedId);
            });
        }

        private async Task ResetPasswordAsync()
        {
            if (!CanResetPassword()) return;
            int selectedId = SelectedUser.Id;
            string password = NewPassword;
            await RunBusyAsync(async () =>
            {
                await _userService.ResetPasswordAsync(selectedId, password);
                NewPassword = string.Empty;
                StatusMessage = "密码已重置";
            });
        }

        private async Task LoadUsersAsync(int? selectedId)
        {
            var users = await _userService.GetUsersAsync();
            Users.Clear();
            foreach (UserInfo user in users) Users.Add(user);
            SelectedUser = selectedId.HasValue
                ? Users.FirstOrDefault(item => item.Id == selectedId.Value)
                : Users.FirstOrDefault();
        }

        private async Task RunBusyAsync(Func<Task> action)
        {
            IsBusy = true;
            try
            {
                if (!UserSession.IsAdministrator)
                    throw new UserOperationException("只有管理员可以管理用户。");
                await action();
            }
            catch (Exception ex)
            {
                StatusMessage = ex.GetBaseException().Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanEdit()
        {
            return !IsBusy && SelectedUser != null && UserSession.IsAdministrator;
        }

        private bool CanResetPassword()
        {
            return CanEdit() &&
                   NewPassword.Length >= UserService.MinimumPasswordLength;
        }

        private bool CanDelete()
        {
            return CanEdit() && CanEditSecurity;
        }

        private void NotifyCommands()
        {
            SaveCommand.NotifyCanExecuteChanged();
            ResetPasswordCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
        }
    }
}
