using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using FrmCommon.Mvvm;
using FrmServices.Services.UserManagement;

namespace FrmServices.ViewModel
{
    public sealed class LoginViewModel : ReactiveObject
    {
        private readonly UserService _userService;
        private readonly ILoginPreferenceStore _preferenceStore;
        private string _userName = string.Empty;
        private string _password = string.Empty;
        private bool _showPassword;
        private bool _rememberPassword;
        private bool _autoLogin;
        private bool _isBusy;
        private string _statusMessage = string.Empty;

        public LoginViewModel(UserService userService)
            : this(userService, LoginPreferenceService.Default)
        {
        }

        public LoginViewModel(UserService userService,
            ILoginPreferenceStore preferenceStore)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _preferenceStore = preferenceStore ??
                               throw new ArgumentNullException(nameof(preferenceStore));
            LoginPreferences preferences = _preferenceStore.Load();
            _userName = preferences.UserName;
            _password = preferences.Password;
            _rememberPassword = preferences.RememberPassword;
            _autoLogin = preferences.AutoLogin;
            LoginCommand = new AsyncRelayCommand(LoginAsync, CanSubmit);
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(this, EventArgs.Empty));
        }

        public event EventHandler LoginSucceeded;
        public event EventHandler CloseRequested;

        public IAsyncRelayCommand LoginCommand { get; }
        public IRelayCommand CancelCommand { get; }
        public UserInfo AuthenticatedUser { get; private set; }

        public string UserName
        {
            get => _userName;
            set
            {
                if (SetProperty(ref _userName, value ?? string.Empty))
                    LoginCommand.NotifyCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value ?? string.Empty))
                    LoginCommand.NotifyCanExecuteChanged();
            }
        }

        public bool ShowPassword
        {
            get => _showPassword;
            set => SetProperty(ref _showPassword, value);
        }

        public bool RememberPassword
        {
            get => _rememberPassword;
            set
            {
                if (!SetProperty(ref _rememberPassword, value)) return;
                if (value) return;

                SetProperty(ref _autoLogin, false, nameof(AutoLogin));
                TrySavePreferences(string.Empty, false, false);
            }
        }

        public bool AutoLogin
        {
            get => _autoLogin;
            set
            {
                if (value && !RememberPassword)
                    RememberPassword = true;
                if (!SetProperty(ref _autoLogin, value)) return;
                if (!value && RememberPassword)
                    TrySavePreferences(Password, true, false);
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (SetProperty(ref _isBusy, value))
                    LoginCommand.NotifyCanExecuteChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value ?? string.Empty);
        }

        private bool CanSubmit()
        {
            return !IsBusy && !string.IsNullOrWhiteSpace(UserName) &&
                   !string.IsNullOrEmpty(Password);
        }

        private async Task LoginAsync()
        {
            await AuthenticateAsync(true);
        }

        public Task<bool> TryAutoLoginAsync()
        {
            if (!AutoLogin || !RememberPassword ||
                string.IsNullOrWhiteSpace(UserName) ||
                string.IsNullOrEmpty(Password))
                return Task.FromResult(false);

            return AuthenticateAsync(false);
        }

        private async Task<bool> AuthenticateAsync(bool notifyLoginSucceeded)
        {
            IsBusy = true;
            StatusMessage = notifyLoginSucceeded
                ? "正在验证用户..."
                : "正在自动登录...";
            try
            {
                AuthenticatedUser = await _userService.AuthenticateAsync(
                    UserName, Password);
                UserSession.SignIn(AuthenticatedUser);
                bool settingsSaved = TrySavePreferences(Password,
                    RememberPassword, AutoLogin);
                StatusMessage = settingsSaved
                    ? (notifyLoginSucceeded ? "登录成功" : "自动登录成功")
                    : "登录成功，但登录设置保存失败";
                if (!RememberPassword) Password = string.Empty;
                if (notifyLoginSucceeded)
                    LoginSucceeded?.Invoke(this, EventArgs.Empty);
                return true;
            }
            catch (Exception ex)
            {
                StatusMessage = ex.GetBaseException().Message;
                Password = string.Empty;
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool TrySavePreferences(string password,
            bool rememberPassword, bool autoLogin)
        {
            try
            {
                _preferenceStore.Save(UserName, password,
                    rememberPassword, autoLogin);
                return true;
            }
            catch (Exception ex)
            {
                StatusMessage = "登录设置保存失败：" +
                                ex.GetBaseException().Message;
                return false;
            }
        }
    }
}
