using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FrmViews.Nodes;

namespace FrmViews.Views
{
    internal partial class PlcComparisonOutputEditorFrm : Form
    {
        private readonly PlcReadValueType _readValueType;
        private readonly bool _keyOnlyMode;
        private readonly string _reservedKey;
        private readonly string _keyDisplayName;

        public PlcComparisonOutputEditorFrm(PlcReadValueType readValueType,
            string comparisonOutputs, string dialogTitle = null,
            bool keyOnlyMode = false, string reservedKey = null,
            string keyDisplayName = "输出 Key")
        {
            _readValueType = readValueType;
            _keyOnlyMode = keyOnlyMode;
            _reservedKey = reservedKey;
            _keyDisplayName = string.IsNullOrWhiteSpace(keyDisplayName)
                ? "Key"
                : keyDisplayName.Trim();
            InitializeComponent();
            if (!string.IsNullOrWhiteSpace(dialogTitle)) Text = dialogTitle;
            if (_keyOnlyMode)
            {
                sectionLabel.Text = _keyDisplayName;
                operatorColumn.Visible = false;
                valueColumn.HeaderText = _keyDisplayName;
                valueColumn.FillWeight = 100F;
            }
            else
            {
                ConfigureOperators();
            }
            LoadConditions(comparisonOutputs);
        }

        public string ComparisonOutputs { get; private set; }

        private void ConfigureOperators()
        {
            operatorColumn.Items.Clear();
            if (_readValueType != PlcReadValueType.String)
            {
                operatorColumn.Items.Add(">");
                operatorColumn.Items.Add("<");
            }
            operatorColumn.Items.Add("=");
        }

        private void LoadConditions(string comparisonOutputs)
        {
            if (_keyOnlyMode)
            {
                LoadKeys(comparisonOutputs);
                return;
            }

            var allowedOperators = new HashSet<string>(
                operatorColumn.Items.Cast<object>().Select(item => item.ToString()),
                StringComparer.Ordinal);
            string[] tokens = (comparisonOutputs ?? string.Empty).Split(
                new[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string rawToken in tokens)
            {
                string token = rawToken.Trim();
                if (token.Length < 1) continue;
                string comparisonOperator = token.Substring(0, 1);
                if (!allowedOperators.Contains(comparisonOperator)) continue;
                string value = token.Substring(1).Trim();
                conditionGrid.Rows.Add(comparisonOperator, value);
            }

            if (conditionGrid.Rows.Count == 0)
                conditionGrid.Rows.Add("=", string.Empty);
            conditionGrid.ClearSelection();
            UpdateDialogHeight();
        }

        private void LoadKeys(string outputKeys)
        {
            string[] tokens = (outputKeys ?? string.Empty).Split(
                new[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string rawToken in tokens)
            {
                string key = rawToken.Trim();
                if (key.Length > 0) conditionGrid.Rows.Add(null, key);
            }

            if (conditionGrid.Rows.Count == 0)
                conditionGrid.Rows.Add(null, string.Empty);
            conditionGrid.ClearSelection();
            UpdateDialogHeight();
        }

        private void AddButtonOnClick(object sender, EventArgs e)
        {
            int index = _keyOnlyMode
                ? conditionGrid.Rows.Add(null, string.Empty)
                : conditionGrid.Rows.Add("=", string.Empty);
            conditionGrid.CurrentCell = conditionGrid.Rows[index].Cells[valueColumn.Index];
            conditionGrid.BeginEdit(true);
            UpdateDialogHeight();
        }

        private void DeleteButtonOnClick(object sender, EventArgs e)
        {
            var rows = conditionGrid.SelectedRows.Cast<DataGridViewRow>().ToArray();
            if (rows.Length == 0 && conditionGrid.CurrentRow != null)
                rows = new[] { conditionGrid.CurrentRow };

            foreach (DataGridViewRow row in rows)
                if (!row.IsNewRow) conditionGrid.Rows.Remove(row);
            UpdateDialogHeight();
        }

        private void UpdateDialogHeight()
        {
            int visibleRows = Math.Max(2, Math.Min(7, conditionGrid.Rows.Count));
            int gridHeight = conditionGrid.ColumnHeadersHeight +
                             visibleRows * conditionGrid.RowTemplate.Height + 2;
            ClientSize = new System.Drawing.Size(540, 120 + gridHeight);
        }

        private void OkButtonOnClick(object sender, EventArgs e)
        {
            conditionGrid.EndEdit();
            if (_keyOnlyMode)
            {
                SaveKeys();
                return;
            }

            var labels = new List<string>();
            var uniqueLabels = new HashSet<string>(StringComparer.Ordinal);
            foreach (DataGridViewRow row in conditionGrid.Rows)
            {
                if (row.IsNewRow) continue;
                string comparisonOperator = Convert.ToString(
                    row.Cells[operatorColumn.Index].Value)?.Trim();
                string value = Convert.ToString(row.Cells[valueColumn.Index].Value)?.Trim();
                if (string.IsNullOrEmpty(comparisonOperator) || string.IsNullOrEmpty(value))
                {
                    MessageBox.Show(this, "每个输出都必须设置比较符和值。", "比较输出",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string label = comparisonOperator + value;
                if (!uniqueLabels.Add(label))
                {
                    MessageBox.Show(this, "比较输出不能重复：" + label, "比较输出",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                labels.Add(label);
            }

            if (labels.Count == 0)
            {
                MessageBox.Show(this, "至少需要添加一个比较输出。", "比较输出",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ComparisonOutputs = string.Join(";", labels.ToArray());
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SaveKeys()
        {
            var keys = new List<string>();
            var uniqueKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataGridViewRow row in conditionGrid.Rows)
            {
                if (row.IsNewRow) continue;
                string key = Convert.ToString(
                    row.Cells[valueColumn.Index].Value)?.Trim();
                if (string.IsNullOrEmpty(key))
                {
                    MessageBox.Show(this, "每一行都必须填写" + _keyDisplayName + "。",
                        _keyDisplayName,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (key.IndexOfAny(new[] { ';', '\r', '\n' }) >= 0)
                {
                    MessageBox.Show(this, _keyDisplayName + "不能包含分号或换行。",
                        _keyDisplayName,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!string.IsNullOrWhiteSpace(_reservedKey) &&
                    string.Equals(key, _reservedKey, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(this,
                        _reservedKey + " 已由节点固定配置，不需要重复添加。",
                        _keyDisplayName,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!uniqueKeys.Add(key))
                {
                    MessageBox.Show(this, _keyDisplayName + "不能重复：" + key,
                        _keyDisplayName,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                keys.Add(key);
            }

            ComparisonOutputs = string.Join(";", keys.ToArray());
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
