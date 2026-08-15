using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using FrmViews.Controls;
using FrmViews.Nodes;

namespace FrmViews.Views
{
    internal sealed class CompensationStepEditorFrm : Form
    {
        private const string AddText = "加 (+)";
        private const string SubtractText = "减 (-)";
        private const string MultiplyText = "乘 (x)";
        private const string DivideText = "除 (/)";

        private readonly DataGridView _stepGrid;
        private readonly DataGridViewComboBoxColumn _operationColumn;
        private readonly DataGridViewTextBoxColumn _valueColumn;

        public CompensationStepEditorFrm(string steps)
        {
            Text = "编辑补偿步骤";
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
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 7, 0, 0)
            };
            toolbar.Controls.Add(CreateToolButton("添加", AddButtonOnClick, true));
            toolbar.Controls.Add(CreateToolButton("删除", DeleteButtonOnClick, false));
            toolbar.Controls.Add(CreateToolButton("上移", MoveUpButtonOnClick, false));
            toolbar.Controls.Add(CreateToolButton("下移", MoveDownButtonOnClick, false));

            _operationColumn = new DataGridViewComboBoxColumn
            {
                HeaderText = "运算",
                FillWeight = 42F,
                FlatStyle = FlatStyle.Flat,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
            };
            _operationColumn.Items.AddRange(AddText, SubtractText, MultiplyText,
                DivideText);
            _valueColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "运算值",
                FillWeight = 58F
            };
            _stepGrid = CreateGrid();
            _stepGrid.Columns.AddRange(_operationColumn, _valueColumn);

            var footer = new Panel { Dock = DockStyle.Fill };
            footer.Controls.Add(new Label
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = UiTheme.Border
            });
            var actions = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Padding = new Padding(0, 15, 0, 0)
            };
            var okButton = UiTheme.StyleCommandButton(new Button { Text = "确定" }, true);
            var cancelButton = UiTheme.StyleCommandButton(new Button
            {
                Text = "取消",
                DialogResult = DialogResult.Cancel,
                Margin = new Padding(0, 0, 8, 0)
            }, false);
            okButton.Click += OkButtonOnClick;
            actions.Controls.Add(okButton);
            actions.Controls.Add(cancelButton);
            footer.Controls.Add(actions);

            root.Controls.Add(toolbar, 0, 0);
            root.Controls.Add(_stepGrid, 0, 1);
            root.Controls.Add(footer, 0, 2);
            Controls.Add(root);
            AcceptButton = okButton;
            CancelButton = cancelButton;

            LoadSteps(steps);
        }

        public string Steps { get; private set; }

        private DataGridView CreateGrid()
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
                ColumnHeadersHeightSizeMode =
                    DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                EditMode = DataGridViewEditMode.EditOnEnter,
                EnableHeadersVisualStyles = false,
                GridColor = UiTheme.Border,
                MultiSelect = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ShowCellErrors = false,
                ShowRowErrors = false
            };
            grid.RowTemplate.Height = 42;
            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = UiTheme.SurfaceMuted,
                ForeColor = UiTheme.Muted,
                Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold),
                SelectionBackColor = UiTheme.SurfaceMuted,
                SelectionForeColor = UiTheme.Muted,
                Padding = new Padding(8, 0, 8, 0)
            };
            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = UiTheme.Surface,
                ForeColor = UiTheme.Text,
                Font = new Font("Microsoft YaHei UI", 9F),
                SelectionBackColor = UiTheme.PrimarySoft,
                SelectionForeColor = UiTheme.Text,
                Padding = new Padding(8, 0, 8, 0)
            };
            grid.DataError += (sender, e) => e.ThrowException = false;
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
                ? UiTheme.PrimarySoft : UiTheme.Border;
            button.Click += click;
            return button;
        }

        private void LoadSteps(string value)
        {
            List<CompensationStepDefinition> steps;
            try
            {
                steps = CompensationStepSerializer.Parse(value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.GetBaseException().Message, "补偿步骤",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                steps = new List<CompensationStepDefinition>();
            }

            foreach (CompensationStepDefinition step in steps)
                _stepGrid.Rows.Add(GetOperationText(step.Operation),
                    step.Value.ToString("R", CultureInfo.InvariantCulture));
            if (_stepGrid.Rows.Count == 0) AddStep();
            _stepGrid.ClearSelection();
        }

        private void AddButtonOnClick(object sender, EventArgs e)
        {
            int index = AddStep();
            _stepGrid.CurrentCell = _stepGrid.Rows[index].Cells[_valueColumn.Index];
            _stepGrid.BeginEdit(true);
        }

        private int AddStep()
        {
            return _stepGrid.Rows.Add(AddText, "0");
        }

        private void DeleteButtonOnClick(object sender, EventArgs e)
        {
            if (_stepGrid.CurrentRow != null)
                _stepGrid.Rows.Remove(_stepGrid.CurrentRow);
        }

        private void MoveUpButtonOnClick(object sender, EventArgs e)
        {
            MoveCurrentRow(-1);
        }

        private void MoveDownButtonOnClick(object sender, EventArgs e)
        {
            MoveCurrentRow(1);
        }

        private void MoveCurrentRow(int offset)
        {
            DataGridViewRow row = _stepGrid.CurrentRow;
            if (row == null) return;
            int targetIndex = row.Index + offset;
            if (targetIndex < 0 || targetIndex >= _stepGrid.Rows.Count) return;
            string operation = Convert.ToString(row.Cells[_operationColumn.Index].Value);
            string value = Convert.ToString(row.Cells[_valueColumn.Index].Value);
            _stepGrid.Rows.RemoveAt(row.Index);
            _stepGrid.Rows.Insert(targetIndex, operation, value);
            _stepGrid.CurrentCell = _stepGrid.Rows[targetIndex]
                .Cells[_valueColumn.Index];
        }

        private void OkButtonOnClick(object sender, EventArgs e)
        {
            _stepGrid.EndEdit();
            var steps = new List<CompensationStepDefinition>();
            foreach (DataGridViewRow row in _stepGrid.Rows)
            {
                CompensationOperation operation;
                if (!TryGetOperation(Convert.ToString(
                        row.Cells[_operationColumn.Index].Value), out operation))
                {
                    ShowValidation("请选择有效的运算类型。", row, _operationColumn);
                    return;
                }

                string valueText = (Convert.ToString(
                    row.Cells[_valueColumn.Index].Value) ?? string.Empty).Trim();
                double operand;
                if (!double.TryParse(valueText, NumberStyles.Float,
                        CultureInfo.InvariantCulture, out operand) ||
                    double.IsNaN(operand) || double.IsInfinity(operand))
                {
                    ShowValidation("运算值必须是有效数字，小数点请使用 .。", row,
                        _valueColumn);
                    return;
                }
                if (operation == CompensationOperation.Divide && operand == 0D)
                {
                    ShowValidation("除数不能为零。", row, _valueColumn);
                    return;
                }
                steps.Add(new CompensationStepDefinition
                {
                    Operation = operation,
                    Value = operand
                });
            }

            if (steps.Count == 0)
            {
                MessageBox.Show(this, "至少需要添加一个补偿步骤。", "补偿步骤",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Steps = CompensationStepSerializer.Serialize(steps);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ShowValidation(string message, DataGridViewRow row,
            DataGridViewColumn column)
        {
            MessageBox.Show(this, message, "补偿步骤",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _stepGrid.CurrentCell = row.Cells[column.Index];
            _stepGrid.Focus();
        }

        private static string GetOperationText(CompensationOperation operation)
        {
            switch (operation)
            {
                case CompensationOperation.Add: return AddText;
                case CompensationOperation.Subtract: return SubtractText;
                case CompensationOperation.Multiply: return MultiplyText;
                case CompensationOperation.Divide: return DivideText;
                default: return AddText;
            }
        }

        private static bool TryGetOperation(string value,
            out CompensationOperation operation)
        {
            switch (value)
            {
                case AddText:
                    operation = CompensationOperation.Add;
                    return true;
                case SubtractText:
                    operation = CompensationOperation.Subtract;
                    return true;
                case MultiplyText:
                    operation = CompensationOperation.Multiply;
                    return true;
                case DivideText:
                    operation = CompensationOperation.Divide;
                    return true;
                default:
                    operation = CompensationOperation.Add;
                    return false;
            }
        }
    }
}
