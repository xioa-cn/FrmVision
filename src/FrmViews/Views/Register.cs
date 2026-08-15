using System;
using System.ComponentModel;
using System.Windows.Forms;
using FrmCommon.FrmExtensions;
using FrmCommon.Mvvm;
using FrmServices.Services.UserManagement;
using FrmServices.ViewModel;

namespace FrmViews.Views
{
    public partial class Register : ViewModelFrm, IViewModelFrm<RegisterViewModel>
    {
        public Register()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            SetViewModel(new RegisterViewModel(UserService.Default));
        }

        public Register(RegisterViewModel viewModel)
        {
            InitializeComponent();
            SetViewModel(viewModel ?? throw new ArgumentNullException(nameof(viewModel)));
        }

        public object DataContext { get; set; }
        public RegisterViewModel ViewModel => (RegisterViewModel)DataContext;
        public UserInfo RegisteredUser => ViewModel?.RegisteredUser;

        public override void FrmBinding()
        {
            this.SetBinding()
                .BindingProperty(userNameTextBox, control => control.Text,
                    vm => vm.UserName)
                .BindingProperty(displayNameTextBox, control => control.Text,
                    vm => vm.DisplayName)
                .BindingProperty(passwordTextBox, control => control.Text,
                    vm => vm.Password)
                .BindingProperty(confirmPasswordTextBox, control => control.Text,
                    vm => vm.ConfirmPassword)
                .BindingProperty(showPasswordCheckBox, control => control.Checked,
                    vm => vm.ShowPassword)
                .BindingProperty(roleHintLabel, control => control.Text,
                    vm => vm.RoleHint)
                .BindingProperty(statusLabel, control => control.Text,
                    vm => vm.StatusMessage)
                .CommandBinding(registerButton, vm => vm.RegisterCommand)
                .CommandBinding(cancelButton, vm => vm.CancelCommand);
            base.FrmBinding();
        }

        private void SetViewModel(RegisterViewModel viewModel)
        {
            DataContext = viewModel;
            viewModel.RegisterSucceeded += ViewModelOnRegisterSucceeded;
            viewModel.CloseRequested += ViewModelOnCloseRequested;
            Disposed += (sender, args) =>
            {
                viewModel.RegisterSucceeded -= ViewModelOnRegisterSucceeded;
                viewModel.CloseRequested -= ViewModelOnCloseRequested;
            };
        }

        private async void RegisterOnShown(object sender, EventArgs e)
        {
            userNameTextBox.Select();
            await ViewModel.InitializeAsync();
        }

        private void ShowPasswordCheckBoxOnCheckedChanged(object sender, EventArgs e)
        {
            bool hidePassword = !showPasswordCheckBox.Checked;
            passwordTextBox.UseSystemPasswordChar = hidePassword;
            confirmPasswordTextBox.UseSystemPasswordChar = hidePassword;
        }

        private void ViewModelOnRegisterSucceeded(object sender, EventArgs e)
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
