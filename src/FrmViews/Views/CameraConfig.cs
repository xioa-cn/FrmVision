using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FrmServices.ViewModel;
using FrmViews.Controls;

namespace FrmViews.Views
{
    public partial class CameraConfig : Form
    {
        private readonly MainFrmViewModel _viewModel;
        private readonly BindingList<CameraDisplayConfiguration> _items =
            new BindingList<CameraDisplayConfiguration>();
        private readonly BindingSource _bindingSource = new BindingSource();
        private readonly DataGridView _grid = new DataGridView();
        private readonly Button _addButton = new Button();
        private readonly Button _deleteButton = new Button();
        private readonly Button _moveUpButton = new Button();
        private readonly Button _moveDownButton = new Button();
        private readonly Button _saveButton = new Button();
        private readonly Button _cancelButton = new Button();
        private readonly Label _countLabel = new Label();

        public CameraConfig() : this(null)
        {
        }

        public CameraConfig(MainFrmViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            InitializeView();
            LoadItems();
        }

        private void InitializeView()
        {
            SuspendLayout();
            Controls.Clear();
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = UiTheme.Page;
            ClientSize = new Size(860, 540);
            Font = new Font("Microsoft YaHei UI", 9F);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(720, 440);
            StartPosition = FormStartPosition.CenterParent;
            Text = "图像窗口配置";

            var root = new TableLayoutPanel
            {
                ColumnCount = 1,
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                RowCount = 3
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));

            var header = new TableLayoutPanel
            {
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                Margin = Padding.Empty
            };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            var titleLabel = new Label
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                Font = new Font("Microsoft YaHei UI", 14F, FontStyle.Bold),
                ForeColor = UiTheme.Text,
                Text = "主界面图像窗口",
                TextAlign = ContentAlignment.MiddleLeft
            };
            _countLabel.AutoSize = true;
            _countLabel.Dock = DockStyle.Fill;
            _countLabel.ForeColor = UiTheme.Muted;
            _countLabel.TextAlign = ContentAlignment.MiddleRight;
            header.Controls.Add(titleLabel, 0, 0);
            header.Controls.Add(_countLabel, 1, 0);

            ConfigureGrid();
            var footer = CreateFooter();
            root.Controls.Add(header, 0, 0);
            root.Controls.Add(_grid, 0, 1);
            root.Controls.Add(footer, 0, 2);
            Controls.Add(root);

