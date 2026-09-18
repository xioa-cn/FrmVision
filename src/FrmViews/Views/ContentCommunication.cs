using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FrmMapper.Data;
using FrmServices.ViewModel;
using FrmViews.Controls;

namespace FrmViews.Views
{
    // 作者：xioa
    // 作者邮箱：1327916255@qq.com
    public partial class ContentCommunication : Form
    {
        private readonly ContentCommunicationViewModel _viewModel;
        private readonly bool _ownsViewModel;
        private readonly BindingSource _source = new BindingSource();
        private readonly DataGridView _list = new DataGridView();
        private readonly PropertyGrid _editor = new PropertyGrid();
        private readonly ComboBox _type = new ComboBox();
        private readonly TextBox _sendText = new TextBox();
        private readonly TextBox _log = new TextBox();
        private readonly Label _status = new Label();
        private readonly Label _heading = new Label();
        private readonly Button _save = new Button { Text = "保存配置" };
        private readonly Button _delete = new Button { Text = "删除" };
        private readonly Button _start = new Button { Text = "启动 / 连接" };
        private readonly Button _stop = new Button { Text = "停止 / 断开" };
        private readonly Button _send = new Button { Text = "发送" };
        private readonly Button _add = new Button { Text = "新增" };
        private readonly CheckBox _showHex = new CheckBox { Text = "显示原始 HEX", Checked = true, AutoSize = true };
        private readonly Queue<ReceivedMessage> _messages = new Queue<ReceivedMessage>();
        private readonly Timer _timer = new Timer { Interval = 500 };
        private ErrorProvider _errors;
        private ContentCommunicationConfiguration _draft;
        private bool _dirty, _updating, _busy;

        public ContentCommunication() : this(new ContentCommunicationViewModel(), true)
        {
        }

        public ContentCommunication(ContentCommunicationViewModel viewModel) : this(viewModel, false)
        {
        }

        private ContentCommunication(ContentCommunicationViewModel viewModel, bool ownsViewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            _ownsViewModel = ownsViewModel;
            InitializeComponent();
            BuildView();
            _source.DataSource = _viewModel.Configurations;
            _list.DataSource = _source;
            _source.CurrentChanged += SelectionChanged;
            _viewModel.MessageReceived += ReceiveMessage;
            _viewModel.BytesReceived += ReceiveBytes;
            _timer.Tick += RefreshStatus;
            Disposed += OnDisposed;
            FormClosing += OnClosing;
            Shown += (s, e) =>
            {
                if (_viewModel.ConfigurationLoadError != null) ShowError(_viewModel.ConfigurationLoadError);
                _timer.Start();
            };
            LoadDraft(_source.Current as ContentCommunicationConfiguration);
        }

