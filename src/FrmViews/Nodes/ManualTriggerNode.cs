using System;
using FrmServices.Services.EditorServices;
using FrmServices.Utils;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    [STNode("流程控制", "xioa", null, null,
        "读取指定的手动触发状态；状态为 True 时继续执行下一个节点。")]
    public sealed class ManualTriggerNode : WorkflowNode, IEditorExecutableNode,
        IEditorNodeReadiness
    {
        public ManualTriggerNode()
        {
            SetNodeTypeTitle("手动触发");
            EnableExecutionLog = true;
            TitleColor = System.Drawing.Color.FromArgb(220, 37, 135, 170);
            LetGetOptions = true;
            Input = InputOptions.Add("输入", typeof(object), true);
            Output = OutputOptions.Add("输出", typeof(object), false);
            Input.DataTransfer += InputOnDataTransfer;
        }

        public STNodeOption Input { get; }
        public STNodeOption Output { get; }

        [STNodeProperty("触发名称", "ManualTriggerUtils 中手动触发状态对应的名称。")]
        public string TriggerName { get; set; } = string.Empty;

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            bool shouldContinue = false;
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();

            string triggerName = (TriggerName ?? string.Empty).Trim();
            if (triggerName.Length == 0)
                return EditorNodeExecutionResult.Failure("触发名称不能为空。");
            try
            {
                shouldContinue = ManualTriggerUtils.GetTrigger(triggerName);
                if (!shouldContinue)
                    return EditorNodeExecutionResult.Success(
                        "手动触发状态为 False，当前分支停止执行。");

                Output.Data = Input.Data;
                Output.TransferData();
                return EditorNodeExecutionResult.Success(
                    "手动触发状态为 True，继续执行下一个节点。", Output);
            }
            finally
            {
                if (shouldContinue)
                {
                    ManualTriggerUtils.SetTrigger(triggerName, false);
                }
            }
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
