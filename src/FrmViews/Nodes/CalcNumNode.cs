using System;
using System.Globalization;
using System.Linq;
using FrmServices.Services.EditorServices;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public enum CalcNumOperation
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    public sealed class CalcNumNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public double LeftValue { get; set; }
        public double RightValue { get; set; }
        public double ResultValue { get; set; }
        public CalcNumOperation Operation { get; set; }
        public string Message { get; set; }
    }

    [STNode("数据处理", "xioa", null, null,
        "对两个数字执行加、减、乘、除运算，并输出计算结果。")]
    public sealed class CalcNumNode : WorkflowNode, IEditorExecutableNode,
        IEditorNodeReadiness
    {
        public CalcNumNode()
        {
            SetNodeTypeTitle("数值计算");
            EnableExecutionLog = true;
            TitleColor = System.Drawing.Color.FromArgb(220, 62, 130, 150);
            LetGetOptions = true;

            LeftInput = InputOptions.Add("数字 1", typeof(object), true);
            RightInput = InputOptions.Add("数字 2", typeof(object), true);
            Output = OutputOptions.Add("结果", typeof(object), false);
            LeftInput.DataTransfer += InputOnDataTransfer;
            RightInput.DataTransfer += InputOnDataTransfer;
        }

        public STNodeOption LeftInput { get; }
        public STNodeOption RightInput { get; }
        public STNodeOption Output { get; }

        [STNodeProperty("计算方式", "选择 +、-、× 或 ÷。",
            DescriptorType = typeof(CalcNumOperationPropertyDescriptor))]
        public CalcNumOperation Operation { get; set; } = CalcNumOperation.Add;

        public CalcNumNodeExecutionResult Execute()
        {
            double left = 0D;
            double right = 0D;
            try
            {
                if (LeftInput.Data == null || RightInput.Data == null)
                    throw new InvalidOperationException("两个数字输入都不能为空。");

                left = Convert.ToDouble(LeftInput.Data,
                    CultureInfo.InvariantCulture);
                right = Convert.ToDouble(RightInput.Data,
                    CultureInfo.InvariantCulture);
                if (double.IsNaN(left) || double.IsInfinity(left) ||
                    double.IsNaN(right) || double.IsInfinity(right))
                    throw new InvalidOperationException("输入必须是有限数值。");

                double result;
                switch (Operation)
                {
                    case CalcNumOperation.Add:
                        result = left + right;
                        break;
                    case CalcNumOperation.Subtract:
                        result = left - right;
                        break;
                    case CalcNumOperation.Multiply:
                        result = left * right;
                        break;
                    case CalcNumOperation.Divide:
                        if (right == 0D)
                            throw new DivideByZeroException("除数不能为零。");
                        result = left / right;
                        break;
                    default:
                        throw new InvalidOperationException("不支持的计算方式。");
                }

                if (double.IsNaN(result) || double.IsInfinity(result))
                    throw new OverflowException("计算结果超出有效数值范围。");

                Output.Data = result;
                Output.TransferData();
                return new CalcNumNodeExecutionResult
                {
                    IsSuccess = true,
                    LeftValue = left,
                    RightValue = right,
                    ResultValue = result,
                    Operation = Operation,
                    Message = "数值计算完成：" + left.ToString("G15",
                        CultureInfo.InvariantCulture) + " " + GetSymbol(Operation) +
                        " " + right.ToString("G15", CultureInfo.InvariantCulture) +
                        " = " + result.ToString("G15", CultureInfo.InvariantCulture) + "。"
                };
            }
            catch (Exception ex)
            {
                return new CalcNumNodeExecutionResult
                {
                    IsSuccess = false,
                    LeftValue = left,
                    RightValue = right,
                    Operation = Operation,
                    Message = ex.GetBaseException().Message
                };
            }
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();
            CalcNumNodeExecutionResult result = Execute();
            return result.IsSuccess
                ? EditorNodeExecutionResult.Success(result.Message, Output)
                : EditorNodeExecutionResult.Failure(result.Message);
        }

        public EditorNodeReadinessResult CanExecute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            bool leftReady = IsInputReady(context, LeftInput);
            bool rightReady = IsInputReady(context, RightInput);
            if (leftReady && rightReady)
                return EditorNodeReadinessResult.Ready();
            if (!leftReady && !rightReady)
                return EditorNodeReadinessResult.NotReady("等待数字 1 和数字 2。");
            return EditorNodeReadinessResult.NotReady(
                leftReady ? "等待数字 2。" : "等待数字 1。");
        }

        private static bool IsInputReady(EditorExecutionContext context,
            STNodeOption input)
        {
            bool constantInput = input.GetConnectedOption().Any(option =>
                option != null && option.Owner is NumNode);
            if (constantInput || GlobalDataNode.IsReadSource(input)) return true;
            return input.Data != null && context.IsInputActivated(input);
        }

        private static string GetSymbol(CalcNumOperation operation)
        {
            switch (operation)
            {
                case CalcNumOperation.Add: return "+";
                case CalcNumOperation.Subtract: return "-";
                case CalcNumOperation.Multiply: return "×";
                case CalcNumOperation.Divide: return "÷";
                default: return "?";
            }
        }

        private static void InputOnDataTransfer(object sender,
            STNodeOptionEventArgs e)
        {
            var input = sender as STNodeOption;
            if (input == null) return;
            input.Data = e.Status == ConnectionStatus.Connected &&
                         e.TargetOption != null
                ? e.TargetOption.Data
                : null;
        }
    }

    public sealed class CalcNumOperationPropertyDescriptor :
        STNodePropertyDescriptor
    {
        protected override object GetValueFromString(string text)
        {
            switch ((text ?? string.Empty).Trim())
            {
                case "+": return CalcNumOperation.Add;
                case "-": return CalcNumOperation.Subtract;
                case "×":
                case "x":
                case "*": return CalcNumOperation.Multiply;
                case "÷":
                case "/": return CalcNumOperation.Divide;
                default: return Enum.Parse(typeof(CalcNumOperation), text, true);
            }
        }

        protected override string GetStringFromValue()
        {
            return GetSymbol((CalcNumOperation)GetValue(null));
        }

        protected override string GetSelectItemText(object value)
        {
            return GetSymbol((CalcNumOperation)value);
        }

        private static string GetSymbol(CalcNumOperation operation)
        {
            switch (operation)
            {
                case CalcNumOperation.Add: return "+";
                case CalcNumOperation.Subtract: return "-";
                case CalcNumOperation.Multiply: return "×";
                case CalcNumOperation.Divide: return "÷";
                default: return "?";
            }
        }
    }
}
