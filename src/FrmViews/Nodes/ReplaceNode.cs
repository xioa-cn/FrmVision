using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using FrmServices.Services.EditorServices;
using FrmViews.Views;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    internal sealed class ReplaceItemDefinition
    {
        public string Find { get; set; }
        public string Replacement { get; set; }
    }

    internal static class ReplaceItemSerializer
    {
        public static string Serialize(IEnumerable<ReplaceItemDefinition> items)
        {
            var root = new XElement("ReplaceItems",
                (items ?? Enumerable.Empty<ReplaceItemDefinition>()).Select(item =>
                    new XElement("Item",
                        new XElement("Find", item.Find ?? string.Empty),
                        new XElement("Replacement", item.Replacement ?? string.Empty))));
            return root.ToString(SaveOptions.DisableFormatting);
        }

        public static List<ReplaceItemDefinition> Parse(string value)
        {
            var items = new List<ReplaceItemDefinition>();
            if (string.IsNullOrWhiteSpace(value)) return items;

            XElement root;
            try
            {
                root = XElement.Parse(value);
            }
            catch (Exception ex)
            {
                throw new FormatException("Replace 列表配置格式不正确。", ex);
            }

            if (!string.Equals(root.Name.LocalName, "ReplaceItems",
                    StringComparison.Ordinal))
                throw new FormatException("Replace 列表配置缺少 ReplaceItems 根节点。");

            int sequence = 0;
            foreach (XElement element in root.Elements("Item"))
            {
                sequence++;
                XElement findElement = element.Element("Find");
                XElement replacementElement = element.Element("Replacement");
                string find = findElement == null ? string.Empty : findElement.Value;
                if (find.Length == 0)
                    throw new FormatException("第 " + sequence + " 项的查找内容不能为空。");

                items.Add(new ReplaceItemDefinition
                {
                    Find = find,
                    Replacement = replacementElement == null
                        ? string.Empty
                        : replacementElement.Value
                });
            }
            return items;
        }

        public static string Display(string value)
        {
            if (string.IsNullOrEmpty(value)) return "空";
            string text = value.Replace("\r", "\\r").Replace("\n", "\\n")
                .Replace("\t", "\\t");
            if (text == " ") return "空格";
            return text.Length > 10 ? text.Substring(0, 10) + "..." : text;
        }
    }

    public sealed class ReplaceNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public string InputValue { get; set; }
        public string OutputValue { get; set; }
        public int ItemCount { get; set; }
        public string Message { get; set; }
    }

    [STNode("数据处理", "xioa", null, null,
        "按列表顺序替换字符串内容，并输出最终字符串。")]
    public sealed class ReplaceNode : WorkflowNode, IEditorExecutableNode,
        IEditorNodeReadiness
    {
        private readonly List<ReplaceItemDefinition> _items =
            new List<ReplaceItemDefinition>();
        private string _replaceItems;

        public ReplaceNode()
        {
            SetNodeTypeTitle("Replace工具");
            EnableExecutionLog = true;
            TitleColor = System.Drawing.Color.FromArgb(220, 112, 79, 145);
            LetGetOptions = true;
            Input = InputOptions.Add("输入", typeof(object), true);
            Output = OutputOptions.Add("输出", typeof(object), false);
            Input.DataTransfer += InputOnDataTransfer;
            ReplaceItems = ReplaceItemSerializer.Serialize(
                Enumerable.Empty<ReplaceItemDefinition>());
        }

        public STNodeOption Input { get; }
        public STNodeOption Output { get; }

        [STNodeProperty("Replace 列表", "按列表顺序执行查找和替换。",
            DescriptorType = typeof(ReplaceItemsPropertyDescriptor))]
        public string ReplaceItems
        {
            get { return _replaceItems; }
            set { SetReplaceItems(value); }
        }

        public ReplaceNodeExecutionResult Execute()
        {
            string input = null;
            try
            {
                if (Input.Data == null)
                    throw new InvalidOperationException("Replace 输入值不能为空。");

                input = Convert.ToString(Input.Data,
                    CultureInfo.InvariantCulture) ?? string.Empty;
                string result = input;
                foreach (ReplaceItemDefinition item in _items)
                    result = result.Replace(item.Find, item.Replacement);

                Output.Data = result;
                Output.TransferData();
                return new ReplaceNodeExecutionResult
                {
                    IsSuccess = true,
                    InputValue = input,
                    OutputValue = result,
                    ItemCount = _items.Count,
                    Message = "Replace 已完成，共执行 " + _items.Count + " 项。"
                };
            }
            catch (Exception ex)
            {
                return new ReplaceNodeExecutionResult
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
            ReplaceNodeExecutionResult result = Execute();
            return result.IsSuccess
                ? EditorNodeExecutionResult.Success(result.Message, Output)
                : EditorNodeExecutionResult.Failure(result.Message);
        }

        public EditorNodeReadinessResult CanExecute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            bool constantInput = Input.GetConnectedOption().Any(option =>
                option != null && option.Owner is StringNode);
            return constantInput || GlobalDataNode.IsReadSource(Input) ||
                   (Input.Data != null && context.IsInputActivated(Input))
                ? EditorNodeReadinessResult.Ready()
                : EditorNodeReadinessResult.NotReady("等待字符串输入。");
        }

        private void SetReplaceItems(string value)
        {
            List<ReplaceItemDefinition> items = ReplaceItemSerializer.Parse(value);
            _items.Clear();
            _items.AddRange(items);
            _replaceItems = ReplaceItemSerializer.Serialize(items);
        }

        private void InputOnDataTransfer(object sender, STNodeOptionEventArgs e)
        {
            Input.Data = e.Status == ConnectionStatus.Connected &&
                         e.TargetOption != null
                ? e.TargetOption.Data
                : null;
        }
    }

    public sealed class ReplaceItemsPropertyDescriptor : STNodePropertyDescriptor
    {
        protected override string GetStringFromValue()
        {
            var node = Node as ReplaceNode;
            if (node == null) return "未配置";
            try
            {
                List<ReplaceItemDefinition> items =
                    ReplaceItemSerializer.Parse(node.ReplaceItems);
                if (items.Count == 0) return "未配置";
                string summary = string.Join("；", items.Take(3).Select(item =>
                    ReplaceItemSerializer.Display(item.Find) + " -> " +
                    ReplaceItemSerializer.Display(item.Replacement)).ToArray());
                if (items.Count > 3) summary += " ...";
                return items.Count + " 项：" + summary;
            }
            catch { return "配置错误"; }
        }

        protected override byte[] GetBytesFromValue()
        {
            var node = Node as ReplaceNode;
            return Encoding.UTF8.GetBytes(node == null
                ? string.Empty
                : node.ReplaceItems ?? string.Empty);
        }

        protected override object GetValueFromBytes(byte[] byData)
        {
            return byData == null ? string.Empty : Encoding.UTF8.GetString(byData);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            var node = Node as ReplaceNode;
            if (node == null) return;
            using (var editor = new ReplaceItemsEditorFrm(node.ReplaceItems))
            {
                if (editor.ShowDialog(Control.FindForm()) == DialogResult.OK)
                    SetValue(editor.Items);
            }
        }
    }
}
