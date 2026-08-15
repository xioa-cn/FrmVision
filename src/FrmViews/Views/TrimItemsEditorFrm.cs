using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FrmViews.Controls;

namespace FrmViews.Views
{
    internal sealed class TrimItemsEditorFrm : Form
    {
        private readonly DataGridView _grid;

        public TrimItemsEditorFrm(string value)
        {
            Text = "编辑 Trim 列表";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            Font = new Font("Microsoft YaHei UI", 9F);
            BackColor = UiTheme.Surface;
            ClientSize = new Size(520, 360);

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

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = UiTheme.Surface,
                BorderStyle = BorderStyle.FixedSingle,
                MultiSelect = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                EnableHeadersVisualStyles = false,
                EditMode = DataGridViewEditMode.EditOnEnter,
                ColumnHeadersHeight = 38,
                GridColor = UiTheme.Border
            };
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "按顺序移除的字符组"
            });

            // Use fixed button columns so both actions remain aligned at the bottom.
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
            button.FlatAppearance.BorderColor = primary ? UiTheme.PrimarySoft : UiTheme.Border;
            button.Click += click;
            return button;
        }

        private void LoadItems(string value)
        {
            List<string> items;
            try { items = FrmViews.Nodes.TrimItemSerializer.Parse(value); }
            catch { items = new List<string>(); }
            foreach (string item in items) _grid.Rows.Add(item);
        }

        private void AddClick(object sender, EventArgs e)
        {
            int row = _grid.Rows.Add(string.Empty);
            _grid.CurrentCell = _grid.Rows[row].Cells[0];
            _grid.BeginEdit(true);
        }

        private void DeleteClick(object sender, EventArgs e)
        {
            if (_grid.CurrentRow != null) _grid.Rows.Remove(_grid.CurrentRow);
        }

        private void MoveRow(int offset)
        {
            if (_grid.CurrentRow == null) return;
            int index = _grid.CurrentRow.Index;
            int target = index + offset;
            if (target < 0 || target >= _grid.Rows.Count) return;
            string item = Convert.ToString(_grid.CurrentRow.Cells[0].Value) ?? string.Empty;
            _grid.Rows.RemoveAt(index);
            _grid.Rows.Insert(target, item);
            _grid.CurrentCell = _grid.Rows[target].Cells[0];
        }

        private void OkClick(object sender, EventArgs e)
        {
            _grid.EndEdit();
            var items = new List<string>();
            foreach (DataGridViewRow row in _grid.Rows)
            {
                string item = Convert.ToString(row.Cells[0].Value) ?? string.Empty;
                if (item.Length > 0) items.Add(item);
            }
            Items = FrmViews.Nodes.TrimItemSerializer.Serialize(items);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
