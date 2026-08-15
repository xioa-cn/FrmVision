using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using FrmServices.Services.EditorServices;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public enum ComparisonOperator
    {
        GreaterThan,
        Equal,
        LessThan
    }

    public enum ComparisonDataType
    {
        Number,
        String
    }

    public sealed class ComparisonNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public bool IsMatch { get; set; }
        public object LeftValue { get; set; }
        public object RightValue { get; set; }
        public ComparisonOperator Operator { get; set; }
        public ComparisonDataType DataType { get; set; }
        public string Message { get; set; }
    }

    [STNode("逻辑判断", "xioa", null, null,
        "比较两个输入值，并根据比较结果从 True 或 False 端口继续流程。")]
    public sealed class ComparisonNode : WorkflowNode, IEditorExecutableNode,
        IEditorNodeReadiness
    {
        public ComparisonNode()
        {
            SetNodeTypeTitle("比较工具");
            EnableExecutionLog = true;
            TitleColor = Color.FromArgb(220, 190, 62, 72);
            LetGetOptions = true;

            LeftInput = InputOptions.Add("输入 1", typeof(object), true);
            RightInput = InputOptions.Add("输入 2", typeof(object), true);
            TrueOutput = OutputOptions.Add("True", typeof(object), false);
            FalseOutput = OutputOptions.Add("False", typeof(object), false);
            LeftInput.DataTransfer += InputOnDataTransfer;
            RightInput.DataTransfer += InputOnDataTransfer;
        }

        public STNodeOption LeftInput { get; }
        public STNodeOption RightInput { get; }
        public STNodeOption TrueOutput { get; }
        public STNodeOption FalseOutput { get; }

        [STNodeProperty("比较方式", "选择 >、= 或 <。",
            DescriptorType = typeof(ComparisonOperatorPropertyDescriptor))]
        public ComparisonOperator Operator { get; set; } = ComparisonOperator.Equal;

        [STNodeProperty("数据类型", "Number 按数值比较，String 按字符串比较。")]
        public ComparisonDataType DataType { get; set; } = ComparisonDataType.Number;

        public ComparisonNodeExecutionResult Execute()
        {
            try
            {
                if (LeftInput.Data == null || RightInput.Data == null)
                    throw new InvalidOperationException("两个比较输入都不能为空。");

                int comparison = Compare(LeftInput.Data, RightInput.Data);
                bool isMatch = IsMatch(comparison);
                STNodeOption activeOutput = isMatch ? TrueOutput : FalseOutput;
                activeOutput.Data = isMatch;
                activeOutput.TransferData();

                return new ComparisonNodeExecutionResult
                {
                    IsSuccess = true,
                    IsMatch = isMatch,
                    LeftValue = LeftInput.Data,
                    RightValue = RightInput.Data,
                    Operator = Operator,
                    DataType = DataType,
                    Message = "比较结果为 " + (isMatch ? "True" : "False") + "。"
                };
            }
            catch (Exception ex)
            {
                return new ComparisonNodeExecutionResult
                {
                    IsSuccess = false,
                    LeftValue = LeftInput.Data,
                    RightValue = RightInput.Data,
                    Operator = Operator,
                    DataType = DataType,
                    Message = ex.GetBaseException().Message
                };
            }
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();
            ComparisonNodeExecutionResult result = Execute();
            if (!result.IsSuccess)
                return EditorNodeExecutionResult.Failure(result.Message);

            return EditorNodeExecutionResult.Success(result.Message,
                result.IsMatch ? TrueOutput : FalseOutput);
        }

        public EditorNodeReadinessResult CanExecute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            bool leftReady = IsInputReady(context, LeftInput);
            bool rightReady = IsInputReady(context, RightInput);
            if (leftReady && rightReady)
                return EditorNodeReadinessResult.Ready();

            if (!leftReady && !rightReady)
                return EditorNodeReadinessResult.NotReady("等待输入 1 和输入 2。");
            return EditorNodeReadinessResult.NotReady(
                leftReady ? "等待输入 2。" : "等待输入 1。");
        }

        private static bool IsInputReady(EditorExecutionContext context,
            STNodeOption input)
        {
            bool constantInput = input.GetConnectedOption().Any(option =>
                option != null &&
                (option.Owner is NumNode || option.Owner is StringNode));
            if (constantInput || GlobalDataNode.IsReadSource(input)) return true;
            return input.Data != null && context.IsInputActivated(input);
        }

        private int Compare(object left, object right)
        {
            if (DataType == ComparisonDataType.Number)
            {
                decimal leftNumber = Convert.ToDecimal(left,
                    CultureInfo.InvariantCulture);
                decimal rightNumber = Convert.ToDecimal(right,
                    CultureInfo.InvariantCulture);
                return leftNumber.CompareTo(rightNumber);
            }

            string leftText = Convert.ToString(left,
                CultureInfo.InvariantCulture) ?? string.Empty;
            string rightText = Convert.ToString(right,
                CultureInfo.InvariantCulture) ?? string.Empty;
            return StringComparer.Ordinal.Compare(leftText, rightText);
        }

        private bool IsMatch(int comparison)
        {
            switch (Operator)
            {
                case ComparisonOperator.GreaterThan: return comparison > 0;
                case ComparisonOperator.Equal: return comparison == 0;
                case ComparisonOperator.LessThan: return comparison < 0;
                default: throw new InvalidOperationException("不支持的比较方式。");
            }
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

    public sealed class ComparisonOperatorPropertyDescriptor :
        STNodePropertyDescriptor
    {
        protected override object GetValueFromString(string text)
        {
            switch ((text ?? string.Empty).Trim())
            {
                case ">": return ComparisonOperator.GreaterThan;
                case "=": return ComparisonOperator.Equal;
                case "<": return ComparisonOperator.LessThan;
                default: return Enum.Parse(typeof(ComparisonOperator), text, true);
            }
        }

        protected override string GetStringFromValue()
        {
            return GetSymbol((ComparisonOperator)GetValue(null));
        }

        protected override string GetSelectItemText(object value)
        {
            return GetSymbol((ComparisonOperator)value);
        }

        private static string GetSymbol(ComparisonOperator value)
        {
            switch (value)
            {
                case ComparisonOperator.GreaterThan: return ">";
                case ComparisonOperator.Equal: return "=";
                case ComparisonOperator.LessThan: return "<";
                default: return "?";
            }
        }
    }
}
