using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Forms;
using FrmServices.ViewModel;

namespace FrmViews.Controls
{
    // 作者：xioa
    // 作者邮箱：1327916255@qq.com
    public partial class MachineStatusBarControl : UserControl
    {
        private ObservableCollection<DeviceStatusViewModel> _devices;

        public MachineStatusBarControl()
        {
            InitializeComponent();
            InitializeAuthorLabel();
            devicePanel.SizeChanged += (sender, args) => AdjustDeviceBadgeWidths();
            Disposed += (sender, args) => DetachDevices();
        }

        private void InitializeAuthorLabel()
        {
            var centerLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                ColumnCount = 3,
                RowCount = 1
            };
            centerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 234F));
            centerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            centerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            centerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var authorLabel = new Label
            {
                Name = "authorLabel",
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                AutoEllipsis = true,
                ForeColor = System.Drawing.Color.FromArgb(148, 163, 184),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Text = "\u0046\u0072\u006D\u0056\u0069\u0073\u0069\u006F\u006E"
                    + "\u0020\u00A9\u0020\u0020\u4F5C\u8005\uFF1A\u0078\u0069\u006F\u0061",
                UseMnemonic = false
            };

            rootLayout.SuspendLayout();
            rootLayout.Controls.Remove(devicePanel);
            centerLayout.Controls.Add(devicePanel, 0, 0);
            centerLayout.Controls.Add(authorLabel, 1, 0);
            rootLayout.Controls.Add(centerLayout, 1, 0);
            rootLayout.ResumeLayout(true);
            centerLayout.SizeChanged += (sender, args) => AdjustDeviceBadgeWidths();
            AdjustDeviceBadgeWidths();
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
            var centerLayout = devicePanel.Parent as TableLayoutPanel;
            if (centerLayout != null && centerLayout.ClientSize.Width > 0)
            {
                float deviceShare = Math.Min(centerLayout.ClientSize.Width,
                    Math.Max(centerLayout.ClientSize.Width * 0.3F, count * 78F));
                if (Math.Abs(centerLayout.ColumnStyles[0].Width - deviceShare) > 0.01F)
                {
                    centerLayout.SuspendLayout();
                    centerLayout.ColumnStyles[0].Width = deviceShare;
                    centerLayout.ResumeLayout(true);
                }
            }
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
