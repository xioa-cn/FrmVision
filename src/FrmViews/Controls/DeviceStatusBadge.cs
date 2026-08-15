using System;
using System.ComponentModel;
using System.Windows.Forms;
using FrmServices.ViewModel;

namespace FrmViews.Controls
{
    public partial class DeviceStatusBadge : UserControl
    {
        private DeviceStatusViewModel _viewModel;

        public DeviceStatusBadge()
        {
            InitializeComponent();
            Disposed += (sender, args) =>
            {
                if (_viewModel != null) _viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            };
        }

        public void Bind(DeviceStatusViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            if (_viewModel != null) _viewModel.PropertyChanged -= ViewModelOnPropertyChanged;
            _viewModel = viewModel;
            nameLabel.DataBindings.Clear();
            stateLabel.DataBindings.Clear();
            nameLabel.DataBindings.Add(nameof(Label.Text), viewModel, nameof(viewModel.Name));
            stateLabel.DataBindings.Add(nameof(Label.Text), viewModel, nameof(viewModel.StatusText));
            viewModel.PropertyChanged += ViewModelOnPropertyChanged;
            UpdateStateStyle();
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DeviceStatusViewModel.IsConnected) ||
                e.PropertyName == nameof(DeviceStatusViewModel.StatusText))
                UpdateStateStyle();
        }

        private void UpdateStateStyle()
        {
            var connected = _viewModel != null && _viewModel.IsConnected;
            statusDot.BackColor = connected ? UiTheme.Success : UiTheme.Danger;
            stateLabel.ForeColor = statusDot.BackColor;
        }
    }
}