            AcceptButton = _saveButton;
            CancelButton = _cancelButton;
            ResumeLayout(true);
        }

        private void ConfigureGrid()
        {
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.AllowUserToResizeRows = false;
            _grid.AutoGenerateColumns = false;
            _grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            _grid.BackgroundColor = UiTheme.Surface;
            _grid.BorderStyle = BorderStyle.FixedSingle;
            _grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            _grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            _grid.ColumnHeadersHeight = 40;
            _grid.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _grid.DefaultCellStyle.BackColor = UiTheme.Surface;
            _grid.DefaultCellStyle.ForeColor = UiTheme.Text;
            _grid.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            _grid.DefaultCellStyle.SelectionBackColor = UiTheme.PrimarySoft;
            _grid.DefaultCellStyle.SelectionForeColor = UiTheme.Text;
            _grid.Dock = DockStyle.Fill;
            _grid.EditMode = DataGridViewEditMode.EditOnEnter;
            _grid.EnableHeadersVisualStyles = false;
            _grid.GridColor = UiTheme.Border;
            _grid.MultiSelect = false;
            _grid.RowHeadersVisible = false;
            _grid.RowTemplate.Height = 38;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.ColumnHeadersDefaultCellStyle.BackColor = UiTheme.SurfaceMuted;
            _grid.ColumnHeadersDefaultCellStyle.ForeColor = UiTheme.Muted;
            _grid.ColumnHeadersDefaultCellStyle.Font =
                new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);

            _grid.Columns.Add(CreateTextColumn("Index", "序号", 55F, true));
            _grid.Columns.Add(CreateTextColumn("Name", "窗口名称", 150F, false));
            _grid.Columns.Add(CreateTextColumn("ProductName", "关联产品 Key", 180F,
                false));
            _grid.Columns.Add(CreateTextColumn("ParameterName", "参数名称", 180F,
                false));

            _bindingSource.DataSource = _items;
            _grid.DataSource = _bindingSource;
            _bindingSource.CurrentChanged += (sender, args) => UpdateButtonStates();
            _grid.DataError += (sender, args) => args.ThrowException = false;
        }

        private Control CreateFooter()
        {
            var footer = new TableLayoutPanel
            {
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 10, 0, 0)
            };
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var editButtons = new FlowLayoutPanel
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Margin = Padding.Empty,
                WrapContents = false
            };
            ConfigureButton(_addButton, "新增", false, AddItem);
            ConfigureButton(_deleteButton, "删除", false, DeleteItem);
            ConfigureButton(_moveUpButton, "上移", false,
                (sender, args) => MoveCurrent(-1));
            ConfigureButton(_moveDownButton, "下移", false,
                (sender, args) => MoveCurrent(1));
            editButtons.Controls.AddRange(new Control[]
            {
                _addButton, _deleteButton, _moveUpButton, _moveDownButton
            });

            var dialogButtons = new FlowLayoutPanel
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Margin = Padding.Empty,
                WrapContents = false
            };
            ConfigureButton(_saveButton, "保存", true, SaveConfiguration);
            ConfigureButton(_cancelButton, "取消", false,
                (sender, args) => Close());
            _cancelButton.DialogResult = DialogResult.Cancel;
            dialogButtons.Controls.Add(_saveButton);
            dialogButtons.Controls.Add(_cancelButton);

            footer.Controls.Add(editButtons, 0, 0);
            footer.Controls.Add(dialogButtons, 1, 0);
            return footer;
        }

        private static DataGridViewTextBoxColumn CreateTextColumn(
            string propertyName, string headerText, float fillWeight,
            bool readOnly)
        {
            return new DataGridViewTextBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DataPropertyName = propertyName,
                FillWeight = fillWeight,
                HeaderText = headerText,
                MinimumWidth = readOnly ? 55 : 120,
                ReadOnly = readOnly,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
        }

        private static void ConfigureButton(Button button, string text,
            bool primary, EventHandler clickHandler)
        {
            UiTheme.StyleCommandButton(button, primary);
            button.AutoSize = false;
            button.Margin = new Padding(0, 0, 8, 0);
            button.Size = new Size(82, 36);
            button.Text = text;
            button.Click += clickHandler;
        }

        private void LoadItems()
        {
            if (_viewModel != null)
            {
                foreach (CameraPanelViewModel camera in _viewModel.Cameras)
                {
                    _items.Add(new CameraDisplayConfiguration
                    {
                        Index = camera.Index,
                        Name = camera.Name,
                        ProductName = camera.ProductName,
                        ParameterName = camera.ParameterName
                    });
                }
            }
            ReindexItems();
            UpdateButtonStates();
        }

        private void AddItem(object sender, EventArgs e)
        {
            _grid.EndEdit();
            int number = 1;
            while (_items.Any(existing => string.Equals(existing.Name, "相机 " + number,
                       StringComparison.OrdinalIgnoreCase)))
                number++;

            var item = new CameraDisplayConfiguration
            {
                Index = _items.Count + 1,
                Name = "相机 " + number,
                ProductName = "产品" + number,
                ParameterName = string.Empty
            };
            _items.Add(item);
            ReindexItems();
            _bindingSource.Position = _items.IndexOf(item);
            UpdateButtonStates();
        }

        private void DeleteItem(object sender, EventArgs e)
        {
            var item = _bindingSource.Current as CameraDisplayConfiguration;
            if (item == null) return;
            int position = _bindingSource.Position;
            _items.Remove(item);
            ReindexItems();
            if (_items.Count > 0)
                _bindingSource.Position = Math.Min(position, _items.Count - 1);
            UpdateButtonStates();
        }

        private void MoveCurrent(int offset)
        {
            _grid.EndEdit();
            var item = _bindingSource.Current as CameraDisplayConfiguration;
            if (item == null) return;
            int currentIndex = _items.IndexOf(item);
            int targetIndex = currentIndex + offset;
            if (targetIndex < 0 || targetIndex >= _items.Count) return;

            _items.RaiseListChangedEvents = false;
            try
            {
                _items.RemoveAt(currentIndex);
                _items.Insert(targetIndex, item);
                ReindexItems(false);
            }
            finally
            {
                _items.RaiseListChangedEvents = true;
                _bindingSource.ResetBindings(false);
            }
            _bindingSource.Position = targetIndex;
            UpdateButtonStates();
        }

        private void ReindexItems(bool resetBindings = true)
        {
            for (int index = 0; index < _items.Count; index++)
                _items[index].Index = index + 1;
            if (resetBindings) _bindingSource.ResetBindings(false);
            _countLabel.Text = _items.Count + " 个窗口";
        }

        private void UpdateButtonStates()
        {
            int position = _bindingSource.Position;
            bool selected = position >= 0 && position < _items.Count;
            _deleteButton.Enabled = selected;
            _moveUpButton.Enabled = selected && position > 0;
            _moveDownButton.Enabled = selected && position < _items.Count - 1;
            _saveButton.Enabled = _viewModel != null;
            _countLabel.Text = _items.Count + " 个窗口";
        }

        private void SaveConfiguration(object sender, EventArgs e)
        {
            _grid.EndEdit();
            _bindingSource.EndEdit();
            ReindexItems();

            string errorMessage = string.Empty;
            if (_viewModel == null || !_viewModel.TryConfigureCameraDisplays(
                    _items.ToArray(), out errorMessage))
            {
                MessageBox.Show(this,
                    string.IsNullOrWhiteSpace(errorMessage)
                        ? "图像窗口配置不可用。"
                        : errorMessage,
                    "图像窗口配置", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
