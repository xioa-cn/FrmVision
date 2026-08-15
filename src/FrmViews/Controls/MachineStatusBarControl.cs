using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Forms;
using FrmServices.ViewModel;

namespace FrmViews.Controls
{
    public partial class MachineStatusBarControl : UserControl
    {
        private ObservableCollection<DeviceStatusViewModel> _devices;

        public MachineStatusBarControl()
        {
            InitializeComponent();
            devicePanel.SizeChanged += (sender, args) => AdjustDeviceBadgeWidths();
            Disposed += (sender, args) => DetachDevices();
        }

        public void Bind(MainFrmViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
            BindText(userValueLabel, viewModel, nameof(viewModel.CurrentUser));
            //BindText(productValueLabel, viewModel, nameof(viewModel.CurrentProduct));
            BindText(modeValueLabel, viewModel, nameof(viewModel.OperationMode));
            BindText(messageLabel, viewModel, nameof(viewModel.StatusMessage));

            BindDevices(viewModel.Devices);
        }

        private void BindDevices(ObservableCollection<DeviceStatusViewModel> devices)
        {
            if (devices == null) throw new ArgumentNullException(nameof(devices));
            if (ReferenceEquals(_devices, devices)) return;

            DetachDevices();
            _devices = devices;
            _devices.CollectionChanged += DevicesOnCollectionChanged;
            RenderDeviceStatuses();
        }

        private void DetachDevices()
        {
            if (_devices != null)
                _devices.CollectionChanged -= DevicesOnCollectionChanged;
            _devices = null;
        }

        private void DevicesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDisposed || Disposing) return;
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(RenderDeviceStatuses));
                return;
            }

            RenderDeviceStatuses();
        }

        private void RenderDeviceStatuses()
        {
            if (IsDisposed || Disposing) return;

            devicePanel.SuspendLayout();
            try
            {
                while (devicePanel.Controls.Count > 0)
                {
                    var oldControl = devicePanel.Controls[0];
                    devicePanel.Controls.RemoveAt(0);
                    oldControl.Dispose();
                }

                if (_devices != null)
                {
                    foreach (var device in _devices)
                    {
                        var badge = new DeviceStatusBadge();
                        devicePanel.Controls.Add(badge);
                        badge.Bind(device);
                    }
                }
            }
            finally
            {
                devicePanel.ResumeLayout(true);
            }

            AdjustDeviceBadgeWidths();
            devicePanel.PerformLayout();
            foreach (Control control in devicePanel.Controls)
            {
                control.PerformLayout();
                if (control.Controls.Count > 0)
                    control.Controls[0].PerformLayout();
            }
        }

        private void AdjustDeviceBadgeWidths()
        {
            var count = devicePanel.Controls.Count;
            if (count == 0 || devicePanel.ClientSize.Width <= 0) return;

            const int horizontalMargin = 2;
            var availableWidth = devicePanel.ClientSize.Width - horizontalMargin * count;
            var badgeWidth = Math.Max(76, Math.Min(92, availableWidth / count));
            foreach (Control control in devicePanel.Controls)
            {
                control.Size = new System.Drawing.Size(badgeWidth, 30);
                control.PerformLayout();
            }
        }

        private static void BindText(Control control, object source, string propertyName)
        {
            control.DataBindings.Clear();
            control.DataBindings.Add(nameof(Text), source, propertyName, true,
                DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
