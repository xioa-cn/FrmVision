using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using FrmServices.Services.EditorServices;
using FrmViews.Controls;
using FrmViews.Views;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    internal static class TrimItemSerializer
    {
        public static string Serialize(IEnumerable<string> items)
        {
            var root = new XElement("TrimItems",
                (items ?? Enumerable.Empty<string>()).Select(item =>
                    new XElement("Item", item ?? string.Empty)));
            return root.ToString(SaveOptions.DisableFormatting);
        }

        public static List<string> Parse(string value)
        {
            var items = new List<string>();
            if (string.IsNullOrWhiteSpace(value)) return items;

            XElement root;
            try
            {
                root = XElement.Parse(value);
            }
            catch
            {
                // Accept a simple semicolon/newline list for hand-edited values.
                items.AddRange(value.Replace("\r\n", "\n")
                    .Split(new[] { '\n', ';' }, StringSplitOptions.RemoveEmptyEntries));
                return items;
            }

            if (!string.Equals(root.Name.LocalName, "TrimItems",
                    StringComparison.Ordinal))
                throw new FormatException("Trim 列表配置缺少 TrimItems 根节点。");

            foreach (XElement item in root.Elements("Item"))
            {
                string text = item.Value ?? string.Empty;
                if (text.Length > 0) items.Add(text);
            }
            return items;
        }

        public static string Display(string value)
        {
            if (string.IsNullOrEmpty(value)) return "空";
            string text = value.Replace("\r", "\\r").Replace("\n", "\\n")
                .Replace("\t", "\\t");
            if (text == " ") return "空格";
            return text.Length > 12 ? text.Substring(0, 12) + "..." : text;
        }
    }

    public sealed class TrimNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public string InputValue { get; set; }
        public string OutputValue { get; set; }
        public int ItemCount { get; set; }
        public string Message { get; set; }
    }

    [STNode("数据处理", "xioa", null, null,
        "按列表顺序从字符串首尾移除指定字符，并输出处理后的字符串。")]
    public sealed class TrimNode : WorkflowNode, IEditorExecutableNode,
        IEditorNodeReadiness
    {
        private readonly List<string> _items = new List<string>();
        private string _trimItems;

        public TrimNode()
        {
            SetNodeTypeTitle("Trim工具");
            EnableExecutionLog = true;
            TitleColor = System.Drawing.Color.FromArgb(220, 74, 120, 86);
            LetGetOptions = true;
            Input = InputOptions.Add("输入", typeof(object), true);
            Output = OutputOptions.Add("输出", typeof(object), false);
            Input.DataTransfer += InputOnDataTransfer;
            TrimItems = TrimItemSerializer.Serialize(new[] { " " });
        }

        public STNodeOption Input { get; }
        public STNodeOption Output { get; }

        [STNodeProperty("Trim 列表", "按列表顺序从字符串首尾移除字符。",
            DescriptorType = typeof(TrimItemsPropertyDescriptor))]
        public string TrimItems
        {
            get { return _trimItems; }
            set { SetTrimItems(value); }
        }

        public TrimNodeExecutionResult Execute()
        {
            string input = null;
            try
            {
                if (Input.Data == null)
                    throw new InvalidOperationException("Trim 输入值不能为空。");
                input = Convert.ToString(Input.Data,
                    System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
                string result = input;
                foreach (string item in _items)
                    if (!string.IsNullOrEmpty(item)) result = result.Trim(item.ToCharArray());

                Output.Data = result;
                Output.TransferData();
                return new TrimNodeExecutionResult
                {
                    IsSuccess = true,
                    InputValue = input,
                    OutputValue = result,
                    ItemCount = _items.Count,
                    Message = "Trim 已完成，共执行 " + _items.Count + " 项。"
                };
            }
            catch (Exception ex)
            {
                return new TrimNodeExecutionResult
                {
                    IsSuccess = false,
                    InputValue = input,
                    ItemCount = _items.Count,
                    Message = ex.GetBaseException().Message
                };
            }
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();
            TrimNodeExecutionResult result = Execute();
            return result.IsSuccess
                ? EditorNodeExecutionResult.Success(result.Message, Output)
                : EditorNodeExecutionResult.Failure(result.Message);
        }

        public EditorNodeReadinessResult CanExecute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            bool constantInput = Input.GetConnectedOption().Any(option =>
                option != null && option.Owner is StringNode);
            return (constantInput || GlobalDataNode.IsReadSource(Input) ||
                    (Input.Data != null && context.IsInputActivated(Input)))
                ? EditorNodeReadinessResult.Ready()
                : EditorNodeReadinessResult.NotReady("等待字符串输入。");
        }

        private void SetTrimItems(string value)
        {
            List<string> items = TrimItemSerializer.Parse(value);
            _items.Clear();
            _items.AddRange(items.Where(item => !string.IsNullOrEmpty(item)));
            _trimItems = TrimItemSerializer.Serialize(_items);
        }

        private void InputOnDataTransfer(object sender, STNodeOptionEventArgs e)
        {
            Input.Data = e.Status == ConnectionStatus.Connected && e.TargetOption != null
                ? e.TargetOption.Data
                : null;
        }
    }

    public sealed class TrimItemsPropertyDescriptor : STNodePropertyDescriptor
    {
        protected override string GetStringFromValue()
        {
            var node = Node as TrimNode;
            if (node == null) return "未配置";
            try
            {
                List<string> items = TrimItemSerializer.Parse(node.TrimItems);
                if (items.Count == 0) return "未配置";
                string summary = string.Join(" -> ", items.Take(4)
                    .Select(TrimItemSerializer.Display).ToArray());
                if (items.Count > 4) summary += " ...";
                return items.Count + " 项：" + summary;
            }
            catch { return "配置错误"; }
        }

        protected override byte[] GetBytesFromValue()
        {
            var node = Node as TrimNode;
            return Encoding.UTF8.GetBytes(node == null ? string.Empty : node.TrimItems ?? string.Empty);
        }

        protected override object GetValueFromBytes(byte[] byData)
        {
            return byData == null ? string.Empty : Encoding.UTF8.GetString(byData);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            var node = Node as TrimNode;
            if (node == null) return;
            using (var editor = new TrimItemsEditorFrm(node.TrimItems))
            {
                if (editor.ShowDialog(Control.FindForm()) == DialogResult.OK)
                    SetValue(editor.Items);
            }
        }
    }
}
