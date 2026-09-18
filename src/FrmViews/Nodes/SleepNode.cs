using System;
using System.Globalization;
using FrmServices.Services.EditorServices;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    [STNode("流程控制", "xioa", null, null,
        "等待指定的毫秒数后，将本轮输入原样输出给下一个节点。")]
    // 作者：xioa
    // 作者邮箱：1327916255@qq.com
    public sealed class SleepNode : WorkflowNode, IEditorExecutableNode,
        IEditorNodeReadiness
    {
        private const int MaximumDelayMilliseconds = 3600000;
        private int _delayMilliseconds = 1000;

        public SleepNode()
        {
            SetNodeTypeTitle("延时节点");
            EnableExecutionLog = true;
            TitleColor = System.Drawing.Color.FromArgb(220, 142, 110, 60);
            LetGetOptions = true;
            Input = InputOptions.Add("输入", typeof(object), true);
            Output = OutputOptions.Add("输出", typeof(object), false);
            Input.DataTransfer += InputOnDataTransfer;
        }

        public STNodeOption Input { get; }
        public STNodeOption Output { get; }

        [STNodeProperty("延时时间", "等待的毫秒数；0 表示不等待，最大 3600000。")]
        public int DelayMilliseconds
        {
            get => _delayMilliseconds;
            set
            {
                if (value < 0 || value > MaximumDelayMilliseconds)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        "延时时间必须在 0 到 " + MaximumDelayMilliseconds +
                        " 毫秒之间。");
                _delayMilliseconds = value;
            }
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();

            int delayMilliseconds = DelayMilliseconds;
            if (delayMilliseconds > 0 &&
                context.CancellationToken.WaitHandle.WaitOne(delayMilliseconds))
                throw new OperationCanceledException(context.CancellationToken);

            Output.Data = Input.Data;
            Output.TransferData();
            return EditorNodeExecutionResult.Success(
                "已延时 " + delayMilliseconds.ToString(CultureInfo.InvariantCulture) +
                " 毫秒，本轮输入已原样传递。", Output);
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
