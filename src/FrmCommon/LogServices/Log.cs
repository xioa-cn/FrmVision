using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using FrmCommon.FrmExtensions;
using FrmCommon.Mvvm;

namespace FrmServices.LogServices
{
    public partial class Log : ViewModelControl, IViewModelFrm<LogViewModel>
    {
        private static readonly Color PageColor = Color.FromArgb(245, 247, 250);
        private static readonly Color SurfaceColor = Color.White;
        private static readonly Color TextColor = Color.FromArgb(24, 32, 45);
        private static readonly Color MutedColor = Color.FromArgb(100, 112, 128);
        private static readonly Color BorderColor = Color.FromArgb(220, 225, 232);
        private static readonly Color InputColor = Color.FromArgb(249, 250, 252);
        private static readonly Color PrimaryColor = Color.FromArgb(36, 99, 235);
        private static readonly Font IconFont = new Font("Segoe MDL2 Assets", 10.5F);
        private const string SearchGlyph = "\uE721";
        private const string DeleteGlyph = "\uE74D";
        private const string FolderGlyph = "\uE8B7";
        private const string SaveGlyph = "\uE74E";
        private const string OpenGlyph = "\uE838";
        private const string CalendarGlyph = "\uE787";

        private BindingSource _liveSource;
        private BindingSource _historySource;
        private BindingList<LogEntry> _liveItems;
        private BindingList<LogEntry> _historyItems;
        private ComboBox _liveLevelFilter;
        private ComboBox _historyLevelFilter;
        private TextBox _liveKeyword;
        private TextBox _historyKeyword;
        private CheckBox _pauseToggle;
        private Label _liveCountLabel;
        private StatusIndicator _statusLabel;
        private Label _historyCountLabel;
        private DateTimePicker _historyFrom;
        private DateTimePicker _historyTo;
        private Button _historySearchButton;
        private TextBox _pathTextBox;
        private NumericUpDown _retentionDays;
        private Button _saveSettingsButton;
        private ErrorProvider _settingsErrors;
        private Timer _statusResetTimer;
        private Panel _pageHost;
        private Panel[] _pages;
        private NavigationButton[] _navigationButtons;
        private bool _historyLoaded;
        private int _pausedEntryCount;
        private readonly object _pendingLiveEntriesRoot = new object();
        private readonly List<LogEntry> _pendingLiveEntries =
            new List<LogEntry>();
        private System.Threading.Timer _liveEntryBatchTimer;
        private int _liveEntryBatchScheduled;
        private int _resourcesReleased;
        private const int LiveEntryBatchIntervalMilliseconds = 250;

        public Log() : this(new LogViewModel())
        {
        }

        public Log(LogService service) : this(new LogViewModel(service))
        {
        }

        private Log(LogViewModel viewModel) : base()
        {
            DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            InitializeComponent();
            BuildInterface();
            _liveEntryBatchTimer = new System.Threading.Timer(
                LiveEntryBatchTimerOnElapsed, null,
                System.Threading.Timeout.Infinite,
                System.Threading.Timeout.Infinite);
            SubscribeToViewModel();
            FrmBinding();
            RefreshLiveGrid();
        }

        public object DataContext { get; set; }

        public LogViewModel ViewModel => (LogViewModel)DataContext;

        [Category("日志限制")]
        [Description("实时日志页面在内存中保留并显示的最大条数。")]
        [DefaultValue(LogViewModel.DefaultMaximumLiveEntries)]
        public int MaximumLiveEntries
        {
            get => ViewModel.MaximumLiveEntries;
            set
            {
                ViewModel.MaximumLiveEntries = value;
                RefreshLiveGrid();
            }
        }

        [Category("日志限制")]
        [Description("一次历史日志查询最多显示的最新日志条数。")]
        [DefaultValue(LogViewModel.DefaultMaximumHistoryEntries)]
        public int MaximumHistoryEntries
        {
            get => ViewModel.MaximumHistoryEntries;
            set => ViewModel.MaximumHistoryEntries = value;
        }

        public void ShowLiveLogs() => SelectPage(0);

        public void ShowHistoryLogs() => SelectPage(1);

        public void ShowStorageSettings() => SelectPage(2);

        public override void FrmBinding()
        {
            this.SetBinding();
            base.FrmBinding();
        }

