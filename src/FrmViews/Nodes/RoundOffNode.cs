using System;
using System.Globalization;
using System.Linq;
using FrmServices.Services.EditorServices;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public enum RoundOffMode
    {
        Ceiling,
        Floor
    }

    public sealed class RounOffNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public double InputValue { get; set; }
        public double ResultValue { get; set; }
        public RoundOffMode Mode { get; set; }
        public string Message { get; set; }
    }

    [STNode("数据处理", "xioa", null, null,
        "把输入数值取整后输出，可选择向上取整或向下取整。")]
    public sealed class RoundOffNode : WorkflowNode, IEditorExecutableNode,
        IEditorNodeReadiness
    {
        public RoundOffNode()
        {
            SetNodeTypeTitle("取整");
            EnableExecutionLog = true;
            TitleColor = System.Drawing.Color.FromArgb(220, 92, 124, 158);
            LetGetOptions = true;

            Input = InputOptions.Add("数字", typeof(object), true);
            Output = OutputOptions.Add("结果", typeof(object), false);
            Input.DataTransfer += InputOnDataTransfer;
        }

        public STNodeOption Input { get; }
        public STNodeOption Output { get; }

        [STNodeProperty("取整方式", "选择向上取整或向下取整。",
            DescriptorType = typeof(RoundOffModePropertyDescriptor))]
        public RoundOffMode Mode { get; set; } = RoundOffMode.Ceiling;

        public RounOffNodeExecutionResult Execute()
        {
            double inputValue = 0D;
            try
            {
                if (Input.Data == null)
                    throw new InvalidOperationException("数字输入不能为空。");
                if (!TryGetInputValue(out inputValue))
                    throw new InvalidOperationException("输入必须是有效数字。");

                double result = Mode == RoundOffMode.Ceiling
                    ? Math.Ceiling(inputValue)
                    : Math.Floor(inputValue);

                Output.Data = result;
                Output.TransferData();
                return new RounOffNodeExecutionResult
                {
                    IsSuccess = true,
                    InputValue = inputValue,
                    ResultValue = result,
                    Mode = Mode,
                    Message = "已" + GetText(Mode) + "：" +
                              Format(inputValue) + " -> " + Format(result) +
                              "。"
                };
            }
            catch (Exception ex)
            {
                return new RounOffNodeExecutionResult
                {
                    IsSuccess = false,
                    InputValue = inputValue,
                    Mode = Mode,
                    Message = ex.GetBaseException().Message
                };
            }
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();
            RounOffNodeExecutionResult result = Execute();
            return result.IsSuccess
                ? EditorNodeExecutionResult.Success(result.Message, Output)
                : EditorNodeExecutionResult.Failure(result.Message);
        }

        public EditorNodeReadinessResult CanExecute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            bool constantInput = Input.GetConnectedOption().Any(option =>
                option != null && option.Owner is NumNode);
            if (constantInput || GlobalDataNode.IsReadSource(Input) ||
                (Input.Data != null && context.IsInputActivated(Input)))
                return EditorNodeReadinessResult.Ready();
            return EditorNodeReadinessResult.NotReady("等待数字输入。");
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

        private static string Format(double value)
        {
            return value.ToString("G15", CultureInfo.InvariantCulture);
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private static string GetText(RoundOffMode mode)
        {
            return mode == RoundOffMode.Ceiling ? "向上取整" : "向下取整";
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

    public sealed class RoundOffModePropertyDescriptor :
        STNodePropertyDescriptor
    {
        protected override object GetValueFromString(string text)
        {
            string normalized = Normalize(text);
            switch (normalized)
            {
                case "向上取整":
                case "向上":
                case "上":
                case "Ceiling":
                case "Ceil":
                case "Up":
                    return RoundOffMode.Ceiling;
                case "向下取整":
                case "向下":
                case "下":
                case "Floor":
                case "Down":
                    return RoundOffMode.Floor;
                default:
                    return Enum.Parse(typeof(RoundOffMode), normalized, true);
            }
        }

        protected override string GetStringFromValue()
        {
            return GetText((RoundOffMode)GetValue(null));
        }

        protected override string GetSelectItemText(object value)
        {
            return GetText((RoundOffMode)value);
        }

        private static string GetText(RoundOffMode mode)
        {
            return mode == RoundOffMode.Ceiling ? "向上取整" : "向下取整";
        }

        private static string Normalize(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Trim()
                .Replace(" ", string.Empty)
                .Replace("\t", string.Empty)
                .Replace("\u3000", string.Empty);
        }
    }
}
