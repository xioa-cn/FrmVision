using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FrmViews.Controls;
using FrmViews.Nodes;

namespace FrmViews.Views
{
    internal sealed class PlcWriteItemEditorFrm : Form
    {
        private const string InputSourceText = "输入";
        private const string FixedSourceText = "固定值";

        private TableLayoutPanel _rootLayout;
        private Panel _toolbarPanel;
        private Label _sectionLabel;
        private FlowLayoutPanel _toolbarActions;
        private Button _addButton;
        private Button _deleteButton;
        private DataGridView _writeGrid;
        private DataGridViewTextBoxColumn _keyColumn;
        private DataGridViewTextBoxColumn _addressColumn;
        private DataGridViewComboBoxColumn _typeColumn;
        private DataGridViewComboBoxColumn _sourceColumn;
        private DataGridViewTextBoxColumn _fixedValueColumn;
        private Panel _footerPanel;
        private Label _footerDivider;
        private FlowLayoutPanel _footerActions;
        private Button _okButton;
        private Button _cancelButton;

        public PlcWriteItemEditorFrm(string writeItems)
        {
            InitializeComponent();
            ConfigureColumns();
            LoadWriteItems(writeItems);
        }

        public string WriteItems { get; private set; }

        private void ConfigureColumns()
        {
            _typeColumn.Items.AddRange(Enum.GetNames(typeof(PlcWriteValueType)));
            _sourceColumn.Items.AddRange(InputSourceText, FixedSourceText);
        }

        private void LoadWriteItems(string writeItems)
        {
            List<PlcWriteItemDefinition> definitions;
            try
            {
                definitions = PlcWriteItemSerializer.Parse(writeItems);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.GetBaseException().Message, "写入配置",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                definitions = new List<PlcWriteItemDefinition>();
            }

            foreach (PlcWriteItemDefinition definition in definitions)
            {
                int index = _writeGrid.Rows.Add(
                    definition.Key,
                    definition.Address,
                    definition.ValueType.ToString(),
                    GetSourceText(definition.ValueSource),
                    definition.FixedValue);
                UpdateFixedValueCell(_writeGrid.Rows[index]);
            }

            if (_writeGrid.Rows.Count == 0)
                AddWriteRow();
            _writeGrid.ClearSelection();
            UpdateDialogHeight();
        }

        private void AddButtonOnClick(object sender, EventArgs e)
        {
            int index = AddWriteRow();
            _writeGrid.CurrentCell = _writeGrid.Rows[index].Cells[_keyColumn.Index];
            _writeGrid.BeginEdit(true);
            UpdateDialogHeight();
        }

        private int AddWriteRow()
        {
            int sequence = _writeGrid.Rows.Count + 1;
            int index = _writeGrid.Rows.Add(
                "写入" + sequence,
                string.Empty,
                PlcWriteValueType.Int.ToString(),
                InputSourceText,
                string.Empty);
            UpdateFixedValueCell(_writeGrid.Rows[index]);
            return index;
        }

        private void DeleteButtonOnClick(object sender, EventArgs e)
        {
            DataGridViewRow row = _writeGrid.CurrentRow;
            if (row != null && !row.IsNewRow)
                _writeGrid.Rows.Remove(row);
            UpdateDialogHeight();
        }

        private void WriteGridOnCurrentCellDirtyStateChanged(object sender,
            EventArgs e)
        {
            if (_writeGrid.IsCurrentCellDirty)
                _writeGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void WriteGridOnCellValueChanged(object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != _sourceColumn.Index) return;
            UpdateFixedValueCell(_writeGrid.Rows[e.RowIndex]);
        }