        private void BuildView()
        {
            Text = "内部通讯配置";
            Font = new Font("Microsoft YaHei UI", 9F);
            BackColor = UiTheme.Page;
            ForeColor = UiTheme.Text;
            StartPosition = FormStartPosition.CenterParent;
            AutoValidate = AutoValidate.EnableAllowFocusChange;
            MinimumSize = new Size(960, 640);
            ClientSize = new Size(1120, 720);
            _errors = new ErrorProvider(components)
                { ContainerControl = this, BlinkStyle = ErrorBlinkStyle.NeverBlink };
            var root = new TableLayoutPanel
                { Dock = DockStyle.Fill, Padding = new Padding(16), ColumnCount = 1, RowCount = 4 };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            root.Controls.Add(
                new Label { Text = "内部通讯配置", AutoSize = true, Font = new Font(Font.FontFamily, 17, FontStyle.Bold) }, 0,
                0);
            var toolbar = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            _type.DropDownStyle = ComboBoxStyle.DropDownList;
            _type.Width = 175;
            foreach (ContentCommunicationType type in Enum.GetValues(typeof(ContentCommunicationType)))
                _type.Items.Add(ContentCommunicationConfiguration.GetTypeText(type));
            _type.SelectedIndex = 0;
            toolbar.Controls.Add(_type);
            foreach (var button in new[] { _add, _save, _delete, _start, _stop })
            {
                UiTheme.StyleCommandButton(button, button == _save);
                button.Margin = new Padding(6, 0, 0, 0);
                toolbar.Controls.Add(button);
            }

            root.Controls.Add(toolbar, 0, 1);
            var workspace = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            workspace.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48));
            workspace.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52));
            _list.Dock = DockStyle.Fill;
            _list.AutoGenerateColumns = false;
            _list.AllowUserToAddRows = false;
            _list.AllowUserToDeleteRows = false;
            _list.ReadOnly = true;
            _list.MultiSelect = false;
            _list.RowHeadersVisible = false;
            _list.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _list.BackgroundColor = UiTheme.Surface;
            _list.BorderStyle = BorderStyle.None;
            _list.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            foreach (var field in new[]
                         { new[] { "Name", "名称" }, new[] { "TypeText", "类型" }, new[] { "Endpoint", "地址" } })
                _list.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = field[0], HeaderText = field[1] });
            _list.Columns.Add(new DataGridViewTextBoxColumn { Name = "RuntimeStatus", HeaderText = "状态" });
            _list.CellFormatting += (s, e) =>
            {
                if (e.RowIndex >= 0 && _list.Columns[e.ColumnIndex].Name == "RuntimeStatus")
                {
                    var item = _list.Rows[e.RowIndex].DataBoundItem as ContentCommunicationConfiguration;
                    e.Value = _viewModel.GetStatus(item == null ? null : item.Id);
                }
            };
            workspace.Controls.Add(_list, 0, 0);
            var details = new TableLayoutPanel
                { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5, Padding = new Padding(12, 0, 0, 0) };
            details.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            details.RowStyles.Add(new RowStyle(SizeType.Percent, 65));
            details.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            details.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            details.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
            _heading.Dock = DockStyle.Fill;
            details.Controls.Add(_heading, 0, 0);
            _editor.Dock = DockStyle.Fill;
            _editor.ToolbarVisible = false;
            _editor.PropertySort = PropertySort.Categorized;
            _editor.PropertyValueChanged += (s, e) =>
            {
                _dirty = true;
                _errors.Clear();
                UpdateButtons();
            };
            _editor.Validating += (s, e) =>
            {
                string error = _draft == null ? null : ContentCommunicationViewModel.ValidateConfiguration(_draft);
                _errors.SetError(_editor, error ?? string.Empty);
                e.Cancel = error != null;
            };
            details.Controls.Add(_editor, 0, 1);
            var debugHeader = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            debugHeader.Controls.Add(new Label
                { Text = "文本调试（发送不自动加换行）", AutoSize = true, Margin = new Padding(0, 8, 8, 0) });
            _showHex.Margin = new Padding(0, 7, 0, 0);
            debugHeader.Controls.Add(_showHex);
            details.Controls.Add(debugHeader, 0, 2);
            var sendRow = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            sendRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            sendRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            _sendText.Dock = DockStyle.Fill;
            UiTheme.StyleTextBox(_sendText);
            _send.Dock = DockStyle.Fill;
            UiTheme.StyleCommandButton(_send, false);
            sendRow.Controls.Add(_sendText, 0, 0);
            sendRow.Controls.Add(_send, 1, 0);
            details.Controls.Add(sendRow, 0, 3);
            _log.Multiline = true;
            _log.ReadOnly = true;
            _log.ScrollBars = ScrollBars.Vertical;
            _log.Dock = DockStyle.Fill;
            _log.BackColor = UiTheme.SurfaceMuted;
            details.Controls.Add(_log, 0, 4);
            workspace.Controls.Add(details, 1, 0);
            root.Controls.Add(workspace, 0, 2);
            _status.Dock = DockStyle.Fill;
            _status.TextAlign = ContentAlignment.MiddleLeft;
            root.Controls.Add(_status, 0, 3);
            Controls.Add(root);
            _add.Click += (s, e) =>
            {
                if (ConfirmDraft())
                {
                    LoadDraft(_viewModel.CreateConfiguration((ContentCommunicationType)_type.SelectedIndex));
                    _dirty = true;
                    UpdateButtons();
                }
            };
            _save.Click += (s, e) => SaveDraft();
            _delete.Click += DeleteSelected;
            _start.Click += async (s, e) =>
            {
                if (_draft == null || (_dirty && !SaveDraft())) return;
                string id = _draft.Id;
                await RunOperation(() => _viewModel.StartAsync(id), "启动成功");
            };
            _stop.Click += async (s, e) =>
            {
                if (_draft != null)
                {
                    string id = _draft.Id;
                    await RunOperation(() => _viewModel.StopAsync(id), "已停止");
                }
            };
            _send.Click += async (s, e) =>
            {
                if (_draft == null) return;
                string id = _draft.Id, text = _sendText.Text;
                await RunOperation(() => _viewModel.SendAsync(id, text), "发送：" + text);
            };
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            if (_updating) return;
            var selected = _source.Current as ContentCommunicationConfiguration;
            if (selected != null && _draft != null && selected.Id == _draft.Id) return;
            int position = _source.Position;
            if (!ConfirmDraft())
            {
                _updating = true;
                _source.Position = _draft == null
                    ? -1
                    : _viewModel.Configurations.ToList().FindIndex(c => c.Id == _draft.Id);
                _updating = false;
                return;
            }

            _updating = true;
            _source.Position = position;
            _updating = false;
            LoadDraft(selected);
        }

        private void LoadDraft(ContentCommunicationConfiguration configuration)
        {
            _draft = configuration == null ? null : configuration.Clone();
            _dirty = false;
            _errors.Clear();
            _editor.SelectedObject = _draft == null ? null : new ProtocolProperties(_draft);
            _heading.Text = _draft == null ? "请选择配置，或点击新增" : _draft.TypeText;
            UpdateButtons();
        }

        private bool SaveDraft()
        {
            if (_draft == null || !ValidateChildren()) return false;
            _updating = true;
            Result result;
            try
            {
                result = _viewModel.SaveConfiguration(_draft);
            }
            finally
            {
                _updating = false;
            }

            if (!result.IsSuccess)
            {
                _errors.SetError(_editor, result.Message);
                ShowError(result.Message);
                return false;
            }

            _dirty = false;
            _updating = true;
            _source.Position = _viewModel.Configurations.ToList().FindIndex(c => c.Id == _draft.Id);
            _updating = false;
            LoadDraft(_source.Current as ContentCommunicationConfiguration);
            AppendLog("配置已保存");
            return true;
        }

        private bool ConfirmDraft()
        {
            if (!_dirty) return true;
            var answer = MessageBox.Show(this, "当前配置尚未保存，是否保存？", Text, MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);
            return answer == DialogResult.No || (answer == DialogResult.Yes && SaveDraft());
        }

        private void DeleteSelected(object sender, EventArgs e)
        {
            if (_draft == null) return;
            if (MessageBox.Show(this, "确定删除“" + _draft.Name + "”？", Text, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes) return;
            if (!_viewModel.Configurations.Any(c => c.Id == _draft.Id))
            {
                LoadDraft(_source.Current as ContentCommunicationConfiguration);
                return;
            }

            _updating = true;
            Result result;
            try
            {
                result = _viewModel.DeleteConfiguration(_draft.Id);
            }
            finally
            {
                _updating = false;
            }

            if (!result.IsSuccess)
            {
                ShowError(result.Message);
                return;
            }

            LoadDraft(_source.Current as ContentCommunicationConfiguration);
        }

        private async Task RunOperation(Func<Task<Result>> operation, string success)
        {
            _busy = true;
            UpdateButtons();
            try
            {
                var result = await operation();
                if (IsDisposed) return;
                if (result.IsSuccess) AppendLog(success);
                else ShowError(result.Message);
            }
            catch (Exception ex)
            {
                if (!IsDisposed) ShowError(ex.Message);
            }
            finally
            {
                _busy = false;
                if (!IsDisposed)
                {
                    var saved = _draft == null
                        ? null
                        : _viewModel.Configurations.FirstOrDefault(c => c.Id == _draft.Id);
                    if (saved != null && !_dirty)
                    {
                        _draft.AutoStart = saved.AutoStart;
                        _editor.Refresh();
                    }

                    UpdateButtons();
                }
            }
        }

        private void UpdateButtons()
        {
            bool selected = _draft != null;
            bool running = selected && _viewModel.HasRuntime(_draft.Id);
            _list.Enabled = _type.Enabled = _add.Enabled = !_busy;
            _editor.Enabled = selected && !_busy && !running;
            _save.Enabled = _delete.Enabled = selected && !_busy && !running;
            _start.Enabled = selected && !_busy && (!running || _viewModel.GetStatus(_draft.Id) == "已断开");
            _stop.Enabled = selected && !_dirty && (running || _draft.AutoStart) && !_busy
                            && _viewModel.Configurations.Any(c => c.Id == _draft.Id);
            bool textTransport = selected && (_draft.Type == ContentCommunicationType.TCPSERVICE ||
                                              _draft.Type == ContentCommunicationType.TCPCLIENT ||
                                              _draft.Type == ContentCommunicationType.UDPSERVICE ||
                                              _draft.Type == ContentCommunicationType.SERIALPORT);
            _send.Enabled = textTransport && running && !_busy;
            _sendText.Enabled = textTransport && !_busy;
            _status.Text = selected
                ? (_dirty ? "未保存 · " : "") + _draft.Name + " · " + _viewModel.GetStatus(_draft.Id)
                : "新增通讯配置后保存，再点击启动。关闭此窗口后通讯继续运行。";
        }

        private void ReceiveMessage(string id, string text)
        {
            EnqueueMessage(new ReceivedMessage
                { Id = id, Text = text.Length > 4000 ? text.Substring(0, 4000) + "…" : text });
        }

        private void ReceiveBytes(string id, byte[] bytes)
        {
            EnqueueMessage(new ReceivedMessage { Id = id, Bytes = bytes });
        }

        private void EnqueueMessage(ReceivedMessage message)
        {
            // Bound both queued messages and visible text; do not flood the UI message pump.
            lock (_messages)
            {
                if (_messages.Count >= 200) _messages.Dequeue();
                _messages.Enqueue(message);
            }
        }

        private void RefreshStatus(object sender, EventArgs e)
        {
            UpdateButtons();
            _list.Invalidate();
            var pending = new List<ReceivedMessage>();
            lock (_messages)
            {
                for (int i = 0; i < 50 && _messages.Count > 0; i++) pending.Add(_messages.Dequeue());
            }

            foreach (var message in pending)
            {
                var configuration = _viewModel.Configurations.FirstOrDefault(c => c.Id == message.Id);
                string prefix = message.Time.ToString("HH:mm:ss") + " [" +
                                (configuration == null ? message.Id : configuration.Name) + "] ";
                if (message.Bytes != null)
                {
                    if (_showHex.Checked)
                        AppendLog(prefix + "接收 HEX (" + message.Bytes.Length + " B)：" +
                                  BitConverter.ToString(message.Bytes).Replace('-', ' '));
                    continue;
                }

                bool invalid = message.Text.IndexOf('\uFFFD') >= 0;
                AppendLog(prefix + "接收文本 (" + (configuration == null ? "" : configuration.EncodingName) + ")：" +
                          EscapeText(message.Text));
                if (invalid)
                    AppendLog("提示：文本包含无法解码的字符，请核对发送端编码；GBK/GB2312 文本请选择 gb2312，二进制数据请查看原始 HEX。修改编码需停止、保存后重新启动。");
            }
        }

        private static string EscapeText(string text)
        {
            var display = new StringBuilder();
            foreach (char value in text)
            {
                switch (value)
                {
                    case '\r': display.Append("\\r"); break;
                    case '\n': display.Append("\\n"); break;
                    case '\t': display.Append("\\t"); break;
                    case '\uFFFD': display.Append("[无法解码]"); break;
                    default:
                        if (char.IsControl(value)) display.Append("\\x" + ((int)value).ToString("X2"));
                        else display.Append(value);
                        break;
                }
            }

            return display.ToString();
        }

        // 作者：xioa
        // 作者邮箱：1327916255@qq.com
        private sealed class ReceivedMessage
        {
            public string Id;
            public string Text;
            public byte[] Bytes;
            public DateTime Time = DateTime.Now;
        }

        private void AppendLog(string text)
        {
            if (_log.TextLength > 32000) _log.Text = _log.Text.Substring(_log.TextLength - 16000);
            _log.AppendText(text + Environment.NewLine);
        }

        private void ShowError(string message)
        {
            AppendLog("失败：" + message);
            MessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            if (_busy)
            {
                e.Cancel = true;
                return;
            }

            e.Cancel = !ConfirmDraft();
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            _timer.Dispose();
            _source.Dispose();
            _viewModel.MessageReceived -= ReceiveMessage;
            _viewModel.BytesReceived -= ReceiveBytes;
            if (_ownsViewModel) _viewModel.Dispose();
        }

        // Expose only fields used by the selected protocol, keeping the persisted DTO independent of WinForms.
        // 作者：xioa
        // 作者邮箱：1327916255@qq.com
        private sealed class ProtocolProperties : CustomTypeDescriptor
        {
            private readonly ContentCommunicationConfiguration _configuration;

            public ProtocolProperties(ContentCommunicationConfiguration configuration)
            {
                _configuration = configuration;
            }

            public override object GetPropertyOwner(PropertyDescriptor pd)
            {
                return _configuration;
            }

            public override PropertyDescriptorCollection GetProperties()
            {
                return GetProperties(null);
            }

            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                var names = new List<string> { "Name", "AutoStart" };
                var type = _configuration.Type;
                if (type == ContentCommunicationType.TCPSERVICE || type == ContentCommunicationType.TCPCLIENT || type == ContentCommunicationType.UDPSERVICE)
                {
                    names.Add("Port");
                    names.AddRange(new[] { "Host", "EncodingName", "ReceiveLogEnabled" });
                    if (type == ContentCommunicationType.TCPCLIENT)
                        names.AddRange(new[] { "ConnectTimeout", "AutoReconnect", "ReconnectInterval" });
                }
                else if (type == ContentCommunicationType.SERIALPORT)
                {
                    names.AddRange(new[]
                    {
                        "SerialPort", "BaudRate", "DataBits", "Parity", "StopBits", "ReceiveIdleMilliseconds", "EncodingName", "ReceiveLogEnabled"
                    });
                }
                else
                {
                    if (type == ContentCommunicationType.MODBUSTCP) names.Add("Port");
                    names.AddRange(new[] { "Station", "DataFormat" });
                    if (type == ContentCommunicationType.MODBUSRTU)
                        names.AddRange(new[] { "SerialPort", "BaudRate", "DataBits", "Parity", "StopBits" });
                }

                return new PropertyDescriptorCollection(TypeDescriptor.GetProperties(_configuration)
                    .Cast<PropertyDescriptor>()
                    .Where(p => names.Contains(p.Name)).ToArray(), true);
            }
        }
    }
}