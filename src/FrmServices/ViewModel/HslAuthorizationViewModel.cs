using System;
using CommunityToolkit.Mvvm.Input;
using FrmCommon.ConfigUtils;
using FrmCommon.Mvvm;

namespace FrmServices.ViewModel
{
    public sealed class HslAuthorizationViewModel : ReactiveObject
    {
        private string _authorizationCode = string.Empty;
        private bool _showAuthorizationCode;
        private string _statusMessage;

        public HslAuthorizationViewModel()
        {
            _statusMessage = BuildStatusMessage();
            SaveCommand = new RelayCommand(Save, CanSave);
            CloseCommand = new RelayCommand(() =>
                CloseRequested?.Invoke(this, EventArgs.Empty));
        }

        public event EventHandler AuthorizationSaved;
        public event EventHandler CloseRequested;

        public IRelayCommand SaveCommand { get; }
        public IRelayCommand CloseCommand { get; }
        public bool IsAuthorized => HslAuthExtensios.IsAuthorized;
        public bool HasSavedAuthorizationCode =>
            HslAuthExtensios.HasSavedAuthorizationCode;

        public string AuthorizationCode
        {
            get => _authorizationCode;
            set
            {
                if (SetProperty(ref _authorizationCode, value ?? string.Empty))
                    SaveCommand.NotifyCanExecuteChanged();
            }
        }

        public bool ShowAuthorizationCode
        {
            get => _showAuthorizationCode;
            set => SetProperty(ref _showAuthorizationCode, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value ?? string.Empty);
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(AuthorizationCode);
        }

        private void Save()
        {
            try
            {
                if (!HslAuthExtensios.SaveAuthorizationCode(AuthorizationCode))
                {
                    StatusMessage = BuildStatusMessage();
                    OnPropertyChanged(nameof(IsAuthorized));
                    OnPropertyChanged(nameof(HasSavedAuthorizationCode));
                    return;
                }

                AuthorizationCode = string.Empty;
                StatusMessage = BuildStatusMessage();
                OnPropertyChanged(nameof(IsAuthorized));
                OnPropertyChanged(nameof(HasSavedAuthorizationCode));
                AuthorizationSaved?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                StatusMessage = "当前状态：保存通讯秘钥失败，" +
                                ex.GetBaseException().Message;
                OnPropertyChanged(nameof(IsAuthorized));
                OnPropertyChanged(nameof(HasSavedAuthorizationCode));
            }
        }

        private static string BuildStatusMessage()
        {
            if (HslAuthExtensios.IsAuthorized)
                return "当前状态：秘钥已成功解密，HSLCommunication 已授权。";
            if (!string.IsNullOrWhiteSpace(HslAuthExtensios.LastError) &&
                HslAuthExtensios.LastError != "尚未配置通讯秘钥。")
                return "当前状态：解密或授权失败，" +
                       HslAuthExtensios.LastError;
            if (!HslAuthExtensios.HasSavedAuthorizationCode)
                return "当前状态：尚未配置通讯秘钥。";
            return "当前状态：解密或授权失败，请重新输入有效秘钥。";
        }
    }
}
