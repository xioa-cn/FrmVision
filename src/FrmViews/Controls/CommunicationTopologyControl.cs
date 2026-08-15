using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FrmServices.ViewModel;

namespace FrmViews.Controls
{
    internal sealed class CommunicationTopologyControl : UserControl
    {
        private readonly CommunicationLaneControl _plcLane;
        private readonly CommunicationLaneControl _cameraLane;
        private readonly CommunicationLaneControl _lightLane;

        public CommunicationTopologyControl()
        {
            AutoScroll = false;
            BackColor = Color.FromArgb(211, 218, 225);
            DoubleBuffered = true;
            MinimumSize = new Size(540, 100);

            var laneLayout = new TableLayoutPanel
            {
                BackColor = Color.FromArgb(211, 218, 225),
                ColumnCount = 3,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                RowCount = 1
            };
            laneLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            laneLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.334F));
            laneLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            laneLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            _plcLane = CreateLane(CommunicationDeviceType.Plc, "PLC",
                Color.FromArgb(0, 132, 137), "PLC");
            _cameraLane = CreateLane(CommunicationDeviceType.Camera, "相机",
                Color.FromArgb(36, 99, 235), "CAM");
            _lightLane = CreateLane(CommunicationDeviceType.LightSource, "光源",
                Color.FromArgb(202, 120, 12), "LGT");

