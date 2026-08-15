using System;
using System.ComponentModel;
using System.Windows.Forms;
using FrmCommon.FrmExtensions;
using FrmCommon.Mvvm;
using FrmServices.Services.UserManagement;
using FrmServices.ViewModel;

namespace FrmViews.Views
{
    public partial class Login : ViewModelFrm, IViewModelFrm<LoginViewModel>
    {
        public Login()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            SetViewModel(new LoginViewModel(UserService.Default));
        }

        public Login(LoginViewModel viewModel)
        {
            InitializeComponent();
            SetViewModel(viewModel ?? throw new ArgumentNullException(nameof(viewModel)));
        }

        public object DataContext { get; set; }
        public LoginViewModel ViewModel => (LoginViewModel)DataContext;
        public UserInfo AuthenticatedUser => ViewModel?.AuthenticatedUser;

        public override void FrmBinding()
        {
            this.SetBinding()
                .BindingProperty(userNameTextBox, control => control.Text,
                    vm => vm.UserName)
                .BindingProperty(passwordTextBox, control => control.Text,
                    vm => vm.Password)
                .BindingProperty(showPasswordCheckBox, control => control.Checked,
                    vm => vm.ShowPassword)
                .BindingProperty(rememberPasswordCheckBox,
                    control => control.Checked, vm => vm.RememberPassword)
                .BindingProperty(autoLoginCheckBox, control => control.Checked,
                    vm => vm.AutoLogin)
                .BindingProperty(statusLabel, control => control.Text,
                    vm => vm.StatusMessage)
                .CommandBinding(loginButton, vm => vm.LoginCommand)
                .CommandBinding(cancelButton, vm => vm.CancelCommand);
            base.FrmBinding();
        }

        private void SetViewModel(LoginViewModel viewModel)
        {
            DataContext = viewModel;
            viewModel.LoginSucceeded += ViewModelOnLoginSucceeded;
            viewModel.CloseRequested += ViewModelOnCloseRequested;
            Disposed += (sender, args) =>
            {
                viewModel.LoginSucceeded -= ViewModelOnLoginSucceeded;
                viewModel.CloseRequested -= ViewModelOnCloseRequested;
            };
        }

        private void LoginOnShown(object sender, EventArgs e)
        {
            userNameTextBox.Select();
        }

        private void ShowPasswordCheckBoxOnCheckedChanged(object sender, EventArgs e)
        {
            passwordTextBox.UseSystemPasswordChar = !showPasswordCheckBox.Checked;
        }

        private void ViewModelOnLoginSucceeded(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ViewModelOnCloseRequested(object sender, EventArgs e)
        {
            Close();
        }
    }
}
