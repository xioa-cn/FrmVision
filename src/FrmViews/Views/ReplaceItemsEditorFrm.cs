using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FrmViews.Controls;
using FrmViews.Nodes;

namespace FrmViews.Views
{
    internal sealed class ReplaceItemsEditorFrm : Form
    {
        private readonly DataGridView _grid;
        private readonly DataGridViewTextBoxColumn _findColumn;
        private readonly DataGridViewTextBoxColumn _replacementColumn;

        public ReplaceItemsEditorFrm(string value)
        {
            Text = "编辑 Replace 列表";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            Font = new Font("Microsoft YaHei UI", 9F);
            BackColor = UiTheme.Surface;
            ClientSize = new Size(620, 390);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(16, 10, 16, 0)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));

            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false,
                Padding = new Padding(0, 7, 0, 0)
            };
            toolbar.Controls.Add(CreateToolButton("添加", AddClick, true));
            toolbar.Controls.Add(CreateToolButton("删除", DeleteClick, false));
            toolbar.Controls.Add(CreateToolButton("上移", (s, e) => MoveRow(-1), false));
            toolbar.Controls.Add(CreateToolButton("下移", (s, e) => MoveRow(1), false));

            _findColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "查找内容",
                FillWeight = 50F
            };
            _replacementColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "替换内容（可为空）",
                FillWeight = 50F
            };
            _grid = CreateGrid();
            _grid.Columns.AddRange(_findColumn, _replacementColumn);

            var footer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Padding = new Padding(0, 15, 0, 0)
            };
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 108F));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 108F));

            var cancel = UiTheme.StyleCommandButton(new Button
            {
                Text = "取消",
                DialogResult = DialogResult.Cancel,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 8, 0)
            }, false);
            var ok = UiTheme.StyleCommandButton(new Button
            {
                Text = "确定",
                Dock = DockStyle.Fill,
                Margin = Padding.Empty
            }, true);
            ok.Click += OkClick;
            footer.Controls.Add(cancel, 1, 0);
            footer.Controls.Add(ok, 2, 0);

            root.Controls.Add(toolbar, 0, 0);
            root.Controls.Add(_grid, 0, 1);
            root.Controls.Add(footer, 0, 2);
            Controls.Add(root);
            AcceptButton = ok;
            CancelButton = cancel;
            LoadItems(value);
        }

        public string Items { get; private set; }

        private static DataGridView CreateGrid()
        {
            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = UiTheme.Surface,
                BorderStyle = BorderStyle.FixedSingle,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersHeight = 38,
                EditMode = DataGridViewEditMode.EditOnEnter,
                EnableHeadersVisualStyles = false,
                GridColor = UiTheme.Border,
                MultiSelect = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            grid.RowTemplate.Height = 42;
            return grid;
        }

        private static Button CreateToolButton(string text, EventHandler click,
            bool primary)
        {
            var button = new Button
            {
                Text = text,
                Size = new Size(88, 34),
                Margin = new Padding(0, 0, 8, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = primary ? UiTheme.PrimarySoft : UiTheme.Surface,
                ForeColor = primary ? UiTheme.Primary : UiTheme.Text,
                UseVisualStyleBackColor = false
            };
            button.FlatAppearance.BorderColor = primary
                ? UiTheme.PrimarySoft
                : UiTheme.Border;
            button.Click += click;
            return button;
        }

        private void LoadItems(string value)
        {
            List<ReplaceItemDefinition> items;
            try
            {
                items = ReplaceItemSerializer.Parse(value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.GetBaseException().Message,
                    "Replace 列表", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                items = new List<ReplaceItemDefinition>();
            }

            foreach (ReplaceItemDefinition item in items)
                _grid.Rows.Add(item.Find, item.Replacement);
        }

        private void AddClick(object sender, EventArgs e)
        {
            int row = _grid.Rows.Add(string.Empty, string.Empty);
            _grid.CurrentCell = _grid.Rows[row].Cells[_findColumn.Index];
            _grid.BeginEdit(true);
        }

        private void DeleteClick(object sender, EventArgs e)
        {
            if (_grid.CurrentRow != null) _grid.Rows.Remove(_grid.CurrentRow);
        }

        private void MoveRow(int offset)
        {
            DataGridViewRow row = _grid.CurrentRow;
            if (row == null) return;
            int target = row.Index + offset;
            if (target < 0 || target >= _grid.Rows.Count) return;
            string find = Convert.ToString(row.Cells[_findColumn.Index].Value) ??
                          string.Empty;
            string replacement = Convert.ToString(
                row.Cells[_replacementColumn.Index].Value) ?? string.Empty;
            _grid.Rows.RemoveAt(row.Index);
            _grid.Rows.Insert(target, find, replacement);
            _grid.CurrentCell = _grid.Rows[target].Cells[_findColumn.Index];
        }

        private void OkClick(object sender, EventArgs e)
        {
            _grid.EndEdit();
            var items = new List<ReplaceItemDefinition>();
            foreach (DataGridViewRow row in _grid.Rows)
            {
                string find = Convert.ToString(
                    row.Cells[_findColumn.Index].Value) ?? string.Empty;
                string replacement = Convert.ToString(
                    row.Cells[_replacementColumn.Index].Value) ?? string.Empty;
                if (find.Length == 0)
                {
                    MessageBox.Show(this, "查找内容不能为空。", "Replace 列表",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _grid.CurrentCell = row.Cells[_findColumn.Index];
                    _grid.Focus();
                    return;
                }
                items.Add(new ReplaceItemDefinition
                {
                    Find = find,
                    Replacement = replacement
                });
            }

            Items = ReplaceItemSerializer.Serialize(items);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
