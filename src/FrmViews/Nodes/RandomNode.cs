using System;
using System.Globalization;
using System.Linq;
using FrmServices.Services.EditorServices;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public enum RandomNodeOperation
    {
        Direct,
        Add,
        Subtract,
        Multiply,
        Divide
    }

    public sealed class RandomNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public bool HasInput { get; set; }
        public double InputValue { get; set; }
        public double RandomValue { get; set; }
        public double ResultValue { get; set; }
        public RandomNodeOperation Operation { get; set; }
        public string Message { get; set; }
    }

    [STNode("数据处理", "xioa", null, null,
        "生成指定范围内的随机数；可选择与输入数字进行加、减、乘、除运算，" +
        "也可不参考输入直接输出随机数。")]
    public sealed class RandomNode : WorkflowNode, IEditorExecutableNode,
        IEditorNodeReadiness
    {
        private const int DivideRetryCount = 10;
        private static readonly Random RandomSource = new Random();
        private static readonly object RandomLock = new object();

        public RandomNode()
        {
            SetNodeTypeTitle("随机数");
            EnableExecutionLog = true;
            TitleColor = System.Drawing.Color.FromArgb(220, 200, 130, 63);
            LetGetOptions = true;

            Input = InputOptions.Add("数字", typeof(object), true);
            Output = OutputOptions.Add("结果", typeof(object), false);
            Input.DataTransfer += InputOnDataTransfer;
        }

        public STNodeOption Input { get; }
        public STNodeOption Output { get; }

        [STNodeProperty("最小值", "随机数范围的下限（含）。")]
        public double Minimum { get; set; } = 0D;

        [STNodeProperty("最大值", "随机数范围的上限（不含）。")]
        public double Maximum { get; set; } = 1D;

        [STNodeProperty("运算方式",
            "选择“直接输出随机数”时不参考输入；选择加、减、乘、除时按“输入 运算 随机数”计算。",
            DescriptorType = typeof(RandomNodeOperationPropertyDescriptor))]
        public RandomNodeOperation Operation { get; set; } =
            RandomNodeOperation.Direct;

        public RandomNodeExecutionResult Execute()
        {
            double inputValue = 0D;
            double randomValue = 0D;
            try
            {
                ValidateRange();

                bool hasInput = Operation != RandomNodeOperation.Direct &&
                                TryGetInputValue(out inputValue);
                randomValue = hasInput &&
                              Operation == RandomNodeOperation.Divide
                    ? NextNonZeroRandom()
                    : NextRandom();

                double result;
                if (hasInput)
                {
                    switch (Operation)
                    {
                        case RandomNodeOperation.Add:
                            result = inputValue + randomValue;
                            break;
                        case RandomNodeOperation.Subtract:
                            result = inputValue - randomValue;
                            break;
                        case RandomNodeOperation.Multiply:
                            result = inputValue * randomValue;
                            break;
                        case RandomNodeOperation.Divide:
                            result = inputValue / randomValue;
                            break;
                        default:
                            throw new InvalidOperationException(
                                "不支持的运算方式。");
                    }
                }
                else
                {
                    result = randomValue;
                }

                if (!IsFinite(result))
                    throw new OverflowException("计算结果超出有效数值范围。");

                Output.Data = result;
                Output.TransferData();
                return new RandomNodeExecutionResult
                {
                    IsSuccess = true,
                    HasInput = hasInput,
                    InputValue = inputValue,
                    RandomValue = randomValue,
                    ResultValue = result,
                    Operation = Operation,
                    Message = BuildSuccessMessage(hasInput, inputValue,
                        randomValue, result)
                };
            }
            catch (Exception ex)
            {
                return new RandomNodeExecutionResult
                {
                    IsSuccess = false,
                    HasInput = false,
                    InputValue = inputValue,
                    RandomValue = randomValue,
                    Operation = Operation,
                    Message = ex.GetBaseException().Message
                };
            }
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();
            RandomNodeExecutionResult result = Execute();
            return result.IsSuccess
                ? EditorNodeExecutionResult.Success(result.Message, Output)
                : EditorNodeExecutionResult.Failure(result.Message);
        }

        public EditorNodeReadinessResult CanExecute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (Operation == RandomNodeOperation.Direct ||
                IsInputReady(context) || !HasConnectedInput())
                return EditorNodeReadinessResult.Ready();
            return EditorNodeReadinessResult.NotReady("等待数字输入。");
        }

        private bool IsInputReady(EditorExecutionContext context)
        {
            bool constantInput = Input.GetConnectedOption().Any(option =>
                option != null && option.Owner is NumNode);
            if (constantInput || GlobalDataNode.IsReadSource(Input)) return true;
            return Input.Data != null && context.IsInputActivated(Input);
        }

        private bool HasConnectedInput()
        {
            return Input.GetConnectedOption().Any(option => option != null);
        }

        private void ValidateRange()
        {
            if (!IsFinite(Minimum) || !IsFinite(Maximum))
                throw new InvalidOperationException("随机范围必须是有限数值。");
            if (Minimum > Maximum)
                throw new InvalidOperationException(
                    "随机最小值不能大于随机最大值。");
        }

        private double NextRandom()
        {
            if (Minimum == Maximum) return Minimum;
            double sample;
            lock (RandomLock)
                sample = RandomSource.NextDouble();
            return Minimum + (Maximum - Minimum) * sample;
        }

        private double NextNonZeroRandom()
        {
            for (int i = 0; i < DivideRetryCount; i++)
            {
                double value = NextRandom();
                if (value != 0D) return value;
            }
            throw new DivideByZeroException("随机数不能为零，请调整随机范围。");
        }

        private bool TryGetInputValue(out double value)
        {
            value = 0D;
            object data = Input.Data;
            if (data == null) return false;

            string text = data as string;
            if (text != null)
            {
                return double.TryParse(text.Trim(),
                    NumberStyles.Float | NumberStyles.AllowThousands,
                    CultureInfo.InvariantCulture, out value) && IsFinite(value);
            }

            try
            {
                value = Convert.ToDouble(data, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return false;
            }
            return IsFinite(value);
        }

        private string BuildSuccessMessage(bool hasInput, double inputValue,
            double randomValue, double result)
        {
            if (!hasInput)
            {
                return Operation == RandomNodeOperation.Direct
                    ? "已生成随机数 " + Format(randomValue) + "。"
                    : "未提供有效数字输入，已直接输出随机数 " +
                      Format(randomValue) + "。";
            }

            string symbol = GetSymbol(Operation);
            return "已生成随机数 " + Format(randomValue) + "，计算 " +
                   Format(inputValue) + " " + symbol + " " +
                   Format(randomValue) + " = " + Format(result) + "。";
        }

        private static string GetSymbol(RandomNodeOperation operation)
        {
            switch (operation)
            {
                case RandomNodeOperation.Add: return "+";
                case RandomNodeOperation.Subtract: return "-";
                case RandomNodeOperation.Multiply: return "×";
                case RandomNodeOperation.Divide: return "÷";
                default: return "?";
            }
        }

        private static string Format(double value)
        {
            return value.ToString("G15", CultureInfo.InvariantCulture);
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
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

    public sealed class RandomNodeOperationPropertyDescriptor :
        STNodePropertyDescriptor
    {
        protected override object GetValueFromString(string text)
        {
            string normalized = Normalize(text);
            switch (normalized)
            {
                case "直接输出":
                case "直接输出随机数":
                case "不参考输入":
                case "无":
                case "Direct":
                case "None":
                    return RandomNodeOperation.Direct;
                case "输入+随机数":
                case "+":
                case "加":
                case "Add":
                    return RandomNodeOperation.Add;
                case "输入-随机数":
                case "-":
                case "减":
                case "Subtract":
                    return RandomNodeOperation.Subtract;
                case "输入×随机数":
                case "输入*随机数":
                case "输入x随机数":
                case "×":
                case "x":
                case "*":
                case "乘":
                case "Multiply":
                    return RandomNodeOperation.Multiply;
                case "输入÷随机数":
                case "输入/随机数":
                case "÷":
                case "/":
                case "除":
                case "Divide":
                    return RandomNodeOperation.Divide;
                default:
                    return Enum.Parse(typeof(RandomNodeOperation), normalized,
                        true);
            }
        }

        private static string Normalize(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Trim()
                .Replace(" ", string.Empty)
                .Replace("\t", string.Empty)
                .Replace("\u3000", string.Empty);
        }

        protected override string GetStringFromValue()
        {
            return GetText((RandomNodeOperation)GetValue(null));
        }

        protected override string GetSelectItemText(object value)
        {
            return GetText((RandomNodeOperation)value);
        }

        private static string GetText(RandomNodeOperation operation)
        {
            switch (operation)
            {
                case RandomNodeOperation.Direct: return "直接输出随机数";
                case RandomNodeOperation.Add: return "输入 + 随机数";
                case RandomNodeOperation.Subtract: return "输入 - 随机数";
                case RandomNodeOperation.Multiply: return "输入 × 随机数";
                case RandomNodeOperation.Divide: return "输入 ÷ 随机数";
                default: return "?";
            }
        }
    }
}
