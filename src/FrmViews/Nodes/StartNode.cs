using ST.Library.UI.NodeEditor;

using FrmServices.Services.EditorServices;

namespace FrmViews.Nodes
{
    [STNode("流程控制", "xioa", null, null,
        "流程的起始节点，只提供可连接多个后续节点的输出端口。")]
    public class StartNode : WorkflowNode, IEditorExecutableNode, IEditorStartNode
    {
        public StartNode()
        {
            SetNodeTypeTitle("开始");
            TitleColor = System.Drawing.Color.FromArgb(220, 21, 146, 78);
            LetGetOptions = true;
            Output = OutputOptions.Add("输出", typeof(object), false);
        }

        public STNodeOption Output { get; }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new System.ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();
            Output.Data = new EditorFlowSignal(context.ExecutionId);
            Output.TransferData();
            return EditorNodeExecutionResult.Success("流程已从开始节点启动。", Output);
        }
    }
}
