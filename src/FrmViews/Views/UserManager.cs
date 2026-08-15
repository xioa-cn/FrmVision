using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using FrmCommon.FrmExtensions;
using FrmCommon.Mvvm;
using FrmServices.Services.UserManagement;
using FrmServices.ViewModel;

namespace FrmViews.Views
{
    public partial class UserManager : ViewModelFrm,
        IViewModelFrm<UserManagerViewModel>
    {
        public UserManager()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            SetViewModel(new UserManagerViewModel(UserService.Default));
        }

        public UserManager(UserManagerViewModel viewModel)
        {
            InitializeComponent();
            SetViewModel(viewModel ?? throw new ArgumentNullException(nameof(viewModel)));
        }

        public object DataContext { get; set; }
        public UserManagerViewModel ViewModel => (UserManagerViewModel)DataContext;

        public override void FrmBinding()
        {
            roleComboBox.DataSource = ViewModel.RoleOptions;
            usersBindingSource.DataSource = ViewModel.Users;

            this.SetBinding()
                .BindingProperty(displayNameTextBox, control => control.Text,
                    vm => vm.EditDisplayName)
                .BindingProperty(roleComboBox, control => control.SelectedItem,
                    vm => vm.SelectedRole)
                .BindingProperty(enabledCheckBox, control => control.Checked,
                    vm => vm.EditEnabled)
                .BindingProperty(roleComboBox, control => control.Enabled,
                    vm => vm.CanEditSecurity)
                .BindingProperty(enabledCheckBox, control => control.Enabled,
                    vm => vm.CanEditSecurity)
                .BindingProperty(newPasswordTextBox, control => control.Text,
                    vm => vm.NewPassword)
                .BindingProperty(statusLabel, control => control.Text,
                    vm => vm.StatusMessage)
                .CommandBinding(refreshButton, vm => vm.RefreshCommand)
                .CommandBinding(saveButton, vm => vm.SaveCommand)
                .CommandBinding(resetPasswordButton, vm => vm.ResetPasswordCommand)
                .CommandBinding(deleteButton, vm => vm.DeleteCommand)
                .CommandBinding(closeButton, vm => vm.CloseCommand);

            UsersBindingSourceOnCurrentChanged(usersBindingSource, EventArgs.Empty);
            base.FrmBinding();
        }

        private void SetViewModel(UserManagerViewModel viewModel)
        {
            DataContext = viewModel;
            viewModel.DeleteConfirmationRequested +=
                ViewModelOnDeleteConfirmationRequested;
            viewModel.CloseRequested += ViewModelOnCloseRequested;
            viewModel.PropertyChanged += ViewModelOnPropertyChanged;
            Disposed += (sender, args) =>
            {
                viewModel.DeleteConfirmationRequested -=
                    ViewModelOnDeleteConfirmationRequested;
                viewModel.CloseRequested -= ViewModelOnCloseRequested;
                viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            };
        }

        private void ViewModelOnPropertyChanged(object sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(UserManagerViewModel.SelectedUser) ||
                ViewModel.SelectedUser == null)
                return;

            int position = ViewModel.Users.ToList().FindIndex(item =>
                item.Id == ViewModel.SelectedUser.Id);
            if (position >= 0 && usersBindingSource.Position != position)
                usersBindingSource.Position = position;
        }

        private async void UserManagerOnShown(object sender, EventArgs e)
        {
            await ViewModel.InitializeAsync();
            usersBindingSource.ResetBindings(false);
        }

        private void UsersBindingSourceOnCurrentChanged(object sender, EventArgs e)
        {
            if (DataContext == null) return;
            ViewModel.SelectedUser = usersBindingSource.Current as UserInfo;
        }

        private async void ViewModelOnDeleteConfirmationRequested(
            object sender, EventArgs e)
        {
            UserInfo user = ViewModel.SelectedUser;
            if (user == null) return;
            if (MessageBox.Show(this,
                    "确定删除用户“" + user.DisplayName + "（" +
                    user.UserName + "）”吗？", "用户管理",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            await ViewModel.DeleteSelectedAsync();
            usersBindingSource.ResetBindings(false);
        }

        private void ViewModelOnCloseRequested(object sender, EventArgs e)
        {
            Close();
        }
    }
}
