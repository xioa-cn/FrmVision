using System;
using FrmServices.Services.EditorServices;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public sealed class NegateNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public bool InputValue { get; set; }
        public bool Value { get; set; }
        public string Message { get; set; }
    }

    [STNode("逻辑判断", "xioa", null, null,
        "对一个 bool 输入值取反，并输出取反后的 bool 值。")]
    public sealed class NegateNode : WorkflowNode, IEditorExecutableNode,
        IEditorNodeReadiness
    {
        public NegateNode()
        {
            SetNodeTypeTitle("布尔取反");
            EnableExecutionLog = true;
            TitleColor = System.Drawing.Color.FromArgb(220, 217, 119, 6);
            LetGetOptions = true;
            Input = InputOptions.Add("输入", typeof(object), true);
            Output = OutputOptions.Add("输出", typeof(object), false);
            Input.DataTransfer += InputOnDataTransfer;
        }

        public STNodeOption Input { get; }
        public STNodeOption Output { get; }

        public NegateNodeExecutionResult Execute()
        {
            try
            {
                if (!(Input.Data is bool inputValue))
                    throw new InvalidOperationException("输入必须是 bool 值。");

                bool value = !inputValue;
                Output.Data = value;
                Output.TransferData();
                return new NegateNodeExecutionResult
                {
                    IsSuccess = true,
                    InputValue = inputValue,
                    Value = value,
                    Message = "布尔值已取反：" + inputValue + " -> " + value + "。"
                };
            }
            catch (Exception ex)
            {
                return new NegateNodeExecutionResult
                {
                    IsSuccess = false,
                    Message = ex.GetBaseException().Message
                };
            }
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();

            NegateNodeExecutionResult result = Execute();
            return result.IsSuccess
                ? EditorNodeExecutionResult.Success(result.Message, Output)
                : EditorNodeExecutionResult.Failure(result.Message);
        }

        public EditorNodeReadinessResult CanExecute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return (context.IsInputActivated(Input) ||
                    GlobalDataNode.IsReadSource(Input))
                ? EditorNodeReadinessResult.Ready()
                : EditorNodeReadinessResult.NotReady("等待本轮 bool 输入。");
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
