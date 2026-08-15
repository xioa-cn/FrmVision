using System;
using FrmServices.Services.EditorServices;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    [STNode("流程控制", "xioa", null, null,
        "将本轮输入原样输出，用于让下游节点比同级分支晚一拍执行。")]
    public sealed class WaitingRhythmNode : WorkflowNode, IEditorExecutableNode,
        IEditorNodeReadiness
    {
        public WaitingRhythmNode()
        {
            SetNodeTypeTitle("空拍节点");
            TitleColor = System.Drawing.Color.FromArgb(220, 96, 105, 118);
            LetGetOptions = true;
            Input = InputOptions.Add("输入", typeof(object), true);
            Output = OutputOptions.Add("输出", typeof(object), false);
            Input.DataTransfer += InputOnDataTransfer;
        }

        public STNodeOption Input { get; }
        public STNodeOption Output { get; }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();

            Output.Data = Input.Data;
            Output.TransferData();
            return EditorNodeExecutionResult.Success(
                "本轮输入已原样传递，下游节点延后一拍执行。", Output);
        }

        public EditorNodeReadinessResult CanExecute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return context.IsInputActivated(Input)
                ? EditorNodeReadinessResult.Ready()
                : EditorNodeReadinessResult.NotReady("等待本轮输入信号。");
        }

        private void InputOnDataTransfer(object sender, STNodeOptionEventArgs e)
        {
            Input.Data = e.Status == ConnectionStatus.Connected &&
                         e.TargetOption != null
                ? e.TargetOption.Data
                : null;
        }
    }
}
