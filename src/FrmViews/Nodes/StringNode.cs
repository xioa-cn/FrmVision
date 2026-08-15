using System;
using FrmServices.Services.EditorServices;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    [STNode("数据源", "xioa", null, null,
        "输出一个固定字符串，供后续节点使用。")]
    public sealed class StringNode : WorkflowNode, IEditorExecutableNode
    {
        private string _value = string.Empty;

        public StringNode()
        {
            SetNodeTypeTitle("字符常量");
            EnableExecutionLog = true;
            TitleColor = System.Drawing.Color.FromArgb(220, 49, 111, 151);
            LetGetOptions = true;
            Output = OutputOptions.Add("输出", typeof(object), false);
            Output.Data = _value;
        }

        public STNodeOption Output { get; }

        [STNodeProperty("字符串", "每轮流程执行时输出的固定字符串。")]
        public string Value
        {
            get => _value;
            set
            {
                _value = value ?? string.Empty;
                Output.Data = _value;
                Output.TransferData();
            }
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();

            Output.Data = Value;
            Output.TransferData();
            return EditorNodeExecutionResult.Success(
                "已输出字符串“" + Value + "”。", Output);
        }
    }
}
