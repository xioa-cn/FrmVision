using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using FrmCommon.FrmExtensions;
using FrmCommon.Mvvm;
using FrmServices.ViewModel;

namespace FrmViews.Views
{
    public partial class CommunicationFrm : ViewModelFrm, IViewModelFrm<CommunicationFrmViewModel>
    {
        private const string NetworkConnectionMode = "网口";
        private const string SerialConnectionMode = "串口";
        private bool _updatingEditor;
        private readonly TableLayoutPanel _plcProtocolParametersLayout =
            new TableLayoutPanel();
        private readonly Dictionary<string, Control> _plcProtocolParameterEditors =
            new Dictionary<string, Control>(StringComparer.OrdinalIgnoreCase);
        private readonly ComboBox _plcDataBitsComboBox = new ComboBox();
        private readonly ComboBox _plcParityComboBox = new ComboBox();
        private readonly ComboBox _plcStopBitsComboBox = new ComboBox();
        private string _displayedPlcProtocol = string.Empty;

        public CommunicationFrm()
        {
            InitializeComponent();
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;

            DataContext = new CommunicationFrmViewModel();
            ConfigureResponsiveLayout();
            InitializeView();
        }

        public CommunicationFrm(CommunicationFrmViewModel communicationFrmViewModel)
        {
            DataContext = communicationFrmViewModel ?? throw new ArgumentNullException(
                nameof(communicationFrmViewModel));
            InitializeComponent();
            ConfigureResponsiveLayout();
            InitializeView();
        }

        public object DataContext { get; set; }
        private CommunicationFrmViewModel ViewModel =>
            (CommunicationFrmViewModel)DataContext;

        private DeviceConnectionConfiguration SelectedConfiguration =>
            deviceBindingSource.Current as DeviceConnectionConfiguration;

        private void InitializeView()
        {
            plcProtocolComboBox.DataSource = ViewModel.PlcProtocolNames.ToArray();
            PopulateSerialPorts(plcSerialPortComboBox);
            PopulateSerialPorts(lightSerialPortComboBox);
            deviceBindingSource.DataSource = ViewModel.DeviceConfigurations;
            deviceBindingSource.CurrentChanged += DeviceBindingSourceOnCurrentChanged;
            topologyControl.Bind(ViewModel.DeviceConfigurations);
            UpdateEditor();
        }

        private void ConfigureResponsiveLayout()
        {
            workspaceSplitContainer.Panel2MinSize = 180;
            workspaceSplitContainer.FixedPanel = FixedPanel.None;
            workspaceSplitContainer.Resize += WorkspaceSplitContainerOnResize;
            SizeChanged += CommunicationFrmOnSizeChanged;
            UpdateWorkspaceSplitRatio();
            ConfigureHeaderLayout();

            closeButton.Visible = false;
            detailsLayout.RowCount = 2;
            detailsLayout.RowStyles.Clear();
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            detailsContentPanel.AutoScroll = true;
            detailsContentPanel.RowStyles[1] =
                new RowStyle(SizeType.Absolute, 230F);
            detailsContentPanel.Resize += DetailsContentPanelOnResize;

            detailsTypeLabel.Dock = DockStyle.Right;
            detailsTypeLabel.TextAlign = ContentAlignment.MiddleCenter;

            ConfigureEditorTable(cameraFieldsLayout, null);
            ConfigureEditorTable(plcFieldsLayout, plcEndpointLayout);
            ConfigureEditorTable(lightFieldsLayout, lightEndpointLayout);
            ConfigureScrollableEditor(plcFieldsLayout, 2, 190F);
            ConfigureScrollableEditor(lightFieldsLayout, 1, 260F);
            plcTabPage.AutoScrollMinSize = Size.Empty;
            lightTabPage.AutoScrollMinSize = Size.Empty;

            ConfigureEndpointContainer(plcEndpointLayout,
                plcNetworkPanel, plcSerialPanel);
            ConfigureEndpointContainer(lightEndpointLayout,
                lightNetworkPanel, lightSerialPanel);
            ConfigureEndpointPanel(plcNetworkPanel, plcNetworkTitleLabel);
            ConfigureEndpointPanel(plcSerialPanel, plcSerialTitleLabel);
            ConfigureEndpointPanel(lightNetworkPanel, lightNetworkTitleLabel);
            ConfigureEndpointPanel(lightSerialPanel, lightSerialTitleLabel);
            InitializePlcSerialParameters();
            InitializePlcProtocolParameters();

            foreach (var label in new[]
                     {
                         cameraProtocolLabel, cameraHostLabel, cameraTimeoutLabel,
                         plcProtocolLabel, plcConnectionModeLabel, plcHostLabel,
                         plcPortLabel, plcSerialPortLabel, plcBaudLabel, plcStationLabel,
                         lightConnectionModeLabel, lightHostLabel, lightPortLabel,
                         lightSerialPortLabel, lightBaudLabel, lightDataBitsLabel,
                         lightParityLabel, lightStopBitsLabel
                     })
                StyleFieldLabel(label);

            foreach (var textBox in new[]
                     {
                         cameraHostTextBox, plcHostTextBox, lightHostTextBox
                     })
                StyleTextBox(textBox);

            foreach (var comboBox in new[]
                     {
                         plcProtocolComboBox, plcConnectionModeComboBox,
                         plcSerialPortComboBox, plcBaudComboBox,
                         lightConnectionModeComboBox, lightSerialPortComboBox,
                         lightBaudComboBox, lightDataBitsComboBox,
                         lightParityComboBox, lightStopBitsComboBox
                     })
                StyleComboBox(comboBox);

            StyleNumeric(cameraTimeoutNumeric, 100, 60000, 500);
            StyleNumeric(plcPortNumeric, 1, 65535, 102);
            StyleNumeric(plcStationNumeric, 0, 255, 1);
            StyleNumeric(lightPortNumeric, 1, 65535, 5000);
            enabledCheckBox.CheckedChanged += EnabledCheckBoxOnCheckedChanged;
            UpdateEnabledCheckBoxAppearance();
        }

