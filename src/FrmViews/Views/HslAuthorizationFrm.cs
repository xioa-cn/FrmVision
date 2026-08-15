using System;
using System.ComponentModel;
using System.Drawing;
using FrmCommon.FrmExtensions;
using FrmCommon.Mvvm;
using FrmServices.ViewModel;

namespace FrmViews.Views
{
    public partial class HslAuthorizationFrm : ViewModelFrm,
        IViewModelFrm<HslAuthorizationViewModel>
    {
        public HslAuthorizationFrm()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;
            SetViewModel(new HslAuthorizationViewModel());
        }

        public HslAuthorizationFrm(HslAuthorizationViewModel viewModel)
        {
            InitializeComponent();
            SetViewModel(viewModel ??
                         throw new ArgumentNullException(nameof(viewModel)));
        }

        public object DataContext { get; set; }
        public HslAuthorizationViewModel ViewModel =>
            (HslAuthorizationViewModel)DataContext;

        public override void FrmBinding()
        {
            this.SetBinding()
                .BindingProperty(authorizationCodeTextBox,
                    control => control.Text, vm => vm.AuthorizationCode)
                .BindingProperty(showAuthorizationCodeCheckBox,
                    control => control.Checked, vm => vm.ShowAuthorizationCode)
                .BindingProperty(statusLabel, control => control.Text,
                    vm => vm.StatusMessage)
                .CommandBinding(saveButton, vm => vm.SaveCommand)
                .CommandBinding(closeButton, vm => vm.CloseCommand);
            UpdateAuthorizationStatusAppearance();
            base.FrmBinding();
        }

        private void SetViewModel(HslAuthorizationViewModel viewModel)
        {
            DataContext = viewModel;
            viewModel.AuthorizationSaved += ViewModelOnAuthorizationSaved;
            viewModel.CloseRequested += ViewModelOnCloseRequested;
            viewModel.PropertyChanged += ViewModelOnPropertyChanged;
            Disposed += (sender, args) =>
            {
                viewModel.AuthorizationSaved -= ViewModelOnAuthorizationSaved;
                viewModel.CloseRequested -= ViewModelOnCloseRequested;
                viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            };
            UpdateAuthorizationStatusAppearance();
        }

        private void ShowAuthorizationCodeOnCheckedChanged(object sender,
            EventArgs e)
        {
            authorizationCodeTextBox.UseSystemPasswordChar =
                !showAuthorizationCodeCheckBox.Checked;
        }

        private void ViewModelOnPropertyChanged(object sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HslAuthorizationViewModel.IsAuthorized) ||
                e.PropertyName ==
                nameof(HslAuthorizationViewModel.HasSavedAuthorizationCode))
                UpdateAuthorizationStatusAppearance();
        }

        private void ViewModelOnAuthorizationSaved(object sender, EventArgs e)
        {
            UpdateAuthorizationStatusAppearance();
            authorizationCodeTextBox.Select();
        }

        private void UpdateAuthorizationStatusAppearance()
        {
            if (DataContext == null) return;
            if (ViewModel.IsAuthorized)
            {
                statusAccentPanel.BackColor = Color.FromArgb(22, 163, 74);
                statusLabel.ForeColor = Color.FromArgb(21, 128, 61);
                return;
            }

            bool hasSavedCode = ViewModel.HasSavedAuthorizationCode;
            statusAccentPanel.BackColor = hasSavedCode
                ? Color.FromArgb(220, 38, 38)
                : Color.FromArgb(217, 119, 6);
            statusLabel.ForeColor = hasSavedCode
                ? Color.FromArgb(185, 28, 28)
                : Color.FromArgb(180, 83, 9);
        }

        private void ViewModelOnCloseRequested(object sender, EventArgs e)
        {
            Close();
        }
    }
}
