using System;
using FrmServices.Services.EditorServices;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    [STNode("数据源", "xioa", null, null,
        "输出一个固定数值，供后续节点使用。")]
    public sealed class NumNode : WorkflowNode, IEditorExecutableNode
    {
        private double _value;

        public NumNode()
        {
            SetNodeTypeTitle("数值常量");
            EnableExecutionLog = true;
            TitleColor = System.Drawing.Color.FromArgb(220, 125, 91, 170);
            LetGetOptions = true;
            Output = OutputOptions.Add("输出", typeof(object), false);
            Output.Data = _value;
        }

        public STNodeOption Output { get; }

        [STNodeProperty("数值", "每轮流程执行时输出的固定数值。")]
        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                Output.Data = value;
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
                "已输出数值 " + Value.ToString("G15") + "。", Output);
        }
    }
}