        private void InitializePlcSerialParameters()
        {
            AddSerialComboField(plcSerialPanel, 4, "数据位",
                _plcDataBitsComboBox, new[] { "5", "6", "7", "8" });
            AddSerialComboField(plcSerialPanel, 5, "校验位",
                _plcParityComboBox, Enum.GetNames(typeof(Parity)));
            AddSerialComboField(plcSerialPanel, 6, "停止位",
                _plcStopBitsComboBox, Enum.GetNames(typeof(StopBits)));
        }

        private void InitializePlcProtocolParameters()
        {
            plcFieldsLayout.RowCount = 4;
            while (plcFieldsLayout.RowStyles.Count < 4)
                plcFieldsLayout.RowStyles.Add(new RowStyle());
            plcFieldsLayout.RowStyles[3] = new RowStyle(SizeType.AutoSize);

            _plcProtocolParametersLayout.Name = "plcProtocolParametersLayout";
            _plcProtocolParametersLayout.AutoSize = true;
            _plcProtocolParametersLayout.AutoSizeMode =
                AutoSizeMode.GrowAndShrink;
            _plcProtocolParametersLayout.BackColor =
                Color.FromArgb(248, 250, 252);
            _plcProtocolParametersLayout.ColumnCount = 3;
            _plcProtocolParametersLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 130F));
            _plcProtocolParametersLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 460F));
            _plcProtocolParametersLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));
            _plcProtocolParametersLayout.Dock = DockStyle.Top;
            _plcProtocolParametersLayout.Margin = new Padding(0, 10, 0, 0);
            _plcProtocolParametersLayout.Padding = new Padding(12, 6, 12, 10);
            plcFieldsLayout.Controls.Add(_plcProtocolParametersLayout, 0, 3);
            plcFieldsLayout.SetColumnSpan(_plcProtocolParametersLayout, 3);
        }

        private static void AddSerialComboField(TableLayoutPanel panel, int row,
            string text, ComboBox comboBox, string[] options)
        {
            while (panel.RowStyles.Count <= row)
                panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            panel.RowCount = Math.Max(panel.RowCount, row + 1);

            var label = new Label { Text = text };
            StyleFieldLabel(label);
            StyleComboBox(comboBox);
            comboBox.Items.AddRange(options.Cast<object>().ToArray());
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = comboBox.Items.Count - 1;
            panel.Controls.Add(label, 0, row);
            panel.Controls.Add(comboBox, 1, row);
        }

        private void WorkspaceSplitContainerOnResize(object sender, EventArgs e)
        {
            UpdateWorkspaceSplitRatio();
            ResetSelectedEditorScrollPosition();
        }

        private void CommunicationFrmOnSizeChanged(object sender, EventArgs e)
        {
            ConfigureHeaderLayout();
            UpdateWorkspaceSplitRatio();
            ResetSelectedEditorScrollPosition();
        }

        private void ConfigureHeaderLayout()
        {
            var compact = ClientSize.Width < 1050;
            rootLayout.RowStyles[0].Height = compact ? 140F : 104F;
            addButtonPanel.Width = compact
                ? Math.Max(800, headerPanel.ClientSize.Width)
                : 810;
            addButtonPanel.Height = 52;
            addButtonPanel.Dock = compact ? DockStyle.Bottom : DockStyle.Right;
            addButtonPanel.WrapContents = false;
            addButtonPanel.Padding = compact
                ? new Padding(0, 8, 0, 0)
                : new Padding(0, 20, 0, 0);

            saveButton.Dock = DockStyle.None;
            saveButton.BackColor = Color.White;
            saveButton.FlatAppearance.BorderColor = Color.FromArgb(220, 225, 232);
            saveButton.FlatAppearance.BorderSize = 1;
            saveButton.ForeColor = Color.FromArgb(24, 32, 45);
            saveButton.UseVisualStyleBackColor = false;
            saveButton.Size = addLightButton.Size;
            saveButton.Margin = new Padding(8, 0, 0, 0);
            saveButton.Visible = true;

            statusLabel.AutoEllipsis = true;
            statusLabel.AutoSize = false;
            statusLabel.Dock = DockStyle.None;
            statusLabel.Size = new Size(110, 38);
            statusLabel.Margin = new Padding(12, 0, 0, 0);
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        }

        private void DetailsContentPanelOnResize(object sender, EventArgs e)
        {
            if (detailsContentPanel.ClientSize.Height <= 0) return;

            var desiredHeight = Math.Max(230,
                detailsContentPanel.ClientSize.Height - 70);
            var rowStyle = detailsContentPanel.RowStyles[1];
            if (Math.Abs(rowStyle.Height - desiredHeight) > 0.5F)
            {
                rowStyle.SizeType = SizeType.Absolute;
                rowStyle.Height = desiredHeight;
            }
        }

        private void UpdateWorkspaceSplitRatio()
        {
            var maximum = workspaceSplitContainer.Height -
                          workspaceSplitContainer.Panel2MinSize -
                          workspaceSplitContainer.SplitterWidth;
            if (maximum < workspaceSplitContainer.Panel1MinSize) return;

            var requestedDistance = (int)Math.Round(
                workspaceSplitContainer.Height * 0.6D);
            var distance = Math.Max(workspaceSplitContainer.Panel1MinSize,
                Math.Min(maximum, requestedDistance));
            if (workspaceSplitContainer.SplitterDistance != distance)
                workspaceSplitContainer.SplitterDistance = distance;
        }

        private static void ConfigureEditorTable(TableLayoutPanel table,
            Control fullWidthContent)
        {
            table.ColumnCount = 3;
            table.ColumnStyles.Clear();
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 460F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            if (fullWidthContent != null)
                table.SetColumnSpan(fullWidthContent, 3);
        }

        private static void ConfigureScrollableEditor(TableLayoutPanel table,
            int contentRowIndex, float contentHeight)
        {
            table.AutoSize = true;
            table.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            table.Dock = DockStyle.Top;
            table.RowStyles[contentRowIndex] =
                new RowStyle(SizeType.Absolute, contentHeight);
        }

        private static void ConfigureEndpointContainer(TableLayoutPanel container,
            Control networkPanel, Control serialPanel)
        {
            container.ColumnCount = 2;
            container.ColumnStyles.Clear();
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            container.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            container.SetCellPosition(networkPanel,
                new TableLayoutPanelCellPosition(0, 0));
            container.SetCellPosition(serialPanel,
                new TableLayoutPanelCellPosition(0, 0));
            container.SetColumnSpan(networkPanel, 2);
            container.SetColumnSpan(serialPanel, 2);
        }

        private static void ConfigureEndpointPanel(TableLayoutPanel panel,
            Label titleLabel)
        {
            panel.BackColor = Color.FromArgb(248, 250, 252);
            panel.ColumnCount = 3;
            panel.ColumnStyles.Clear();
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panel.Dock = DockStyle.Fill;
            panel.Padding = new Padding(12, 6, 12, 8);
            panel.RowCount = 6;
            panel.RowStyles.Clear();
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            panel.SetColumnSpan(titleLabel, 3);
            titleLabel.Dock = DockStyle.Fill;
            titleLabel.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(24, 32, 45);
            titleLabel.TextAlign = ContentAlignment.MiddleLeft;
        }

        private static void StyleFieldLabel(Label label)
        {
            label.Dock = DockStyle.Fill;
            label.Font = new Font("Microsoft YaHei UI", 9F);
            label.ForeColor = Color.FromArgb(100, 112, 128);
            label.TextAlign = ContentAlignment.MiddleLeft;
        }

        private static void StyleTextBox(TextBox textBox)
        {
            textBox.BackColor = Color.FromArgb(249, 250, 252);
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Dock = DockStyle.Fill;
            textBox.Font = new Font("Microsoft YaHei UI", 9.5F);
            textBox.Margin = new Padding(0, 8, 0, 8);
        }

        private static void StyleComboBox(ComboBox comboBox)
        {
            comboBox.BackColor = Color.FromArgb(249, 250, 252);
            comboBox.Dock = DockStyle.Fill;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.Font = new Font("Microsoft YaHei UI", 9F);
            comboBox.Margin = new Padding(0, 7, 0, 7);
        }

        private static void StyleNumeric(NumericUpDown numeric,
            decimal minimum, decimal maximum, decimal value)
        {
            numeric.BackColor = Color.FromArgb(249, 250, 252);
            numeric.BorderStyle = BorderStyle.FixedSingle;
            numeric.Dock = DockStyle.Fill;
            numeric.Font = new Font("Microsoft YaHei UI", 9F);
            numeric.Margin = new Padding(0, 8, 0, 8);
            numeric.Maximum = maximum;
            numeric.Minimum = minimum;
            numeric.Value = value;
        }

        private void EnabledCheckBoxOnCheckedChanged(object sender, EventArgs e)
        {
            UpdateEnabledCheckBoxAppearance();
        }

        private void UpdateEnabledCheckBoxAppearance()
        {
            enabledCheckBox.Text = enabledCheckBox.Checked ? "已启用" : "已停用";
            enabledCheckBox.ForeColor = enabledCheckBox.Checked
                ? Color.FromArgb(21, 146, 78)
                : Color.FromArgb(100, 112, 128);
        }

        private void ResetSelectedEditorScrollPosition()
        {
            if (detailsContentPanel.AutoScroll)
                detailsContentPanel.AutoScrollPosition = Point.Empty;
            var selectedPage = settingsTabControl.SelectedTab;
            if (selectedPage != null && selectedPage.AutoScroll)
                selectedPage.AutoScrollPosition = Point.Empty;
        }

        private static void PopulateSerialPorts(ComboBox comboBox)
        {
            var portNames = SerialPort.GetPortNames()
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            comboBox.Items.Clear();
            comboBox.Items.AddRange(portNames.Length > 0
                ? portNames.Cast<object>().ToArray()
                : new object[] { "COM1" });
            comboBox.SelectedIndex = 0;
        }

        private void AddCameraButtonOnClick(object sender, EventArgs e) =>
            AddConfiguration(CommunicationDeviceType.Camera);

        private void AddPlcButtonOnClick(object sender, EventArgs e) =>
            AddConfiguration(CommunicationDeviceType.Plc);

        private void AddLightButtonOnClick(object sender, EventArgs e) =>
            AddConfiguration(CommunicationDeviceType.LightSource);

        private void AddConfiguration(CommunicationDeviceType deviceType)
        {
            var configuration = ViewModel.AddConfiguration(deviceType);
            deviceBindingSource.Position = ViewModel.DeviceConfigurations.IndexOf(configuration);
            UpdateEditor();
            nameTextBox.Focus();
            nameTextBox.SelectAll();
        }

        private void DeviceBindingSourceOnCurrentChanged(object sender, EventArgs e)
        {
            if (!_updatingEditor) UpdateEditor();
        }

        private void TopologyControlOnDeviceSelected(object sender,
            FrmViews.Controls.CommunicationDeviceSelectedEventArgs e)
        {
            deviceBindingSource.Position = ViewModel.DeviceConfigurations.IndexOf(e.Configuration);
            UpdateEditor();
        }

        private void SettingsTabControlOnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updatingEditor) return;

            CommunicationDeviceType targetType;
            if (settingsTabControl.SelectedTab == plcTabPage)
                targetType = CommunicationDeviceType.Plc;
            else if (settingsTabControl.SelectedTab == cameraTabPage)
                targetType = CommunicationDeviceType.Camera;
            else
                targetType = CommunicationDeviceType.LightSource;

            var configuration = ViewModel.DeviceConfigurations
                .FirstOrDefault(item => item.DeviceType == targetType);
            if (configuration == null)
            {
                UpdateEditor();
                return;
            }

            deviceBindingSource.Position =
                ViewModel.DeviceConfigurations.IndexOf(configuration);
        }

        private void UpdateEditor()
        {
            var configuration = SelectedConfiguration;
            _updatingEditor = true;
            try
            {
                detailsContentPanel.Enabled = configuration != null;
                emptyStateLabel.Visible = configuration == null;
                topologyControl.SelectConfiguration(configuration);
                if (configuration == null)
                {
                    detailsTypeLabel.Text = "未选择设备";
                    return;
                }

                detailsTypeLabel.Text = configuration.DeviceTypeText + "配置";
                nameTextBox.Text = configuration.Name ?? string.Empty;
                enabledCheckBox.Checked = configuration.Enabled;

                switch (configuration.DeviceType)
                {
                    case CommunicationDeviceType.Camera:
                        settingsTabControl.SelectedTab = cameraTabPage;
                        cameraHostTextBox.Text = configuration.Host ?? string.Empty;
                        cameraTimeoutNumeric.Value = Clamp(configuration.Timeout,
                            cameraTimeoutNumeric.Minimum, cameraTimeoutNumeric.Maximum);
                        break;
                    case CommunicationDeviceType.Plc:
                        settingsTabControl.SelectedTab = plcTabPage;
                        plcProtocolComboBox.SelectedItem = configuration.Protocol;
                        plcConnectionModeComboBox.SelectedItem =
                            GetDisplayConnectionMode(configuration.ConnectionMode);
                        plcHostTextBox.Text = configuration.Host ?? string.Empty;
                        plcPortNumeric.Value = Clamp(configuration.Port,
                            plcPortNumeric.Minimum, plcPortNumeric.Maximum);
                        SelectComboValue(plcSerialPortComboBox, configuration.SerialPort);
                        SelectComboValue(plcBaudComboBox, configuration.BaudRate.ToString());
                        plcStationNumeric.Value = Clamp(configuration.Station,
                            plcStationNumeric.Minimum, plcStationNumeric.Maximum);
                        SelectComboValue(_plcDataBitsComboBox,
                            configuration.DataBits.ToString());
                        SelectComboValue(_plcParityComboBox, configuration.Parity);
                        SelectComboValue(_plcStopBitsComboBox,
                            configuration.StopBits);
                        UpdatePlcConnectionMode();
                        BuildPlcProtocolParameterEditors(configuration,
                            configuration.Protocol);
                        break;
                    default:
                        settingsTabControl.SelectedTab = lightTabPage;
                        lightConnectionModeComboBox.SelectedItem =
                            GetDisplayConnectionMode(configuration.ConnectionMode);
                        lightHostTextBox.Text = configuration.Host ?? string.Empty;
                        lightPortNumeric.Value = Clamp(configuration.Port,
                            lightPortNumeric.Minimum, lightPortNumeric.Maximum);
                        SelectComboValue(lightSerialPortComboBox, configuration.SerialPort);
                        SelectComboValue(lightBaudComboBox, configuration.BaudRate.ToString());
                        SelectComboValue(lightDataBitsComboBox, configuration.DataBits.ToString());
                        SelectComboValue(lightParityComboBox, configuration.Parity);
                        SelectComboValue(lightStopBitsComboBox, configuration.StopBits);
                        UpdateLightConnectionMode();
                        break;
                }

                statusLabel.Text = "待保存";
                statusLabel.ForeColor = Color.FromArgb(100, 112, 128);
                ResetSelectedEditorScrollPosition();
            }
            finally
            {
                _updatingEditor = false;
            }
        }

        private static decimal Clamp(int value, decimal minimum, decimal maximum)
        {
            return Math.Max(minimum, Math.Min(maximum, value));
        }

        private static void SelectComboValue(ComboBox comboBox, string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && !comboBox.Items.Contains(value))
                comboBox.Items.Add(value);
            comboBox.SelectedItem = value;
            if (comboBox.SelectedIndex < 0 && comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        private void PlcProtocolComboBoxOnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updatingEditor) return;
            var protocol = plcProtocolComboBox.SelectedItem as string ?? string.Empty;
            SaveDisplayedPlcProtocolParameters(SelectedConfiguration);
            var serial = ViewModel.IsPlcSerialProtocol(protocol);
            plcConnectionModeComboBox.SelectedItem = serial
                ? SerialConnectionMode
                : NetworkConnectionMode;
            BuildPlcProtocolParameterEditors(SelectedConfiguration, protocol);
        }

        private void PlcConnectionModeOnSelectedIndexChanged(object sender, EventArgs e) =>
            UpdatePlcConnectionMode();

        private void LightConnectionModeOnSelectedIndexChanged(object sender, EventArgs e) =>
            UpdateLightConnectionMode();

        private void UpdatePlcConnectionMode()
        {
            var network = IsNetworkConnectionMode(
                plcConnectionModeComboBox.SelectedItem as string);
            plcNetworkPanel.Enabled = network;
            plcNetworkPanel.Visible = network;
            plcSerialPanel.Enabled = !network;
            plcSerialPanel.Visible = !network;
            plcFieldsLayout.RowStyles[2].Height = network ? 140F : 294F;
            if (network)
                plcNetworkPanel.BringToFront();
            else
                plcSerialPanel.BringToFront();
        }

        private void UpdateLightConnectionMode()
        {
            var network = IsNetworkConnectionMode(
                lightConnectionModeComboBox.SelectedItem as string);
            lightNetworkPanel.Enabled = network;
            lightNetworkPanel.Visible = network;
            lightSerialPanel.Enabled = !network;
            lightSerialPanel.Visible = !network;
            lightFieldsLayout.RowStyles[1].Height = network ? 140F : 260F;
            if (network)
                lightNetworkPanel.BringToFront();
            else
                lightSerialPanel.BringToFront();
        }

        private void SaveButtonOnClick(object sender, EventArgs e)
        {
            var configuration = SelectedConfiguration;
            if (configuration == null) return;
            errorProvider.Clear();

            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                errorProvider.SetError(nameTextBox, "请输入设备名称");
                nameTextBox.Focus();
                return;
            }

            configuration.Name = nameTextBox.Text.Trim();
            configuration.Enabled = enabledCheckBox.Checked;
            if (configuration.DeviceType == CommunicationDeviceType.Camera)
            {
                if (!ValidateHost(cameraHostTextBox)) return;
                configuration.Protocol = "ICMP Ping";
                configuration.ConnectionMode = "Ping";
                configuration.Host = cameraHostTextBox.Text.Trim();
                configuration.Port = 0;
                configuration.Timeout = Decimal.ToInt32(cameraTimeoutNumeric.Value);
            }
            else if (configuration.DeviceType == CommunicationDeviceType.Plc)
            {
                configuration.Protocol = plcProtocolComboBox.SelectedItem as string ?? string.Empty;
                configuration.ConnectionMode = plcConnectionModeComboBox.SelectedItem as string ??
                    NetworkConnectionMode;
                if (!SaveEndpoint(configuration, configuration.ConnectionMode,
                        plcHostTextBox, plcPortNumeric, plcSerialPortComboBox,
                        plcBaudComboBox)) return;
                configuration.Station = Decimal.ToInt32(plcStationNumeric.Value);
                configuration.DataBits = int.Parse(_plcDataBitsComboBox.Text);
                configuration.Parity = _plcParityComboBox.Text;
                configuration.StopBits = _plcStopBitsComboBox.Text;
                SavePlcProtocolParameters(configuration);
            }
            else
            {
                configuration.Protocol = lightConnectionModeComboBox.SelectedItem as string ??
                    NetworkConnectionMode;
                configuration.ConnectionMode = configuration.Protocol;
                if (!SaveEndpoint(configuration, configuration.ConnectionMode,
                        lightHostTextBox, lightPortNumeric, lightSerialPortComboBox,
                        lightBaudComboBox)) return;
                configuration.DataBits = int.Parse(lightDataBitsComboBox.Text);
                configuration.Parity = lightParityComboBox.Text;
                configuration.StopBits = lightStopBitsComboBox.Text;
            }

            deviceBindingSource.ResetCurrentItem();
            topologyControl.Invalidate();
            if (!TrySaveConfigurations()) return;
            statusLabel.Text = "已保存";
            statusLabel.ForeColor = Color.FromArgb(21, 146, 78);
        }

        private bool SaveEndpoint(DeviceConnectionConfiguration configuration,
            string connectionMode, TextBox hostTextBox, NumericUpDown portNumeric,
            ComboBox serialPortComboBox, ComboBox baudRateComboBox)
        {
            if (IsNetworkConnectionMode(connectionMode))
            {
                if (!ValidateHost(hostTextBox)) return false;
                configuration.Host = hostTextBox.Text.Trim();
                configuration.Port = Decimal.ToInt32(portNumeric.Value);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(serialPortComboBox.Text))
                {
                    errorProvider.SetError(serialPortComboBox, "请选择串口");
                    return false;
                }

                configuration.SerialPort = serialPortComboBox.Text;
                configuration.BaudRate = int.Parse(baudRateComboBox.Text);
            }

            return true;
        }

        private void BuildPlcProtocolParameterEditors(
            DeviceConnectionConfiguration configuration, string protocol)
        {
            _plcProtocolParametersLayout.SuspendLayout();
            try
            {
                _plcProtocolParametersLayout.Controls.Clear();
                _plcProtocolParametersLayout.RowStyles.Clear();
                _plcProtocolParameterEditors.Clear();

                var definitions = ViewModel.GetPlcProtocolParameters(protocol);
                if (IsSerialConnectionMode(
                        plcConnectionModeComboBox.SelectedItem as string))
                    definitions = definitions.Where(item => !string.Equals(
                        item.Name, "Station",
                        StringComparison.OrdinalIgnoreCase)).ToArray();
                if (definitions.Count == 0)
                {
                    _plcProtocolParametersLayout.Visible = false;
                    _displayedPlcProtocol = protocol ?? string.Empty;
                    return;
                }

                _plcProtocolParametersLayout.Visible = true;
                _plcProtocolParametersLayout.RowCount = definitions.Count + 1;
                _plcProtocolParametersLayout.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 36F));
                var title = new Label
                {
                    Text = "协议参数",
                    Dock = DockStyle.Fill,
                    Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(24, 32, 45),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                _plcProtocolParametersLayout.Controls.Add(title, 0, 0);
                _plcProtocolParametersLayout.SetColumnSpan(title, 3);

                Dictionary<string, string> values = GetProtocolValues(
                    configuration, protocol, false);
                for (int index = 0; index < definitions.Count; index++)
                {
                    PlcProtocolParameterDefinition definition = definitions[index];
                    int row = index + 1;
                    _plcProtocolParametersLayout.RowStyles.Add(
                        new RowStyle(SizeType.Absolute, 42F));
                    var label = new Label { Text = definition.DisplayName };
                    StyleFieldLabel(label);
                    label.AutoEllipsis = true;
                    label.Tag = definition.Name;

                    Control editor = CreateProtocolParameterEditor(definition);
                    string value;
                    if (values == null || !values.TryGetValue(definition.Name,
                            out value))
                        value = definition.DefaultValue;
                    SetProtocolEditorValue(editor, definition, value);
                    editor.Tag = definition;
                    _plcProtocolParameterEditors[definition.Name] = editor;
                    _plcProtocolParametersLayout.Controls.Add(label, 0, row);
                    _plcProtocolParametersLayout.Controls.Add(editor, 1, row);
                }
                _displayedPlcProtocol = protocol ?? string.Empty;
            }
            finally
            {
                _plcProtocolParametersLayout.ResumeLayout(true);
            }
        }

        private static Control CreateProtocolParameterEditor(
            PlcProtocolParameterDefinition definition)
        {
            if (definition.IsBoolean)
            {
                return new CheckBox
                {
                    AutoSize = true,
                    Text = "启用",
                    Font = new Font("Microsoft YaHei UI", 9F),
                    ForeColor = Color.FromArgb(24, 32, 45),
                    Anchor = AnchorStyles.Left,
                    Margin = new Padding(0, 10, 0, 8)
                };
            }

            if (definition.IsEnum)
            {
                var comboBox = new ComboBox();
                StyleComboBox(comboBox);
                comboBox.Items.AddRange(definition.Options.Cast<object>().ToArray());
                return comboBox;
            }

            if (definition.IsNumeric)
            {
                var numeric = new NumericUpDown
                {
                    DecimalPlaces = definition.ValueType == typeof(float) ||
                                    definition.ValueType == typeof(double) ||
                                    definition.ValueType == typeof(decimal)
                        ? 4
                        : 0,
                    ThousandsSeparator = false
                };
                SetNumericRange(numeric, definition.ValueType);
                StyleNumeric(numeric, numeric.Minimum, numeric.Maximum,
                    Math.Max(numeric.Minimum, Math.Min(numeric.Maximum, 0M)));
                return numeric;
            }

            var textBox = new TextBox();
            StyleTextBox(textBox);
            return textBox;
        }

        private static void SetProtocolEditorValue(Control editor,
            PlcProtocolParameterDefinition definition, string value)
        {
            var checkBox = editor as CheckBox;
            if (checkBox != null)
            {
                bool checkedValue;
                checkBox.Checked = bool.TryParse(value, out checkedValue) &&
                                   checkedValue;
                return;
            }

            var comboBox = editor as ComboBox;
            if (comboBox != null)
            {
                SelectComboValue(comboBox, value);
                return;
            }


            var numeric = editor as NumericUpDown;
            if (numeric != null)
            {
                decimal numericValue;
                if (decimal.TryParse(value,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out numericValue))
                    numeric.Value = Math.Max(numeric.Minimum,
                        Math.Min(numeric.Maximum, numericValue));
                return;
            }

            editor.Text = value ?? string.Empty;
        }

        private static void SetNumericRange(NumericUpDown numeric, Type type)
        {
            if (type == typeof(byte)) { numeric.Minimum = byte.MinValue; numeric.Maximum = byte.MaxValue; }
            else if (type == typeof(sbyte)) { numeric.Minimum = sbyte.MinValue; numeric.Maximum = sbyte.MaxValue; }
            else if (type == typeof(short)) { numeric.Minimum = short.MinValue; numeric.Maximum = short.MaxValue; }
            else if (type == typeof(ushort)) { numeric.Minimum = ushort.MinValue; numeric.Maximum = ushort.MaxValue; }
            else if (type == typeof(int)) { numeric.Minimum = int.MinValue; numeric.Maximum = int.MaxValue; }
            else if (type == typeof(uint)) { numeric.Minimum = uint.MinValue; numeric.Maximum = uint.MaxValue; }
            else if (type == typeof(long)) { numeric.Minimum = decimal.MinValue; numeric.Maximum = decimal.MaxValue; }
            else if (type == typeof(ulong)) { numeric.Minimum = 0M; numeric.Maximum = decimal.MaxValue; }
            else { numeric.Minimum = -1000000000000M; numeric.Maximum = 1000000000000M; }
        }

        private void SavePlcProtocolParameters(
            DeviceConnectionConfiguration configuration)
        {
            string protocol = plcProtocolComboBox.SelectedItem as string ??
                              string.Empty;
            SaveDisplayedPlcProtocolParameters(configuration);
            if (string.Equals(protocol, _displayedPlcProtocol,
                    StringComparison.OrdinalIgnoreCase)) return;
        }

        private void SaveDisplayedPlcProtocolParameters(
            DeviceConnectionConfiguration configuration)
        {
            string protocol = _displayedPlcProtocol;
            if (configuration == null || string.IsNullOrWhiteSpace(protocol))
                return;
            Dictionary<string, string> values = GetProtocolValues(configuration,
                protocol, true);
            foreach (KeyValuePair<string, Control> item in
                     _plcProtocolParameterEditors)
            {
                var checkBox = item.Value as CheckBox;
                var numeric = item.Value as NumericUpDown;
                values[item.Key] = checkBox != null
                    ? checkBox.Checked.ToString()
                    : numeric != null
                        ? numeric.Value.ToString(
                            System.Globalization.CultureInfo.InvariantCulture)
                        : item.Value.Text ?? string.Empty;
            }
        }

        private static Dictionary<string, string> GetProtocolValues(
            DeviceConnectionConfiguration configuration, string protocol,
            bool create)
        {
            if (configuration == null || string.IsNullOrWhiteSpace(protocol))
                return null;
            if (configuration.ProtocolParameters == null)
            {
                if (!create) return null;
                configuration.ProtocolParameters =
                    new Dictionary<string, Dictionary<string, string>>(
                        StringComparer.OrdinalIgnoreCase);
            }

            Dictionary<string, string> values;
            if (configuration.ProtocolParameters.TryGetValue(protocol, out values))
                return values;
            if (!create) return null;
            values = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);
            configuration.ProtocolParameters[protocol] = values;
            return values;
        }

        private static string GetDisplayConnectionMode(string connectionMode)
        {
            return IsSerialConnectionMode(connectionMode)
                ? SerialConnectionMode
                : NetworkConnectionMode;
        }

        private static bool IsNetworkConnectionMode(string connectionMode)
        {
            return string.Equals(connectionMode, NetworkConnectionMode,
                       StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(connectionMode, "TCP/IP",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSerialConnectionMode(string connectionMode)
        {
            return string.Equals(connectionMode, SerialConnectionMode,
                       StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(connectionMode, "RS232",
                       StringComparison.OrdinalIgnoreCase);
        }

        private bool ValidateHost(TextBox hostTextBox)
        {
            if (!string.IsNullOrWhiteSpace(hostTextBox.Text)) return true;
            errorProvider.SetError(hostTextBox, "请输入 IP 地址或主机名");
            hostTextBox.Focus();
            return false;
        }

        private void DeleteButtonOnClick(object sender, EventArgs e)
        {
            var configuration = SelectedConfiguration;
            if (configuration == null) return;
            if (MessageBox.Show(this, "确定删除“" + configuration.Name + "”吗？",
                    "删除设备", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) !=
                DialogResult.OK) return;

            var index = ViewModel.DeviceConfigurations.IndexOf(configuration);
            ViewModel.DeviceConfigurations.Remove(configuration);
            if (!TrySaveConfigurations())
            {
                ViewModel.DeviceConfigurations.Insert(index, configuration);
                deviceBindingSource.Position = index;
                UpdateEditor();
                return;
            }

            UpdateEditor();
            statusLabel.Text = "已删除";
            statusLabel.ForeColor = Color.FromArgb(21, 146, 78);
        }

        private bool TrySaveConfigurations()
        {
            try
            {
                ViewModel.SaveConfigurations();
                return true;
            }
            catch (Exception ex)
            {
                statusLabel.Text = "保存失败";
                statusLabel.ForeColor = Color.FromArgb(220, 38, 38);
                MessageBox.Show(this,
                    "配置文件写入失败：" + ex.Message + Environment.NewLine +
                    ViewModel.ConfigurationFilePath,
                    "保存配置", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void CloseButtonOnClick(object sender, EventArgs e) => Close();
    }
}
