using System;
using System.Globalization;
using System.Linq;
using FrmServices.Services.EditorServices;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public sealed class CharacterMergingNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public string FirstValue { get; set; }
        public string SecondValue { get; set; }
        public string Value { get; set; }
        public string Message { get; set; }
    }

    [STNode("数据处理", "xioa", null, null,
        "将两个输入转换为字符串，并按字符串 1 + 字符串 2 的顺序合并输出。")]
    public sealed class CharacterMergingNode : WorkflowNode,
        IEditorExecutableNode, IEditorNodeReadiness
    {
        public CharacterMergingNode()
        {
            SetNodeTypeTitle("字符合并");
            EnableExecutionLog = true;
            TitleColor = System.Drawing.Color.FromArgb(220, 157, 82, 151);
            LetGetOptions = true;

            FirstInput = InputOptions.Add("字符串 1", typeof(object), true);
            SecondInput = InputOptions.Add("字符串 2", typeof(object), true);
            Output = OutputOptions.Add("输出", typeof(object), false);
            FirstInput.DataTransfer += InputOnDataTransfer;
            SecondInput.DataTransfer += InputOnDataTransfer;
        }

        public STNodeOption FirstInput { get; }
        public STNodeOption SecondInput { get; }
        public STNodeOption Output { get; }

        public CharacterMergingNodeExecutionResult Execute()
        {
            try
            {
                string first = ConvertToString(FirstInput.Data);
                string second = ConvertToString(SecondInput.Data);
                string value = first + second;

                Output.Data = value;
                Output.TransferData();
                return new CharacterMergingNodeExecutionResult
                {
                    IsSuccess = true,
                    FirstValue = first,
                    SecondValue = second,
                    Value = value,
                    Message = "字符串已合并。"
                };
            }
            catch (Exception ex)
            {
                return new CharacterMergingNodeExecutionResult
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
            CharacterMergingNodeExecutionResult result = Execute();
            return result.IsSuccess
                ? EditorNodeExecutionResult.Success(result.Message, Output)
                : EditorNodeExecutionResult.Failure(result.Message);
        }

        public EditorNodeReadinessResult CanExecute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            bool firstReady = IsInputReady(context, FirstInput);
            bool secondReady = IsInputReady(context, SecondInput);
            if (firstReady && secondReady)
                return EditorNodeReadinessResult.Ready();
            if (!firstReady && !secondReady)
                return EditorNodeReadinessResult.NotReady(
                    "等待字符串 1 和字符串 2。");
            return EditorNodeReadinessResult.NotReady(
                firstReady ? "等待字符串 2。" : "等待字符串 1。");
        }

        private static bool IsInputReady(EditorExecutionContext context,
            STNodeOption input)
        {
            bool stringConstant = input.GetConnectedOption().Any(option =>
                option != null && option.Owner is StringNode);
            if (stringConstant || GlobalDataNode.IsReadSource(input)) return true;
            return input.Data != null && context.IsInputActivated(input);
        }

        private static string ConvertToString(object value)
        {
            if (value == null)
                throw new InvalidOperationException("字符合并输入不能为空。");
            return Convert.ToString(value, CultureInfo.InvariantCulture) ??
                   string.Empty;
        }

        private void InputOnDataTransfer(object sender, STNodeOptionEventArgs e)
        {
            var input = sender as STNodeOption;
            if (input == null) return;
            input.Data = e.Status == ConnectionStatus.Connected &&
                         e.TargetOption != null
                ? e.TargetOption.Data
                : null;
        }
    }
}