        private void WriteGridOnDataError(object sender,
            DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void UpdateFixedValueCell(DataGridViewRow row)
        {
            string source = Convert.ToString(row.Cells[_sourceColumn.Index].Value);
            DataGridViewCell cell = row.Cells[_fixedValueColumn.Index];
            bool isFixed = string.Equals(source, FixedSourceText,
                StringComparison.Ordinal);
            cell.ReadOnly = !isFixed;
            cell.Style.BackColor = isFixed ? UiTheme.Input : UiTheme.SurfaceMuted;
            cell.Style.ForeColor = isFixed ? UiTheme.Text : UiTheme.Muted;
        }

        private void OkButtonOnClick(object sender, EventArgs e)
        {
            _writeGrid.EndEdit();
            var definitions = new List<PlcWriteItemDefinition>();
            var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (DataGridViewRow row in _writeGrid.Rows)
            {
                if (row.IsNewRow) continue;
                string key = GetCellText(row, _keyColumn);
                string address = GetCellText(row, _addressColumn);
                string typeText = GetCellText(row, _typeColumn);
                string sourceText = GetCellText(row, _sourceColumn);
                string fixedValue = Convert.ToString(
                    row.Cells[_fixedValueColumn.Index].Value) ?? string.Empty;

                if (key.Length == 0)
                {
                    ShowValidationMessage("每个写入项都必须填写 Key。", row,
                        _keyColumn);
                    return;
                }
                if (!keys.Add(key))
                {
                    ShowValidationMessage("写入 Key 不能重复：" + key, row,
                        _keyColumn);
                    return;
                }
                if (address.Length == 0)
                {
                    ShowValidationMessage("写入项 " + key + " 必须填写地址。", row,
                        _addressColumn);
                    return;
                }

                PlcWriteValueType valueType;
                if (!Enum.TryParse(typeText, out valueType))
                {
                    ShowValidationMessage("写入项 " + key + " 的类型无效。", row,
                        _typeColumn);
                    return;
                }

                PlcWriteValueSource source;
                if (string.Equals(sourceText, InputSourceText,
                        StringComparison.Ordinal))
                    source = PlcWriteValueSource.Input;
                else if (string.Equals(sourceText, FixedSourceText,
                             StringComparison.Ordinal))
                    source = PlcWriteValueSource.FixedValue;
                else
                {
                    ShowValidationMessage("写入项 " + key + " 的取值方式无效。",
                        row, _sourceColumn);
                    return;
                }

                if (source == PlcWriteValueSource.FixedValue)
                {
                    try
                    {
                        PlcNode.ConvertWriteValue(fixedValue, valueType);
                    }
                    catch (Exception ex)
                    {
                        ShowValidationMessage("写入项 " + key + " 的固定值无效：" +
                                              ex.GetBaseException().Message,
                            row, _fixedValueColumn);
                        return;
                    }
                }

                definitions.Add(new PlcWriteItemDefinition
                {
                    Key = key,
                    Address = address,
                    ValueType = valueType,
                    ValueSource = source,
                    FixedValue = fixedValue
                });
            }

            if (definitions.Count == 0)
            {
                MessageBox.Show(this, "至少需要添加一个 PLC 写入项。", "写入配置",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            WriteItems = PlcWriteItemSerializer.Serialize(definitions);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ShowValidationMessage(string message, DataGridViewRow row,
            DataGridViewColumn column)
        {
            MessageBox.Show(this, message, "写入配置",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _writeGrid.CurrentCell = row.Cells[column.Index];
            _writeGrid.Focus();
        }

        private static string GetCellText(DataGridViewRow row,
            DataGridViewColumn column)
        {
            return (Convert.ToString(row.Cells[column.Index].Value) ?? string.Empty)
                .Trim();
        }

        private static string GetSourceText(PlcWriteValueSource source)
        {
            return source == PlcWriteValueSource.FixedValue
                ? FixedSourceText
                : InputSourceText;
        }

        private void UpdateDialogHeight()
        {
            int visibleRows = Math.Max(3, Math.Min(8, _writeGrid.Rows.Count));
            int gridHeight = _writeGrid.ColumnHeadersHeight +
                             visibleRows * _writeGrid.RowTemplate.Height + 2;
            ClientSize = new Size(900, 120 + gridHeight);
        }

        private void InitializeComponent()
        {
            var headerStyle = new DataGridViewCellStyle();
            var cellStyle = new DataGridViewCellStyle();
            _rootLayout = new TableLayoutPanel();
            _toolbarPanel = new Panel();
            _sectionLabel = new Label();
            _toolbarActions = new FlowLayoutPanel();
            _addButton = new Button();
            _deleteButton = new Button();
            _writeGrid = new DataGridView();
            _keyColumn = new DataGridViewTextBoxColumn();
            _addressColumn = new DataGridViewTextBoxColumn();
            _typeColumn = new DataGridViewComboBoxColumn();
            _sourceColumn = new DataGridViewComboBoxColumn();
            _fixedValueColumn = new DataGridViewTextBoxColumn();
            _footerPanel = new Panel();
            _footerDivider = new Label();
            _footerActions = new FlowLayoutPanel();
            _okButton = new Button();
            _cancelButton = new Button();
            _rootLayout.SuspendLayout();
            _toolbarPanel.SuspendLayout();
            _toolbarActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)_writeGrid).BeginInit();
            _footerPanel.SuspendLayout();
            SuspendLayout();

            _rootLayout.ColumnCount = 1;
            _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _rootLayout.Controls.Add(_toolbarPanel, 0, 0);
            _rootLayout.Controls.Add(_writeGrid, 0, 1);
            _rootLayout.Controls.Add(_footerPanel, 0, 2);
            _rootLayout.Dock = DockStyle.Fill;
            _rootLayout.Padding = new Padding(16, 10, 16, 0);
            _rootLayout.RowCount = 3;
            _rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));

            _toolbarPanel.Controls.Add(_sectionLabel);
            _toolbarPanel.Controls.Add(_toolbarActions);
            _toolbarPanel.Dock = DockStyle.Fill;
            _toolbarPanel.Margin = Padding.Empty;

            _sectionLabel.Dock = DockStyle.Left;
            _sectionLabel.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            _sectionLabel.ForeColor = UiTheme.Text;
            _sectionLabel.Size = new Size(180, 48);
            _sectionLabel.Text = "PLC 写入项";
            _sectionLabel.TextAlign = ContentAlignment.MiddleLeft;

            _toolbarActions.Controls.Add(_addButton);
            _toolbarActions.Controls.Add(_deleteButton);
            _toolbarActions.Dock = DockStyle.Right;
            _toolbarActions.Location = new Point(664, 0);
            _toolbarActions.Padding = new Padding(0, 7, 0, 0);
            _toolbarActions.Size = new Size(204, 48);

            _addButton.BackColor = UiTheme.PrimarySoft;
            _addButton.FlatAppearance.BorderColor = UiTheme.PrimarySoft;
            _addButton.FlatStyle = FlatStyle.Flat;
            _addButton.Font = new Font("Microsoft YaHei UI", 9F);
            _addButton.ForeColor = UiTheme.Primary;
            _addButton.Margin = new Padding(0, 0, 8, 0);
            _addButton.Size = new Size(98, 34);
            _addButton.Text = "+  添加";
            _addButton.UseVisualStyleBackColor = false;
            _addButton.Click += AddButtonOnClick;

            _deleteButton.BackColor = UiTheme.Surface;
            _deleteButton.FlatAppearance.BorderColor = UiTheme.Border;
            _deleteButton.FlatStyle = FlatStyle.Flat;
            _deleteButton.Font = new Font("Microsoft YaHei UI", 9F);
            _deleteButton.ForeColor = UiTheme.Muted;
            _deleteButton.Margin = Padding.Empty;
            _deleteButton.Size = new Size(98, 34);
            _deleteButton.Text = "删除";
            _deleteButton.UseVisualStyleBackColor = false;
            _deleteButton.Click += DeleteButtonOnClick;

            _writeGrid.AllowUserToAddRows = false;
            _writeGrid.AllowUserToDeleteRows = false;
            _writeGrid.AllowUserToResizeRows = false;
            _writeGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _writeGrid.BackgroundColor = UiTheme.Surface;
            _writeGrid.BorderStyle = BorderStyle.FixedSingle;
            _writeGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            _writeGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            headerStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            headerStyle.BackColor = UiTheme.SurfaceMuted;
            headerStyle.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            headerStyle.ForeColor = UiTheme.Muted;
            headerStyle.Padding = new Padding(8, 0, 8, 0);
            headerStyle.SelectionBackColor = UiTheme.SurfaceMuted;
            headerStyle.SelectionForeColor = UiTheme.Muted;
            _writeGrid.ColumnHeadersDefaultCellStyle = headerStyle;
            _writeGrid.ColumnHeadersHeight = 38;
            _writeGrid.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _writeGrid.Columns.AddRange(_keyColumn, _addressColumn, _typeColumn,
                _sourceColumn, _fixedValueColumn);
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            cellStyle.BackColor = UiTheme.Surface;
            cellStyle.Font = new Font("Microsoft YaHei UI", 9F);
            cellStyle.ForeColor = UiTheme.Text;
            cellStyle.Padding = new Padding(8, 0, 8, 0);
            cellStyle.SelectionBackColor = UiTheme.PrimarySoft;
            cellStyle.SelectionForeColor = UiTheme.Text;
            _writeGrid.DefaultCellStyle = cellStyle;
            _writeGrid.Dock = DockStyle.Fill;
            _writeGrid.EditMode = DataGridViewEditMode.EditOnEnter;
            _writeGrid.EnableHeadersVisualStyles = false;
            _writeGrid.GridColor = UiTheme.Border;
            _writeGrid.Margin = Padding.Empty;
            _writeGrid.MultiSelect = false;
            _writeGrid.RowHeadersVisible = false;
            _writeGrid.RowTemplate.Height = 42;
            _writeGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            _writeGrid.ShowCellErrors = false;
            _writeGrid.ShowEditingIcon = false;
            _writeGrid.ShowRowErrors = false;
            _writeGrid.CurrentCellDirtyStateChanged +=
                WriteGridOnCurrentCellDirtyStateChanged;
            _writeGrid.CellValueChanged += WriteGridOnCellValueChanged;
            _writeGrid.DataError += WriteGridOnDataError;

            _keyColumn.FillWeight = 20F;
            _keyColumn.HeaderText = "输入 Key";
            _addressColumn.FillWeight = 24F;
            _addressColumn.HeaderText = "PLC 地址";
            _typeColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            _typeColumn.FillWeight = 18F;
            _typeColumn.FlatStyle = FlatStyle.Flat;
            _typeColumn.HeaderText = "数据类型";
            _sourceColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            _sourceColumn.FillWeight = 18F;
            _sourceColumn.FlatStyle = FlatStyle.Flat;
            _sourceColumn.HeaderText = "取值方式";
            _fixedValueColumn.FillWeight = 24F;
            _fixedValueColumn.HeaderText = "固定值";

            _footerPanel.Controls.Add(_footerDivider);
            _footerPanel.Controls.Add(_footerActions);
            _footerPanel.Dock = DockStyle.Fill;
            _footerPanel.Margin = Padding.Empty;

            _footerDivider.BackColor = UiTheme.Border;
            _footerDivider.Dock = DockStyle.Top;
            _footerDivider.Height = 1;

            _footerActions.Controls.Add(_okButton);
            _footerActions.Controls.Add(_cancelButton);
            _footerActions.Dock = DockStyle.Fill;
            _footerActions.FlowDirection = FlowDirection.RightToLeft;
            _footerActions.Padding = new Padding(0, 15, 0, 0);
            _footerActions.WrapContents = false;

            _okButton.BackColor = UiTheme.Primary;
            _okButton.FlatAppearance.BorderColor = UiTheme.Primary;
            _okButton.FlatStyle = FlatStyle.Flat;
            _okButton.Font = new Font("Microsoft YaHei UI", 9F);
            _okButton.ForeColor = Color.White;
            _okButton.Margin = Padding.Empty;
            _okButton.Size = new Size(104, 36);
            _okButton.Text = "确定";
            _okButton.UseVisualStyleBackColor = false;
            _okButton.Click += OkButtonOnClick;

            _cancelButton.BackColor = UiTheme.Surface;
            _cancelButton.DialogResult = DialogResult.Cancel;
            _cancelButton.FlatAppearance.BorderColor = UiTheme.Border;
            _cancelButton.FlatStyle = FlatStyle.Flat;
            _cancelButton.Font = new Font("Microsoft YaHei UI", 9F);
            _cancelButton.ForeColor = UiTheme.Text;
            _cancelButton.Margin = new Padding(0, 0, 8, 0);
            _cancelButton.Size = new Size(104, 36);
            _cancelButton.Text = "取消";
            _cancelButton.UseVisualStyleBackColor = false;

            AcceptButton = _okButton;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = UiTheme.Surface;
            CancelButton = _cancelButton;
            ClientSize = new Size(900, 330);
            Controls.Add(_rootLayout);
            Font = new Font("Microsoft YaHei UI", 9F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "编辑 PLC 写入配置";

            _rootLayout.ResumeLayout(false);
            _toolbarPanel.ResumeLayout(false);
            _toolbarActions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)_writeGrid).EndInit();
            _footerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