        private void BuildInterface()
        {
            SuspendLayout();
            Controls.Clear();
            if (components == null) components = new Container();
            _statusResetTimer = new Timer(components) { Interval = 3200 };
            _statusResetTimer.Tick += (sender, args) =>
            {
                _statusResetTimer.Stop();
                RestoreDefaultStatus();
            };

            BackColor = PageColor;
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            MinimumSize = new Size(420, 320);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = SurfaceColor,
                ColumnCount = 1,
                RowCount = 3,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            root.Controls.Add(CreateHeader(), 0, 0);
            root.Controls.Add(CreateNavigation(), 0, 1);

            _pageHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SurfaceColor,
                Padding = new Padding(14, 12, 14, 14),
                Margin = Padding.Empty
            };
            _pages = new[] { CreateLivePage(), CreateHistoryPage(), CreateSettingsPage() };
            foreach (var page in _pages)
            {
                page.Visible = false;
                _pageHost.Controls.Add(page);
            }
            root.Controls.Add(_pageHost, 0, 2);
            var shell = new RoundedSurfacePanel(12, Color.FromArgb(232, 236, 242))
            {
                Dock = DockStyle.Fill,
                BackColor = SurfaceColor,
                Margin = Padding.Empty
            };
            shell.Controls.Add(root);
            var backdrop = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = PageColor,
                Padding = new Padding(14),
                Margin = Padding.Empty
            };
            backdrop.Controls.Add(shell);
            Controls.Add(backdrop);
            SelectPage(0);
            ResumeLayout(true);
        }

        private Control CreateHeader()
        {
            var header = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = SurfaceColor,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(24, 0, 24, 0),
                Margin = Padding.Empty
            };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            header.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var title = new Label
            {
                Dock = DockStyle.Fill,
                Text = "日志中心",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = TextColor,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true,
                Margin = Padding.Empty
            };
            _statusLabel = new StatusIndicator
            {
                Dock = DockStyle.Fill,
                Text = "实时记录中",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(21, 146, 78),
                Margin = Padding.Empty
            };
            header.Controls.Add(title, 0, 0);
            header.Controls.Add(_statusLabel, 1, 0);
            return header;
        }

        private Control CreateNavigation()
        {
            var navigation = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = SurfaceColor,
                ColumnCount = 4,
                RowCount = 1,
                Padding = new Padding(16, 0, 16, 0),
                Margin = Padding.Empty
            };
            navigation.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112F));
            navigation.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112F));
            navigation.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112F));
            navigation.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            navigation.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            _navigationButtons = new[]
            {
                new NavigationButton("实时日志"),
                new NavigationButton("历史日志"),
                new NavigationButton("存储设置")
            };
            for (var index = 0; index < _navigationButtons.Length; index++)
            {
                var pageIndex = index;
                _navigationButtons[index].Click += (sender, args) => SelectPage(pageIndex);
                navigation.Controls.Add(_navigationButtons[index], index, 0);
            }

            navigation.Paint += (sender, args) =>
            {
                using (var pen = new Pen(BorderColor))
                    args.Graphics.DrawLine(pen, 0, navigation.Height - 1, navigation.Width, navigation.Height - 1);
            };
            return navigation;
        }

        private Panel CreateLivePage()
        {
            var page = CreatePageSurface();
            _liveLevelFilter = CreateLevelFilter();
            _liveKeyword = CreateTextBox();
            _liveKeyword.AccessibleName = "实时日志关键词";
            SetCueBanner(_liveKeyword, "输入关键词搜索");
            _pauseToggle = CreateToggleButton("暂停");
            _liveCountLabel = CreateMutedLabel("共 0 条");
            var clearButton = CreateButton("清空", false, 96, DeleteGlyph, true);

            var levelField = new LabeledField(null, _liveLevelFilter, true);
            var keywordField = new LabeledField(null, _liveKeyword, true);
            var toolbar = new LiveToolbar(levelField, keywordField, _pauseToggle, clearButton, _liveCountLabel)
            {
                Dock = DockStyle.Top
            };
            var grid = CreateLogGrid();
            _liveSource = new BindingSource(components);
            _liveItems = new BindingList<LogEntry>();
            _liveSource.DataSource = _liveItems;
            grid.DataSource = _liveSource;

            _liveLevelFilter.SelectedIndexChanged += (sender, args) => RefreshLiveGrid();
            _liveKeyword.TextChanged += (sender, args) => RefreshLiveGrid();
            _pauseToggle.CheckedChanged += PauseToggleOnCheckedChanged;
            clearButton.Click += (sender, args) =>
            {
                ViewModel.ClearLiveEntries();
                RefreshLiveGrid();
            };

            page.Controls.Add(grid);
            page.Controls.Add(toolbar);
            return page;
        }

        private Panel CreateHistoryPage()
        {
            var page = CreatePageSurface();
            _historyFrom = CreateDateTimePicker(DateTime.Today.AddDays(-7));
            _historyTo = CreateDateTimePicker(DateTime.Today);
            _historyLevelFilter = CreateLevelFilter();
            _historyKeyword = CreateTextBox();
            _historyKeyword.AccessibleName = "历史日志关键词";
            SetCueBanner(_historyKeyword, "输入关键词筛选");
            _historySearchButton = CreateButton("查询", true, 94, SearchGlyph);
            _historyCountLabel = CreateMutedLabel(string.Empty);

            var toolbar = new HistoryToolbar(
                new LabeledField("开始日期", _historyFrom),
                new LabeledField("结束日期", _historyTo),
                new LabeledField("等级", _historyLevelFilter),
                new LabeledField("关键词", _historyKeyword),
                _historySearchButton,
                _historyCountLabel)
            {
                Dock = DockStyle.Top
            };
            var grid = CreateLogGrid();
            _historySource = new BindingSource(components);
            _historyItems = new BindingList<LogEntry>();
            _historySource.DataSource = _historyItems;
            grid.DataSource = _historySource;

            _historySearchButton.Click += async (sender, args) => await LoadHistoryAsync();
            _historyKeyword.KeyDown += async (sender, args) =>
            {
                if (args.KeyCode != Keys.Enter) return;
                args.SuppressKeyPress = true;
                await LoadHistoryAsync();
            };

            page.Controls.Add(grid);
            page.Controls.Add(toolbar);
            return page;
        }

        private Panel CreateSettingsPage()
        {
            var page = CreatePageSurface();
            page.AutoScroll = true;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = SurfaceColor,
                ColumnCount = 2,
                RowCount = 8,
                Padding = new Padding(30, 26, 30, 30),
                Margin = Padding.Empty
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 146F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));

            var heading = new Label
            {
                Dock = DockStyle.Fill,
                Text = "日志存储",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = TextColor,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = Padding.Empty
            };
            _pathTextBox = CreateTextBox();
            _pathTextBox.Text = ViewModel.LogDirectory;
            _pathTextBox.Dock = DockStyle.Fill;
            _pathTextBox.Margin = new Padding(0, 2, 12, 4);
            var browseButton = CreateButton("选择目录", false, 132, FolderGlyph);
            browseButton.Dock = DockStyle.Fill;
            browseButton.Margin = new Padding(0, 2, 0, 4);

            _retentionDays = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 3650,
                Value = ViewModel.RetentionDays,
                Width = 138,
                Height = 32,
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = InputColor,
                ForeColor = TextColor,
                Margin = Padding.Empty
            };
            var retentionHost = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = SurfaceColor,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            var daysLabel = CreateMutedLabel("天");
            daysLabel.Margin = new Padding(10, 7, 0, 0);
            retentionHost.Controls.Add(_retentionDays);
            retentionHost.Controls.Add(daysLabel);

            _saveSettingsButton = CreateButton("保存设置", true, 138, SaveGlyph);
            var openFolderButton = CreateButton("打开目录", false, 138, OpenGlyph);
            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = SurfaceColor,
                Margin = Padding.Empty,
                Padding = new Padding(0, 6, 0, 0)
            };
            actions.Controls.Add(_saveSettingsButton);
            actions.Controls.Add(openFolderButton);

            layout.Controls.Add(heading, 0, 0);
            layout.SetColumnSpan(heading, 2);
            AddSettingsLabel(layout, "保存路径", 1);
            layout.Controls.Add(_pathTextBox, 0, 2);
            layout.Controls.Add(browseButton, 1, 2);
            AddSettingsLabel(layout, "最长保留天数", 4);
            layout.Controls.Add(retentionHost, 0, 5);
            layout.SetColumnSpan(retentionHost, 2);
            layout.Controls.Add(actions, 0, 7);
            layout.SetColumnSpan(actions, 2);

            _settingsErrors = new ErrorProvider(components)
            {
                ContainerControl = this,
                BlinkStyle = ErrorBlinkStyle.NeverBlink
            };
            browseButton.Click += BrowseButtonOnClick;
            _saveSettingsButton.Click += async (sender, args) => await SaveSettingsAsync();
            openFolderButton.Click += OpenFolderButtonOnClick;
            _pathTextBox.Validating += PathTextBoxOnValidating;

            page.Controls.Add(layout);
            return page;
        }

        private static void AddSettingsLabel(TableLayoutPanel layout, string text, int row)
        {
            var label = new Label
            {
                Dock = DockStyle.Fill,
                Text = text,
                Font = new Font("Segoe UI", 9F),
                ForeColor = MutedColor,
                TextAlign = ContentAlignment.BottomLeft,
                Margin = Padding.Empty
            };
            layout.Controls.Add(label, 0, row);
            layout.SetColumnSpan(label, 2);
        }

        private void SelectPage(int index)
        {
            if (_pages == null || index < 0 || index >= _pages.Length) return;
            for (var i = 0; i < _pages.Length; i++)
            {
                _pages[i].Visible = i == index;
                _navigationButtons[i].Active = i == index;
            }
            _pages[index].BringToFront();

            if (index == 1 && !_historyLoaded)
                BeginInvoke(new Action(async () => await LoadHistoryAsync()));
        }

        private void SubscribeToViewModel()
        {
            ViewModel.LiveEntryReceived += ViewModelOnLiveEntryReceived;
            ViewModel.StorageError += ViewModelOnStorageError;
            Disposed += (sender, args) =>
                ReleaseLogResources();
        }

        private void ViewModelOnLiveEntryReceived(object sender, LogEntryEventArgs e)
        {
            if (e == null || e.Entry == null || IsDisposed || Disposing ||
                _resourcesReleased != 0) return;
            lock (_pendingLiveEntriesRoot)
            {
                _pendingLiveEntries.Add(e.Entry);
                int maximumPending = Math.Max(100, MaximumLiveEntries);
                if (_pendingLiveEntries.Count > maximumPending)
                    _pendingLiveEntries.RemoveRange(0,
                        _pendingLiveEntries.Count - maximumPending);
            }

            if (System.Threading.Interlocked.CompareExchange(
                    ref _liveEntryBatchScheduled, 1, 0) != 0)
                return;
            _liveEntryBatchTimer?.Change(LiveEntryBatchIntervalMilliseconds,
                System.Threading.Timeout.Infinite);
        }

        private void LiveEntryBatchTimerOnElapsed(object state)
        {
            if (IsDisposed || Disposing || _resourcesReleased != 0)
            {
                System.Threading.Interlocked.Exchange(
                    ref _liveEntryBatchScheduled, 0);
                return;
            }

            try
            {
                BeginInvoke(new MethodInvoker(ApplyPendingLiveEntries));
            }
            catch (ObjectDisposedException)
            {
                System.Threading.Interlocked.Exchange(
                    ref _liveEntryBatchScheduled, 0);
            }
            catch (InvalidOperationException)
            {
                System.Threading.Interlocked.Exchange(
                    ref _liveEntryBatchScheduled, 0);
            }
        }

        private void ApplyPendingLiveEntries()
        {
            LogEntry[] entries;
            lock (_pendingLiveEntriesRoot)
            {
                entries = _pendingLiveEntries.ToArray();
                _pendingLiveEntries.Clear();
            }
            System.Threading.Interlocked.Exchange(ref _liveEntryBatchScheduled, 0);

            if (IsDisposed || Disposing || _resourcesReleased != 0 ||
                entries.Length == 0) return;
            if (_pauseToggle.Checked)
            {
                _pausedEntryCount += entries.Length;
                _statusLabel.Text = $"已暂停 · {_pausedEntryCount} 条待显示";
            }
            else
            {
                int maximumEntries = MaximumLiveEntries;
                LogLevel? selectedLevel = SelectedLevel(_liveLevelFilter);
                string keyword = _liveKeyword.Text;
                var visibleEntries = new List<LogEntry>(maximumEntries);
                for (int index = entries.Length - 1;
                     index >= 0 && visibleEntries.Count < maximumEntries;
                     index--)
                {
                    LogEntry entry = entries[index];
                    if (Matches(entry, selectedLevel, keyword))
                        visibleEntries.Add(entry);
                }

                foreach (LogEntry entry in _liveItems)
                {
                    if (visibleEntries.Count >= maximumEntries) break;
                    visibleEntries.Add(entry);
                }

                _liveItems = new BindingList<LogEntry>(visibleEntries);
                _liveSource.DataSource = _liveItems;
                UpdateLiveCount();
            }

            lock (_pendingLiveEntriesRoot)
            {
                if (_pendingLiveEntries.Count == 0) return;
            }
            if (System.Threading.Interlocked.CompareExchange(
                    ref _liveEntryBatchScheduled, 1, 0) == 0)
                _liveEntryBatchTimer?.Change(LiveEntryBatchIntervalMilliseconds,
                    System.Threading.Timeout.Infinite);
        }

        private void ReleaseLogResources()
        {
            if (System.Threading.Interlocked.Exchange(
                    ref _resourcesReleased, 1) != 0) return;

            ViewModel.LiveEntryReceived -= ViewModelOnLiveEntryReceived;
            ViewModel.StorageError -= ViewModelOnStorageError;

            System.Threading.Timer batchTimer = _liveEntryBatchTimer;
            _liveEntryBatchTimer = null;
            if (batchTimer != null)
            {
                batchTimer.Change(System.Threading.Timeout.Infinite,
                    System.Threading.Timeout.Infinite);
                batchTimer.Dispose();
            }

            lock (_pendingLiveEntriesRoot)
                _pendingLiveEntries.Clear();

            if (_liveSource != null) _liveSource.DataSource = null;
            if (_historySource != null) _historySource.DataSource = null;
            _liveItems?.Clear();
            _historyItems?.Clear();
            ViewModel.Dispose();
        }

        private void ViewModelOnStorageError(object sender, LogStorageErrorEventArgs e)
        {
            RunOnUiThread(() =>
            {
                ShowTransientStatus("写入失败: " + e.Exception.Message,
                    Color.FromArgb(220, 38, 38), 6000);
            });
        }

        private void PauseToggleOnCheckedChanged(object sender, EventArgs e)
        {
            if (_pauseToggle.Checked)
            {
                _pauseToggle.Text = "继续";
                _statusLabel.Text = "已暂停";
                _statusLabel.ForeColor = Color.FromArgb(202, 120, 12);
                return;
            }

            _pauseToggle.Text = "暂停";
            _pausedEntryCount = 0;
            _statusLabel.Text = "实时记录中";
            _statusLabel.ForeColor = Color.FromArgb(21, 146, 78);
            RefreshLiveGrid();
        }

        private void RefreshLiveGrid()
        {
            if (_liveSource == null || _liveLevelFilter == null || _liveKeyword == null) return;
            var entries = ViewModel.GetLiveEntries(SelectedLevel(_liveLevelFilter), _liveKeyword.Text);
            _liveItems = new BindingList<LogEntry>(entries.ToList());
            _liveSource.DataSource = _liveItems;
            UpdateLiveCount();
        }

        private async Task LoadHistoryAsync()
        {
            if (_historyFrom.Value.Date > _historyTo.Value.Date)
            {
                MessageBox.Show(this, "开始时间不能晚于结束时间。", "时间范围",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _historySearchButton.Enabled = false;
            _historySearchButton.Text = "查询中";
            _historyCountLabel.Text = "正在读取...";
            try
            {
                var entries = await ViewModel.GetHistoryAsync(
                    _historyFrom.Value.Date,
                    _historyTo.Value.Date.AddDays(1).AddTicks(-1),
                    SelectedLevel(_historyLevelFilter),
                    _historyKeyword.Text);
                _historyItems = new BindingList<LogEntry>(entries.ToList());
                _historySource.DataSource = _historyItems;
                _historyCountLabel.Text = entries.Count >= MaximumHistoryEntries
                    ? $"最新 {entries.Count} 条（已达上限）"
                    : $"共 {entries.Count} 条";
                _historyLoaded = true;
            }
            catch (Exception ex)
            {
                _historyCountLabel.Text = "读取失败";
                MessageBox.Show(this, ex.Message, "无法读取历史日志",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _historySearchButton.Enabled = true;
                _historySearchButton.Text = "查询";
            }
        }

        private async Task SaveSettingsAsync()
        {
            if (!ValidateChildren(ValidationConstraints.Enabled)) return;
            _saveSettingsButton.Enabled = false;
            _saveSettingsButton.Text = "保存中";
            try
            {
                var path = _pathTextBox.Text.Trim();
                var days = decimal.ToInt32(_retentionDays.Value);
                await Task.Run(() => ViewModel.SaveSettings(path, days));
                _pathTextBox.Text = ViewModel.LogDirectory;
                _historyLoaded = false;
                ShowTransientStatus("设置已保存", Color.FromArgb(21, 146, 78));
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "无法保存日志设置",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _saveSettingsButton.Enabled = true;
                _saveSettingsButton.Text = "保存设置";
            }
        }

        private void BrowseButtonOnClick(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog
            {
                Description = "选择日志保存目录",
                SelectedPath = _pathTextBox.Text,
                ShowNewFolderButton = true
            })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                    _pathTextBox.Text = dialog.SelectedPath;
            }
        }

        private void OpenFolderButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var directory = ViewModel.LogDirectory;
                System.IO.Directory.CreateDirectory(directory);
                Process.Start("explorer.exe", "\"" + directory + "\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "无法打开日志目录",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PathTextBoxOnValidating(object sender, CancelEventArgs e)
        {
            var invalid = string.IsNullOrWhiteSpace(_pathTextBox.Text);
            _settingsErrors.SetError(_pathTextBox, invalid ? "请输入日志保存路径。" : string.Empty);
            e.Cancel = invalid;
        }

        private void UpdateLiveCount()
        {
            _liveCountLabel.Text = $"共 {_liveItems.Count} 条";
        }

        private void ShowTransientStatus(string text, Color color, int duration = 3200)
        {
            _statusResetTimer.Stop();
            _statusLabel.Text = text;
            _statusLabel.ForeColor = color;
            _statusResetTimer.Interval = duration;
            _statusResetTimer.Start();
        }

        private void RestoreDefaultStatus()
        {
            if (_pauseToggle != null && _pauseToggle.Checked)
            {
                _statusLabel.Text = _pausedEntryCount > 0
                    ? $"已暂停 · {_pausedEntryCount} 条待显示"
                    : "已暂停";
                _statusLabel.ForeColor = Color.FromArgb(202, 120, 12);
                return;
            }

            _statusLabel.Text = "实时记录中";
            _statusLabel.ForeColor = Color.FromArgb(21, 146, 78);
        }

        private void RunOnUiThread(Action action)
        {
            if (IsDisposed || Disposing) return;
            if (InvokeRequired)
            {
                try { BeginInvoke(action); }
                catch (InvalidOperationException) { }
                return;
            }
            action();
        }

        private static bool Matches(LogEntry entry, LogLevel? level, string keyword)
        {
            if (level.HasValue && entry.Level != level.Value) return false;
            if (string.IsNullOrWhiteSpace(keyword)) return true;
            var term = keyword.Trim();
            return entry.Message.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0 ||
                   entry.Source.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static LogLevel? SelectedLevel(ComboBox comboBox)
        {
            return (comboBox.SelectedItem as LevelFilterItem)?.Level;
        }

        private static Panel CreatePageSurface()
        {
            return new RoundedSurfacePanel(10, Color.FromArgb(231, 235, 241))
            {
                Dock = DockStyle.Fill,
                BackColor = SurfaceColor,
                Margin = Padding.Empty
            };
        }

        private static ComboBox CreateLevelFilter()
        {
            var combo = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                BackColor = InputColor,
                ForeColor = TextColor,
                Font = new Font("Segoe UI", 9.5F),
                IntegralHeight = false,
                DropDownHeight = 180
            };
            combo.Items.Add(new LevelFilterItem("全部", null));
            combo.Items.Add(new LevelFilterItem("调试", LogLevel.Debug));
            combo.Items.Add(new LevelFilterItem("信息", LogLevel.Info));
            combo.Items.Add(new LevelFilterItem("警告", LogLevel.Warning));
            combo.Items.Add(new LevelFilterItem("错误", LogLevel.Error));
            combo.Items.Add(new LevelFilterItem("严重", LogLevel.Critical));
            combo.SelectedIndex = 0;
            return combo;
        }

        private static TextBox CreateTextBox()
        {
            var textBox = new TextBox
            {
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = TextColor,
                BackColor = InputColor
            };
            SetTextBoxMargins(textBox, 8, 6);
            return textBox;
        }

        private static void SetCueBanner(TextBox textBox, string cueText)
        {
            void ApplyCueBanner()
            {
                if (textBox.IsHandleCreated)
                    SendMessage(textBox.Handle, 0x1501, IntPtr.Zero, cueText);
            }

            textBox.HandleCreated += (sender, args) => ApplyCueBanner();
            ApplyCueBanner();
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        private static void SetTextBoxMargins(TextBox textBox, int left, int right)
        {
            void ApplyMargins()
            {
                if (!textBox.IsHandleCreated) return;
                var margins = new IntPtr((right << 16) | (left & 0xFFFF));
                SendMessageValue(textBox.Handle, 0x00D3, new IntPtr(3), margins);
            }

            textBox.HandleCreated += (sender, args) => ApplyMargins();
            ApplyMargins();
        }

        [DllImport("user32.dll", EntryPoint = "SendMessageW")]
        private static extern IntPtr SendMessageValue(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private static DateTimePicker CreateDateTimePicker(DateTime value)
        {
            return new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd",
                Value = value,
                Font = new Font("Segoe UI", 9F),
                CalendarForeColor = TextColor,
                CalendarMonthBackground = SurfaceColor
            };
        }

        private static Button CreateButton(
            string text,
            bool primary,
            int width,
            string iconGlyph = null,
            bool danger = false)
        {
            var button = new ModernButton(text, iconGlyph, primary, danger)
            {
                Height = 26,
                Margin = new Padding(0, 0, 12, 0)
            };
            button.Width = Math.Max(width, button.GetPreferredSize(Size.Empty).Width);
            return button;
        }

        private static CheckBox CreateToggleButton(string text)
        {
            return new ModernToggleButton
            {
                Text = text,
                Width = 96,
                Height = 26,
                Margin = Padding.Empty
            };
        }

        private static Label CreateMutedLabel(string text)
        {
            return new Label
            {
                AutoSize = false,
                Text = text,
                ForeColor = MutedColor,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private static DataGridView CreateLogGrid()
        {
            var grid = new BufferedDataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = SurfaceColor,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = true,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders,
                AutoGenerateColumns = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 52,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                RowTemplate = { Height = 56, MinimumHeight = 56 },
                GridColor = Color.FromArgb(237, 240, 244),
                ShowCellToolTips = true,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            };
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(247, 249, 252),
                ForeColor = TextColor,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                SelectionBackColor = Color.FromArgb(247, 249, 252),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(14, 0, 0, 0)
            };
            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = SurfaceColor,
                ForeColor = TextColor,
                SelectionBackColor = Color.FromArgb(232, 240, 254),
                SelectionForeColor = TextColor,
                Font = new Font("Segoe UI", 9F),
                Padding = new Padding(14, 10, 8, 8),
                Alignment = DataGridViewContentAlignment.TopLeft
            };
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 253);

            var timeColumn = new DataGridViewTextBoxColumn
            {
                Name = "TimeColumn",
                HeaderText = "时间",
                DataPropertyName = nameof(LogEntry.DisplayTime),
                Width = 250,
                MinimumWidth = 120,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Consolas", 9F),
                    Padding = new Padding(14, 12, 8, 8),
                    Alignment = DataGridViewContentAlignment.TopLeft
                }
            };
            var levelColumn = new DataGridViewTextBoxColumn
            {
                Name = "LevelColumn",
                HeaderText = "等级",
                DataPropertyName = nameof(LogEntry.LevelText),
                Width = 122,
                MinimumWidth = 84,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            var messageColumn = new DataGridViewTextBoxColumn
            {
                Name = "MessageColumn",
                HeaderText = "内容",
                DataPropertyName = nameof(LogEntry.Message),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                MinimumWidth = 130,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.TopLeft,
                    Padding = new Padding(14, 10, 8, 8)
                }
            };
            grid.Columns.AddRange(timeColumn, levelColumn, messageColumn);
            grid.Resize += (sender, args) =>
            {
                var width = grid.ClientSize.Width;
                timeColumn.Width = width < 580 ? 150 : width < 860 ? 190 : 250;
                levelColumn.Width = width < 580 ? 92 : width < 860 ? 108 : 122;
            };
            grid.CellPainting += LogGridOnCellPainting;
            grid.CellDoubleClick += LogGridOnCellDoubleClick;
            grid.CellToolTipTextNeeded += (sender, args) =>
            {
                if (args.RowIndex < 0) return;
                var entry = ((DataGridView)sender).Rows[args.RowIndex].DataBoundItem as LogEntry;
                if (entry != null) args.ToolTipText = entry.Message;
            };
            return grid;
        }

        private static void LogGridOnCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            var grid = (DataGridView)sender;
            if (e.RowIndex < 0 || grid.Columns[e.ColumnIndex].DataPropertyName != nameof(LogEntry.LevelText)) return;
            var entry = grid.Rows[e.RowIndex].DataBoundItem as LogEntry;
            if (entry == null) return;

            Color foreground;
            Color background;
            switch (entry.Level)
            {
                case LogLevel.Debug:
                    foreground = Color.FromArgb(71, 85, 105);
                    background = Color.FromArgb(241, 245, 249);
                    break;
                case LogLevel.Info:
                    foreground = Color.FromArgb(29, 78, 216);
                    background = Color.FromArgb(239, 246, 255);
                    break;
                case LogLevel.Warning:
                    foreground = Color.FromArgb(161, 81, 0);
                    background = Color.FromArgb(255, 247, 225);
                    break;
                case LogLevel.Error:
                    foreground = Color.FromArgb(190, 24, 24);
                    background = Color.FromArgb(254, 242, 242);
                    break;
                default:
                    foreground = Color.FromArgb(127, 29, 29);
                    background = Color.FromArgb(254, 226, 226);
                    break;
            }

            var selected = (e.State & DataGridViewElementStates.Selected) != 0;
            var cellBackground = selected
                ? grid.DefaultCellStyle.SelectionBackColor
                : e.CellStyle.BackColor;
            using (var cellBrush = new SolidBrush(cellBackground))
                e.Graphics.FillRectangle(cellBrush, e.CellBounds);
            using (var borderPen = new Pen(grid.GridColor))
                e.Graphics.DrawLine(borderPen, e.CellBounds.Left, e.CellBounds.Bottom - 1,
                    e.CellBounds.Right, e.CellBounds.Bottom - 1);
            const int badgeHeight = 28;
            var label = LevelSymbol(entry.Level) + "  " + Convert.ToString(e.FormattedValue);
            var measured = TextRenderer.MeasureText(label, e.CellStyle.Font,
                new Size(int.MaxValue, badgeHeight), TextFormatFlags.NoPadding);
            var badgeWidth = Math.Min(e.CellBounds.Width - 16, Math.Max(62, measured.Width + 20));
            var badge = new Rectangle(e.CellBounds.Left + (e.CellBounds.Width - badgeWidth) / 2,
                e.CellBounds.Top + 12,
                badgeWidth, badgeHeight);
            using (var path = CreateRoundedRectangle(badge, 8))
            using (var brush = new SolidBrush(background))
                e.Graphics.FillPath(brush, path);
            TextRenderer.DrawText(e.Graphics, label, e.CellStyle.Font,
                badge, foreground,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            e.Handled = true;
        }

        private static string LevelSymbol(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug: return "#";
                case LogLevel.Info: return "#";
                case LogLevel.Warning: return "#";
                case LogLevel.Error: return "#";
                default: return "#";
            }
        }

        private static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            var diameter = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.Left, bounds.Top, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Top, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void DrawButtonContent(
            Graphics graphics,
            Rectangle bounds,
            string text,
            string iconGlyph,
            Font textFont,
            Color color)
        {
            var textSize = TextRenderer.MeasureText(text ?? string.Empty, textFont,
                Size.Empty, TextFormatFlags.NoPadding);
            var iconWidth = string.IsNullOrEmpty(iconGlyph) ? 0 : 18;
            var gap = iconWidth == 0 ? 0 : 5;
            var contentWidth = iconWidth + gap + textSize.Width;
            var x = bounds.Left + Math.Max(0, (bounds.Width - contentWidth) / 2);
            if (iconWidth > 0)
            {
                TextRenderer.DrawText(graphics, iconGlyph, IconFont,
                    new Rectangle(x, bounds.Top, iconWidth, bounds.Height), color,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                x += iconWidth + gap;
            }
            TextRenderer.DrawText(graphics, text ?? string.Empty, textFont,
                new Rectangle(x, bounds.Top, Math.Max(1, Math.Min(bounds.Right - x, textSize.Width + 6)), bounds.Height), color,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
        }

        private static void LogGridOnCellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var grid = (DataGridView)sender;
            var entry = grid.Rows[e.RowIndex].DataBoundItem as LogEntry;
            if (entry == null) return;

            using (var detail = new Form
            {
                Text = "日志详情",
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(680, 420),
                MinimumSize = new Size(460, 300),
                Font = new Font("Segoe UI", 9F),
                BackColor = SurfaceColor
            })
            {
                var text = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Multiline = true,
                    ReadOnly = true,
                    WordWrap = true,
                    ScrollBars = ScrollBars.Vertical,
                    BorderStyle = BorderStyle.None,
                    BackColor = SurfaceColor,
                    ForeColor = TextColor,
                    Font = new Font("Consolas", 10F),
                    Text = $"时间: {entry.DisplayTime}{Environment.NewLine}" +
                           $"等级: {entry.LevelText}{Environment.NewLine}" +
                           $"来源: {entry.Source}{Environment.NewLine}{Environment.NewLine}" + entry.Message
                };
                text.Text = NormalizeLineEndings(text.Text);
                var host = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20), BackColor = SurfaceColor };
                host.Controls.Add(text);
                detail.Controls.Add(host);
                detail.ShowDialog(grid.FindForm());
            }
        }

        private static string NormalizeLineEndings(string value)
        {
            return (value ?? string.Empty)
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", Environment.NewLine);
        }

        private sealed class LabeledField : Panel
        {
            private readonly Label _label;
            private readonly bool _compact;

            public LabeledField(string labelText, Control input, bool compact = false)
            {
                BackColor = SurfaceColor;
                Margin = Padding.Empty;
                Input = input;
                _compact = compact;
                input.Dock = DockStyle.None;
                input.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                input.Margin = Padding.Empty;
                _label = new Label
                {
                    AutoSize = false,
                    Text = labelText ?? string.Empty,
                    Font = new Font("Segoe UI", 8.5F),
                    ForeColor = MutedColor,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Location = new Point(0, 0),
                    Height = 20,
                    Visible = !compact
                };
                Controls.Add(_label);
                Controls.Add(input);
            }

            public Control Input { get; }

            protected override void OnLayout(LayoutEventArgs levent)
            {
                base.OnLayout(levent);
                if (Controls.Count < 2) return;
                _label.Width = ClientSize.Width;
                var availableTop = _compact ? 0 : 22;
                var availableHeight = Math.Max(1, ClientSize.Height - availableTop);
                var preferredHeight = Math.Min(availableHeight, Math.Max(28, Input.PreferredSize.Height));
                var inputY = availableTop + Math.Max(0, (availableHeight - preferredHeight) / 2);
                Input.SetBounds(0, inputY, ClientSize.Width, preferredHeight);
            }
        }

        private sealed class RoundedSurfacePanel : Panel
        {
            private readonly int _radius;
            private readonly Color _borderColor;

            public RoundedSurfacePanel(int radius, Color borderColor)
            {
                _radius = radius;
                _borderColor = borderColor;
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            }

            protected override void OnResize(EventArgs eventargs)
            {
                base.OnResize(eventargs);
                if (Width <= 1 || Height <= 1) return;
                using (var path = CreateRoundedRectangle(new Rectangle(0, 0, Width, Height), _radius))
                {
                    var oldRegion = Region;
                    Region = new Region(path);
                    oldRegion?.Dispose();
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var bounds = ClientRectangle;
                bounds.Width--;
                bounds.Height--;
                using (var path = CreateRoundedRectangle(bounds, _radius))
                using (var pen = new Pen(_borderColor))
                    e.Graphics.DrawPath(pen, path);
            }
        }

        private sealed class StatusIndicator : Label
        {
            public StatusIndicator()
            {
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var availableWidth = Math.Max(1, Width - 26);
                var textSize = TextRenderer.MeasureText(Text ?? string.Empty, Font,
                    new Size(availableWidth, Height), TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis);
                var textWidth = Math.Min(availableWidth, textSize.Width + 12);
                var textX = Math.Max(22, Width - textWidth - 2);
                var dotX = Math.Max(2, textX - 20);
                var centerY = Height / 2;
                using (var halo = new SolidBrush(Color.FromArgb(28, ForeColor)))
                    e.Graphics.FillEllipse(halo, dotX - 4, centerY - 8, 16, 16);
                using (var dot = new SolidBrush(ForeColor))
                    e.Graphics.FillEllipse(dot, dotX, centerY - 4, 8, 8);
                TextRenderer.DrawText(e.Graphics, Text ?? string.Empty, Font,
                    new Rectangle(textX, 0, Math.Max(1, Width - textX - 2), Height), ForeColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }
        }

        private sealed class ModernButton : Button
        {
            private readonly string _iconGlyph;
            private readonly bool _primary;
            private readonly bool _danger;
            private bool _hovered;
            private bool _pressed;

            public ModernButton(string text, string iconGlyph, bool primary, bool danger)
            {
                Text = text;
                _iconGlyph = iconGlyph;
                _primary = primary;
                _danger = danger;
                Font = new Font("Segoe UI", 9.5F);
                Cursor = Cursors.Hand;
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                TabStop = true;
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            }

            protected override void OnMouseEnter(EventArgs e) { _hovered = true; Invalidate(); base.OnMouseEnter(e); }
            protected override void OnMouseLeave(EventArgs e) { _hovered = false; _pressed = false; Invalidate(); base.OnMouseLeave(e); }
            protected override void OnMouseDown(MouseEventArgs mevent) { _pressed = true; Invalidate(); base.OnMouseDown(mevent); }
            protected override void OnMouseUp(MouseEventArgs mevent) { _pressed = false; Invalidate(); base.OnMouseUp(mevent); }
            protected override void OnEnabledChanged(EventArgs e) { base.OnEnabledChanged(e); Invalidate(); }

            public override Size GetPreferredSize(Size proposedSize)
            {
                var textSize = TextRenderer.MeasureText(Text ?? string.Empty, Font,
                    Size.Empty, TextFormatFlags.NoPadding);
                var iconWidth = string.IsNullOrEmpty(_iconGlyph) ? 0 : 23;
                return new Size(textSize.Width + iconWidth + 28, Math.Max(26, textSize.Height + 8));
            }

            protected override void OnPaint(PaintEventArgs pevent)
            {
                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var foreground = _primary ? Color.White : _danger ? Color.FromArgb(235, 60, 70) : TextColor;
                var background = _primary
                    ? (_pressed ? Color.FromArgb(24, 68, 190) : _hovered ? Color.FromArgb(28, 78, 216) : PrimaryColor)
                    : _danger
                        ? (_hovered ? Color.FromArgb(255, 244, 245) : SurfaceColor)
                        : (_pressed ? Color.FromArgb(231, 235, 241) : _hovered ? Color.FromArgb(241, 244, 248) : Color.FromArgb(247, 249, 252));
                var border = _primary ? PrimaryColor : _danger ? Color.FromArgb(255, 137, 145) : BorderColor;
                if (!Enabled)
                {
                    foreground = Color.FromArgb(160, 169, 181);
                    background = Color.FromArgb(244, 246, 249);
                    border = Color.FromArgb(229, 233, 239);
                }

                var bounds = ClientRectangle;
                bounds.Width--;
                bounds.Height--;
                using (var brush = new SolidBrush(background))
                using (var pen = new Pen(border))
                {
                    pevent.Graphics.FillRectangle(brush, bounds);
                    pevent.Graphics.DrawRectangle(pen, bounds);
                }
                DrawButtonContent(pevent.Graphics, bounds, Text, _iconGlyph, Font, foreground);
                if (Focused && ShowFocusCues)
                {
                    var focus = bounds;
                    focus.Inflate(-4, -4);
                    ControlPaint.DrawFocusRectangle(pevent.Graphics, focus);
                }
            }
        }

        private sealed class ModernToggleButton : CheckBox
        {
            private bool _hovered;
            private bool _pressed;

            public ModernToggleButton()
            {
                Appearance = Appearance.Button;
                Font = new Font("Segoe UI", 9.5F);
                Cursor = Cursors.Hand;
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                TextAlign = ContentAlignment.MiddleCenter;
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            }

            protected override void OnMouseEnter(EventArgs e) { _hovered = true; Invalidate(); base.OnMouseEnter(e); }
            protected override void OnMouseLeave(EventArgs e) { _hovered = false; _pressed = false; Invalidate(); base.OnMouseLeave(e); }
            protected override void OnMouseDown(MouseEventArgs e) { _pressed = true; Invalidate(); base.OnMouseDown(e); }
            protected override void OnMouseUp(MouseEventArgs e) { _pressed = false; Invalidate(); base.OnMouseUp(e); }
            protected override void OnCheckedChanged(EventArgs e) { base.OnCheckedChanged(e); Invalidate(); }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var background = Checked
                    ? Color.FromArgb(255, 248, 230)
                    : _pressed ? Color.FromArgb(231, 235, 241)
                    : _hovered ? Color.FromArgb(241, 244, 248)
                    : Color.FromArgb(247, 249, 252);
                var foreground = Checked ? Color.FromArgb(180, 105, 8) : Color.FromArgb(71, 85, 105);
                var bounds = ClientRectangle;
                bounds.Width--;
                bounds.Height--;
                using (var brush = new SolidBrush(background))
                using (var pen = new Pen(BorderColor))
                {
                    e.Graphics.FillRectangle(brush, bounds);
                    e.Graphics.DrawRectangle(pen, bounds);
                }
                DrawButtonContent(e.Graphics, bounds, Text, Checked ? "\uE768" : "\uE769", Font, foreground);
            }
        }

        private sealed class LiveToolbar : Panel
        {
            private readonly Control _level;
            private readonly Control _keyword;
            private readonly Control _pause;
            private readonly Control _clear;
            private readonly Control _count;

            public LiveToolbar(Control level, Control keyword, Control pause, Control clear, Control count)
            {
                _level = level;
                _keyword = keyword;
                _pause = pause;
                _clear = clear;
                _count = count;
                BackColor = SurfaceColor;
                Controls.AddRange(new[] { level, keyword, pause, clear, count });
                Height = 88;
            }

            protected override void OnLayout(LayoutEventArgs levent)
            {
                base.OnLayout(levent);
                var width = ClientSize.Width;
                const int pad = 24;
                const int gap = 12;
                if (width >= 820)
                {
                    SetHeight(88);
                    var actionWidth = 96 + gap + 96 + gap + 82;
                    var keywordX = pad + 180 + 16;
                    var keywordWidth = Math.Max(200, width - pad * 2 - 180 - 16 - actionWidth - 16);
                    _level.SetBounds(pad, 22, 180, 42);
                    _keyword.SetBounds(keywordX, 22, keywordWidth, 42);
                    var actionX = width - pad - actionWidth;
                    _pause.SetBounds(actionX, 29, 96, 26);
                    _clear.SetBounds(actionX + 96 + gap, 29, 96, 26);
                    _count.SetBounds(actionX + 216, 28, 82, 28);
                }
                else if (width >= 560)
                {
                    SetHeight(148);
                    _level.SetBounds(pad, 18, 160, 42);
                    _keyword.SetBounds(pad + 176, 18, Math.Max(180, width - pad * 2 - 176), 42);
                    _pause.SetBounds(pad, 94, 96, 26);
                    _clear.SetBounds(pad + 108, 94, 96, 26);
                    _count.SetBounds(pad + 216, 93, 110, 28);
                }
                else
                {
                    SetHeight(202);
                    _level.SetBounds(pad, 16, Math.Max(140, width - pad * 2), 42);
                    _keyword.SetBounds(pad, 70, Math.Max(140, width - pad * 2), 42);
                    _pause.SetBounds(pad, 144, 96, 26);
                    _clear.SetBounds(pad + 108, 144, 96, 26);
                    _count.SetBounds(pad + 216, 143, Math.Max(76, width - pad * 2 - 216), 28);
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                using (var pen = new Pen(BorderColor))
                    e.Graphics.DrawLine(pen, 0, Height - 1, Width, Height - 1);
            }

            private void SetHeight(int height)
            {
                if (Height != height) Height = height;
            }
        }

        private sealed class HistoryToolbar : Panel
        {
            private readonly Control _from;
            private readonly Control _to;
            private readonly Control _level;
            private readonly Control _keyword;
            private readonly Control _search;
            private readonly Control _count;

            public HistoryToolbar(Control from, Control to, Control level, Control keyword, Control search, Control count)
            {
                _from = from;
                _to = to;
                _level = level;
                _keyword = keyword;
                _search = search;
                _count = count;
                BackColor = SurfaceColor;
                Controls.AddRange(new[] { from, to, level, keyword, search, count });
                Height = 90;
            }

            protected override void OnLayout(LayoutEventArgs levent)
            {
                base.OnLayout(levent);
                var width = ClientSize.Width;
                const int pad = 24;
                const int gap = 12;
                if (width >= 900)
                {
                    SetHeight(90);
                    _from.SetBounds(pad, 16, 142, 58);
                    _to.SetBounds(pad + 154, 16, 142, 58);
                    _level.SetBounds(pad + 308, 16, 124, 58);
                    var actionWidth = 94 + gap + 92;
                    var keywordX = pad + 444;
                    var keywordWidth = Math.Max(160, width - keywordX - pad - actionWidth - gap);
                    _keyword.SetBounds(keywordX, 16, keywordWidth, 58);
                    var actionX = width - pad - actionWidth;
                    _search.SetBounds(actionX, 42, 94, 26);
                    _count.SetBounds(actionX + 106, 41, 92, 28);
                }
                else if (width >= 650)
                {
                    SetHeight(158);
                    _from.SetBounds(pad, 14, 142, 58);
                    _to.SetBounds(pad + 154, 14, 142, 58);
                    _level.SetBounds(pad + 308, 14, Math.Max(124, width - pad * 2 - 308), 58);
                    _keyword.SetBounds(pad, 84, Math.Max(180, width - pad * 2 - 212), 58);
                    _search.SetBounds(width - pad - 198, 110, 94, 26);
                    _count.SetBounds(width - pad - 92, 109, 92, 28);
                }
                else
                {
                    SetHeight(222);
                    var half = Math.Max(120, (width - pad * 2 - gap) / 2);
                    _from.SetBounds(pad, 12, half, 58);
                    _to.SetBounds(pad + half + gap, 12, half, 58);
                    _level.SetBounds(pad, 82, 120, 58);
                    _keyword.SetBounds(pad + 132, 82, Math.Max(120, width - pad * 2 - 132), 58);
                    _search.SetBounds(pad, 168, 94, 26);
                    _count.SetBounds(pad + 106, 167, Math.Max(88, width - pad * 2 - 106), 28);
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                using (var pen = new Pen(BorderColor))
                    e.Graphics.DrawLine(pen, 0, Height - 1, Width, Height - 1);
            }

            private void SetHeight(int height)
            {
                if (Height != height) Height = height;
            }
        }

        private sealed class NavigationButton : Button
        {
            private bool _active;
            private bool _hovered;
            private bool _pressed;

            public NavigationButton(string text)
            {
                Text = text;
                Dock = DockStyle.Fill;
                Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                ForeColor = MutedColor;
                BackColor = SurfaceColor;
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                Cursor = Cursors.Hand;
                Margin = Padding.Empty;
                TabStop = true;
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer, true);
            }

            public bool Active
            {
                get => _active;
                set
                {
                    if (_active == value) return;
                    _active = value;
                    Invalidate();
                }
            }

            protected override void OnMouseEnter(EventArgs e) { _hovered = true; Invalidate(); base.OnMouseEnter(e); }
            protected override void OnMouseLeave(EventArgs e) { _hovered = false; _pressed = false; Invalidate(); base.OnMouseLeave(e); }
            protected override void OnMouseDown(MouseEventArgs mevent) { _pressed = true; Invalidate(); base.OnMouseDown(mevent); }
            protected override void OnMouseUp(MouseEventArgs mevent) { _pressed = false; Invalidate(); base.OnMouseUp(mevent); }

            protected override void OnPaint(PaintEventArgs pevent)
            {
                var background = _pressed
                    ? Color.FromArgb(235, 240, 247)
                    : _hovered ? Color.FromArgb(247, 249, 252) : SurfaceColor;
                pevent.Graphics.Clear(background);
                using (var divider = new Pen(BorderColor))
                    pevent.Graphics.DrawLine(divider, 0, Height - 1, Width, Height - 1);
                TextRenderer.DrawText(pevent.Graphics, Text, Font, ClientRectangle,
                    Active ? PrimaryColor : MutedColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                if (Active)
                {
                    const int indicatorWidth = 64;
                    using (var brush = new SolidBrush(PrimaryColor))
                        pevent.Graphics.FillRectangle(brush,
                            Math.Max(0, (Width - indicatorWidth) / 2), Height - 3,
                            Math.Min(indicatorWidth, Width), 3);
                }
                if (Focused && ShowFocusCues)
                {
                    var focus = ClientRectangle;
                    focus.Inflate(-5, -5);
                    ControlPaint.DrawFocusRectangle(pevent.Graphics, focus);
                }
            }
        }

        private sealed class LevelFilterItem
        {
            public LevelFilterItem(string text, LogLevel? level)
            {
                Text = text;
                Level = level;
            }
            public string Text { get; }
            public LogLevel? Level { get; }
            public override string ToString() => Text;
        }

        private sealed class BufferedDataGridView : DataGridView
        {
            public BufferedDataGridView()
            {
                DoubleBuffered = true;
            }
        }
    }
}