            _plcLane.Margin = new Padding(0, 0, 1, 0);
            _cameraLane.Margin = new Padding(0, 0, 1, 0);
            _lightLane.Margin = Padding.Empty;
            laneLayout.Controls.Add(_plcLane, 0, 0);
            laneLayout.Controls.Add(_cameraLane, 1, 0);
            laneLayout.Controls.Add(_lightLane, 2, 0);
            Controls.Add(laneLayout);
        }

        public event EventHandler<CommunicationDeviceSelectedEventArgs> DeviceSelected;

        public void Bind(BindingList<DeviceConnectionConfiguration> configurations)
        {
            _plcLane.Bind(configurations);
            _cameraLane.Bind(configurations);
            _lightLane.Bind(configurations);
        }

        public void SelectConfiguration(DeviceConnectionConfiguration configuration)
        {
            _plcLane.SelectConfiguration(configuration);
            _cameraLane.SelectConfiguration(configuration);
            _lightLane.SelectConfiguration(configuration);
        }

        private CommunicationLaneControl CreateLane(CommunicationDeviceType deviceType,
            string title, Color accent, string badge)
        {
            var lane = new CommunicationLaneControl(deviceType, title, accent, badge)
            {
                Dock = DockStyle.Fill
            };
            lane.DeviceSelected += LaneOnDeviceSelected;
            return lane;
        }

        private void LaneOnDeviceSelected(object sender,
            CommunicationDeviceSelectedEventArgs e)
        {
            SelectConfiguration(e.Configuration);
            DeviceSelected?.Invoke(this, e);
        }
    }

    internal sealed class CommunicationLaneControl : UserControl
    {
        private const int HeaderHeight = 54;
        private const int NodeTop = 68;
        private const int NodeHeight = 70;
        private const int NodeGap = 16;
        private const int ContentBottomPadding = 18;

        private readonly CommunicationDeviceType _deviceType;
        private readonly string _title;
        private readonly Color _accent;
        private readonly string _badge;
        private readonly VScrollBar _scrollBar;
        private readonly Dictionary<Rectangle, DeviceConnectionConfiguration> _nodeBounds =
            new Dictionary<Rectangle, DeviceConnectionConfiguration>();
        private readonly Font _headerFont =
            new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
        private readonly Font _badgeFont =
            new Font("Segoe UI", 7.5F, FontStyle.Bold);
        private readonly Font _nameFont =
            new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
        private readonly Font _endpointFont = new Font("Microsoft YaHei UI", 7.5F);
        private readonly Font _statusFont = new Font("Microsoft YaHei UI", 7.5F);

        private BindingList<DeviceConnectionConfiguration> _configurations;
        private DeviceConnectionConfiguration _selectedConfiguration;

        public CommunicationLaneControl(CommunicationDeviceType deviceType,
            string title, Color accent, string badge)
        {
            _deviceType = deviceType;
            _title = title;
            _accent = accent;
            _badge = badge;

            AutoScroll = false;
            BackColor = Color.FromArgb(247, 249, 251);
            DoubleBuffered = true;
            ResizeRedraw = true;
            TabStop = true;
            _scrollBar = new VScrollBar
            {
                Dock = DockStyle.Right,
                SmallChange = NodeHeight + NodeGap,
                Visible = false
            };
            _scrollBar.ValueChanged += ScrollBarOnValueChanged;
            Controls.Add(_scrollBar);
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        public event EventHandler<CommunicationDeviceSelectedEventArgs> DeviceSelected;

        public void Bind(BindingList<DeviceConnectionConfiguration> configurations)
        {
            if (ReferenceEquals(_configurations, configurations)) return;
            if (_configurations != null)
                _configurations.ListChanged -= ConfigurationsOnListChanged;

            _configurations = configurations;
            if (_configurations != null)
                _configurations.ListChanged += ConfigurationsOnListChanged;
            UpdateScrollExtent();
        }

        public void SelectConfiguration(DeviceConnectionConfiguration configuration)
        {
            var selection = configuration != null &&
                            configuration.DeviceType == _deviceType
                ? configuration
                : null;
            if (ReferenceEquals(_selectedConfiguration, selection)) return;

            _selectedConfiguration = selection;
            EnsureConfigurationVisible(selection);
            Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_configurations != null)
                    _configurations.ListChanged -= ConfigurationsOnListChanged;
                _headerFont.Dispose();
                _badgeFont.Dispose();
                _nameFont.Dispose();
                _endpointFont.Dispose();
                _statusFont.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Cursor = HitTest(e.Location) == null ? Cursors.Default : Cursors.Hand;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            var configuration = HitTest(e.Location);
            if (configuration == null) return;

            SelectConfiguration(configuration);
            DeviceSelected?.Invoke(this,
                new CommunicationDeviceSelectedEventArgs(configuration));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(BackColor);
            e.Graphics.SmoothingMode =
                System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var state = e.Graphics.Save();
            e.Graphics.SetClip(new Rectangle(0, HeaderHeight,
                GetContentWidth(), Math.Max(0, ClientSize.Height - HeaderHeight)));
            var scrollOffset = -_scrollBar.Value;
            DrawGrid(e.Graphics, scrollOffset);
            DrawNodes(e.Graphics, scrollOffset);
            e.Graphics.Restore(state);
            DrawHeader(e.Graphics);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateScrollExtent();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (!_scrollBar.Visible) return;

            var wheelSteps = e.Delta / SystemInformation.MouseWheelScrollDelta;
            SetScrollValue(_scrollBar.Value -
                           wheelSteps * _scrollBar.SmallChange);
        }

        private void DrawHeader(Graphics graphics)
        {
            using (var surface = new SolidBrush(Color.FromArgb(247, 249, 251)))
                graphics.FillRectangle(surface, 0, 0, ClientSize.Width, HeaderHeight);

            var headerBounds = new Rectangle(18, 10, GetContentWidth() - 36, 30);
            TextRenderer.DrawText(graphics, _title + "  " + GetItems().Count,
                _headerFont, headerBounds, Color.FromArgb(24, 32, 45),
                TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            using (var accentPen = new Pen(_accent, 3F))
                graphics.DrawLine(accentPen, 18, HeaderHeight - 5,
                    GetContentWidth() - 18, HeaderHeight - 5);
        }

        private void DrawGrid(Graphics graphics, int scrollOffset)
        {
            using (var brush = new SolidBrush(Color.FromArgb(226, 231, 236)))
            {
                for (var x = 12; x < GetContentWidth(); x += 24)
                for (var contentY = 12; contentY < GetContentHeight(); contentY += 24)
                {
                    var y = contentY + scrollOffset;
                    if (y < HeaderHeight || y >= ClientSize.Height) continue;
                    graphics.FillEllipse(brush, x, y, 2, 2);
                }
            }
        }

        private void DrawNodes(Graphics graphics, int scrollOffset)
        {
            _nodeBounds.Clear();
            var items = GetItems();
            var railX = 18;
            var nodeLeft = 32;
            var nodeWidth = Math.Max(120, GetContentWidth() - 52);

            if (items.Count > 0)
            {
                using (var railPen = new Pen(Color.FromArgb(137, 151, 164), 1.5F))
                    graphics.DrawLine(railPen, railX, NodeTop + scrollOffset, railX,
                        NodeTop + scrollOffset +
                        items.Count * (NodeHeight + NodeGap) - NodeGap);
            }

            for (var index = 0; index < items.Count; index++)
            {
                var y = NodeTop + index * (NodeHeight + NodeGap) + scrollOffset;
                var bounds = new Rectangle(nodeLeft, y, nodeWidth, NodeHeight);
                if (bounds.Bottom < HeaderHeight || bounds.Top > ClientSize.Height)
                    continue;
                using (var connector = new Pen(Color.FromArgb(137, 151, 164), 1.5F))
                    graphics.DrawLine(connector, railX, y + NodeHeight / 2,
                        bounds.Left, y + NodeHeight / 2);
                DrawNode(graphics, bounds, items[index]);
                if (!IsDesignMode)
                    _nodeBounds[bounds] = items[index];
            }
        }

        private void DrawNode(Graphics graphics, Rectangle bounds,
            DeviceConnectionConfiguration configuration)
        {
            var selected = ReferenceEquals(_selectedConfiguration, configuration);
            var enabled = configuration.Enabled;
            var nodeAccent = enabled ? _accent : Color.FromArgb(148, 158, 168);
            using (var shadow = new SolidBrush(Color.FromArgb(18, 31, 42, 52)))
                graphics.FillRectangle(shadow, bounds.X + 3, bounds.Y + 4,
                    bounds.Width, bounds.Height);
            using (var surface = new SolidBrush(enabled
                       ? Color.White
                       : Color.FromArgb(244, 246, 248)))
                graphics.FillRectangle(surface, bounds);
            using (var accentBrush = new SolidBrush(nodeAccent))
                graphics.FillRectangle(accentBrush, bounds.X, bounds.Y, 6, bounds.Height);
            using (var border = new Pen(selected ? nodeAccent :
                       Color.FromArgb(205, 213, 221), selected ? 2F : 1F))
                graphics.DrawRectangle(border, bounds);

            var badgeBounds = new Rectangle(bounds.X + 16, bounds.Y + 15, 42, 42);
            using (var badgeBrush = new SolidBrush(enabled
                       ? Color.FromArgb(239, 244, 246)
                       : Color.FromArgb(232, 235, 238)))
                graphics.FillRectangle(badgeBrush, badgeBounds);
            TextRenderer.DrawText(graphics, _badge, _badgeFont, badgeBounds, nodeAccent,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            var statusBounds = new Rectangle(bounds.Right - 70, bounds.Y + 12, 58, 22);
            var statusColor = enabled
                ? Color.FromArgb(21, 146, 78)
                : Color.FromArgb(100, 112, 128);
            using (var statusSurface = new SolidBrush(enabled
                       ? Color.FromArgb(228, 247, 237)
                       : Color.FromArgb(232, 235, 238)))
                graphics.FillRectangle(statusSurface, statusBounds);
            TextRenderer.DrawText(graphics, enabled ? "启用" : "停用", _statusFont,
                statusBounds, statusColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            var nameBounds = new Rectangle(bounds.X + 70, bounds.Y + 13,
                Math.Max(1, bounds.Width - 154), 26);
            TextRenderer.DrawText(graphics,
                configuration.Name ?? configuration.DeviceTypeText,
                _nameFont, nameBounds, enabled
                    ? Color.FromArgb(24, 32, 45)
                    : Color.FromArgb(100, 112, 128),
                TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter);
            var endpointBounds = new Rectangle(bounds.X + 70, bounds.Y + 40,
                Math.Max(1, bounds.Width - 80), 22);
            TextRenderer.DrawText(graphics, configuration.Endpoint, _endpointFont,
                endpointBounds, enabled
                    ? Color.FromArgb(100, 112, 128)
                    : Color.FromArgb(148, 158, 168),
                TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter);
        }

        private IList<DeviceConnectionConfiguration> GetItems()
        {
            var items = new List<DeviceConnectionConfiguration>();
            if (IsDesignMode)
            {
                items.Add(CreateDesignConfiguration());
                return items;
            }

            if (_configurations == null) return items;
            foreach (var configuration in _configurations)
                if (configuration.DeviceType == _deviceType) items.Add(configuration);
            return items;
        }

        private DeviceConnectionConfiguration CreateDesignConfiguration()
        {
            var configuration = new DeviceConnectionConfiguration
            {
                DeviceType = _deviceType,
                Name = _title + " 1",
                Enabled = true
            };
            if (_deviceType == CommunicationDeviceType.LightSource)
            {
                configuration.ConnectionMode = "串口";
                configuration.SerialPort = "COM3";
                configuration.BaudRate = 9600;
            }
            else
            {
                configuration.ConnectionMode = _deviceType ==
                    CommunicationDeviceType.Camera ? "Ping" : "网口";
                configuration.Host = _deviceType == CommunicationDeviceType.Plc
                    ? "192.168.1.10"
                    : "192.168.1.20";
                configuration.Port = _deviceType == CommunicationDeviceType.Plc ? 102 : 0;
            }

            return configuration;
        }

        private bool IsDesignMode =>
            LicenseManager.UsageMode == LicenseUsageMode.Designtime;

        private DeviceConnectionConfiguration HitTest(Point location)
        {
            if (location.Y < HeaderHeight) return null;
            foreach (var item in _nodeBounds)
                if (item.Key.Contains(location)) return item.Value;
            return null;
        }

        private void ConfigurationsOnListChanged(object sender, ListChangedEventArgs e)
        {
            UpdateScrollExtent();
        }

        private void UpdateScrollExtent()
        {
            var requiredHeight = GetRequiredContentHeight();
            var shouldScroll = requiredHeight > ClientSize.Height;
            _scrollBar.Visible = shouldScroll;
            if (shouldScroll)
            {
                _scrollBar.LargeChange = Math.Max(1, ClientSize.Height);
                _scrollBar.Maximum = Math.Max(0, requiredHeight - 1);
                SetScrollValue(_scrollBar.Value);
            }
            else
            {
                _scrollBar.Value = 0;
            }

            Invalidate();
        }

        private int GetRequiredContentHeight()
        {
            var itemCount = GetItems().Count;
            return NodeTop + itemCount * (NodeHeight + NodeGap) -
                   (itemCount > 0 ? NodeGap : 0) + ContentBottomPadding;
        }

        private int GetContentHeight()
        {
            return Math.Max(ClientSize.Height, GetRequiredContentHeight());
        }

        private int GetContentWidth()
        {
            return Math.Max(1, ClientSize.Width -
                (_scrollBar.Visible ? _scrollBar.Width : 0));
        }

        private void EnsureConfigurationVisible(
            DeviceConnectionConfiguration configuration)
        {
            if (configuration == null) return;
            var items = GetItems();
            var index = -1;
            for (var itemIndex = 0; itemIndex < items.Count; itemIndex++)
            {
                if (!ReferenceEquals(items[itemIndex], configuration)) continue;
                index = itemIndex;
                break;
            }
            if (index < 0) return;

            var nodeTop = NodeTop + index * (NodeHeight + NodeGap);
            var nodeBottom = nodeTop + NodeHeight;
            var viewportTop = _scrollBar.Value + HeaderHeight;
            var viewportBottom = _scrollBar.Value + ClientSize.Height;
            var targetTop = _scrollBar.Value;
            if (nodeTop < viewportTop)
                targetTop = Math.Max(0, nodeTop - HeaderHeight - NodeGap);
            else if (nodeBottom > viewportBottom)
                targetTop = nodeBottom - ClientSize.Height + NodeGap;

            SetScrollValue(targetTop);
        }

        private void ScrollBarOnValueChanged(object sender, EventArgs e)
        {
            Invalidate();
            Update();
        }

        private void SetScrollValue(int value)
        {
            var maximumValue = Math.Max(_scrollBar.Minimum,
                _scrollBar.Maximum - _scrollBar.LargeChange + 1);
            _scrollBar.Value = Math.Max(_scrollBar.Minimum,
                Math.Min(maximumValue, value));
        }
    }
    
    internal sealed class CommunicationDeviceSelectedEventArgs : EventArgs
    {
        public CommunicationDeviceSelectedEventArgs(
            DeviceConnectionConfiguration configuration)
        {
            Configuration = configuration;
        }

        public DeviceConnectionConfiguration Configuration { get; }
    }
}
