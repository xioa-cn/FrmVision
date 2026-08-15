using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FrmCommon.Mvvm;
using FrmServices.Services.UserManagement;

namespace FrmServices.ViewModel
{
    public sealed class RegisterViewModel : ReactiveObject
    {
        private readonly UserService _userService;
        private string _userName = string.Empty;
        private string _displayName = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _roleHint = "正在确认账户角色...";
        private string _statusMessage = string.Empty;
        private bool _showPassword;
        private bool _isBusy;

        public RegisterViewModel(UserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            RegisterCommand = new AsyncRelayCommand(RegisterAsync, CanSubmit);
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(this, EventArgs.Empty));
        }

        public event EventHandler RegisterSucceeded;
        public event EventHandler CloseRequested;

        public IAsyncRelayCommand RegisterCommand { get; }
        public IRelayCommand CancelCommand { get; }
        public UserInfo RegisteredUser { get; private set; }

        public string UserName
        {
            get => _userName;
            set
            {
                if (SetProperty(ref _userName, value ?? string.Empty))
                    RegisterCommand.NotifyCanExecuteChanged();
            }
        }

        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value ?? string.Empty);
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value ?? string.Empty))
                    RegisterCommand.NotifyCanExecuteChanged();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (SetProperty(ref _confirmPassword, value ?? string.Empty))
                    RegisterCommand.NotifyCanExecuteChanged();
            }
        }

        public string RoleHint
        {
            get => _roleHint;
            private set => SetProperty(ref _roleHint, value ?? string.Empty);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value ?? string.Empty);
        }

        public bool ShowPassword
        {
            get => _showPassword;
            set => SetProperty(ref _showPassword, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (SetProperty(ref _isBusy, value))
                    RegisterCommand.NotifyCanExecuteChanged();
            }
        }

        public async Task InitializeAsync()
        {
            try
            {
                bool hasUsers = await _userService.HasUsersAsync();
                RoleHint = hasUsers
                    ? "新账户角色：员工"
                    : "首个账户将自动成为管理员";
            }
            catch (Exception ex)
            {
                StatusMessage = ex.GetBaseException().Message;
            }
        }

        private bool CanSubmit()
        {
            return !IsBusy && !string.IsNullOrWhiteSpace(UserName) &&
                   Password.Length >= UserService.MinimumPasswordLength &&
                   ConfirmPassword.Length >= UserService.MinimumPasswordLength;
        }

        private async Task RegisterAsync()
        {
            if (!string.Equals(Password, ConfirmPassword, StringComparison.Ordinal))
            {
                StatusMessage = "两次输入的密码不一致。";
                return;
            }

            IsBusy = true;
            StatusMessage = "正在创建用户...";
            try
            {
                RegisteredUser = await _userService.RegisterAsync(
                    UserName, DisplayName, Password);
                StatusMessage = "用户创建成功";
                Password = string.Empty;
                ConfirmPassword = string.Empty;
                RegisterSucceeded?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                StatusMessage = ex.GetBaseException().Message;
                Password = string.Empty;
                ConfirmPassword = string.Empty;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
