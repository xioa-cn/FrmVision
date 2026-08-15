using System;
using System.Collections.Generic;
using System.Linq;
using FrmServices.Services.EditorServices;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public enum BoolJudgmentMode
    {
        And,
        Or
    }

    public sealed class BoolNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public bool Value { get; set; }
        public bool[] InputValues { get; set; }
        public BoolJudgmentMode Mode { get; set; }
        public string Message { get; set; }
    }

    [STNode("逻辑判断", "xioa", null, null,
        "对多个布尔输入执行且或判断，并从 True 或 False 端口继续流程。")]
    public sealed class BoolNode : WorkflowNode, IEditorExecutableNode,
        IEditorNodeReadiness
    {
        private const int MinimumInputCount = 2;
        private const int MaximumInputCount = 32;
        private readonly List<STNodeOption> _inputs =
            new List<STNodeOption>();
        private int _inputCount = MinimumInputCount;

        public BoolNode()
        {
            SetNodeTypeTitle("布尔判断");
            EnableExecutionLog = true;
            TitleColor = System.Drawing.Color.FromArgb(220, 15, 118, 110);
            LetGetOptions = true;

            TrueOutput = OutputOptions.Add("True", typeof(object), false);
            FalseOutput = OutputOptions.Add("False", typeof(object), false);
            ConfigureInputs(_inputCount);
        }

        public STNodeOption TrueOutput { get; }
        public STNodeOption FalseOutput { get; }
        public IReadOnlyList<STNodeOption> Inputs => _inputs;

        [STNodeProperty("判断模式", "&：所有输入为 True 时结果为 True；|：任一输入为 True 时结果为 True。",
            DescriptorType = typeof(BoolJudgmentModePropertyDescriptor))]
        public BoolJudgmentMode Mode { get; set; } = BoolJudgmentMode.And;

        [STNodeProperty("输入数量", "布尔输入端口数量，可设置为 2 到 32。")]
        public int InputCount
        {
            get => _inputCount;
            set
            {
                if (value < MinimumInputCount || value > MaximumInputCount)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        "输入数量必须在 2 到 32 之间。");
                if (_inputCount == value && _inputs.Count == value) return;
                _inputCount = value;
                ConfigureInputs(value);
            }
        }

        public BoolNodeExecutionResult Execute()
        {
            try
            {
                bool[] values = _inputs.Select(GetBooleanValue).ToArray();
                bool result;
                switch (Mode)
                {
                    case BoolJudgmentMode.And:
                        result = values.All(value => value);
                        break;
                    case BoolJudgmentMode.Or:
                        result = values.Any(value => value);
                        break;
                    default:
                        throw new InvalidOperationException("不支持的布尔判断模式。");
                }

                STNodeOption activeOutput = result ? TrueOutput : FalseOutput;
                activeOutput.Data = result;
                activeOutput.TransferData();
                return new BoolNodeExecutionResult
                {
                    IsSuccess = true,
                    Value = result,
                    InputValues = values,
                    Mode = Mode,
                    Message = "布尔" + GetModeText(Mode) + "判断结果为 " +
                              (result ? "True" : "False") + "。"
                };
            }
            catch (Exception ex)
            {
                return new BoolNodeExecutionResult
                {
                    IsSuccess = false,
                    InputValues = new bool[0],
                    Mode = Mode,
                    Message = ex.GetBaseException().Message
                };
            }
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();

            BoolNodeExecutionResult result = Execute();
            if (!result.IsSuccess)
                return EditorNodeExecutionResult.Failure(result.Message);
            return EditorNodeExecutionResult.Success(result.Message,
                result.Value ? TrueOutput : FalseOutput);
        }

        public EditorNodeReadinessResult CanExecute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            string[] waitingInputs = _inputs
                .Where(input => !context.IsInputActivated(input) &&
                                !GlobalDataNode.IsReadSource(input))
                .Select(input => input.Text)
                .ToArray();
            return waitingInputs.Length == 0
                ? EditorNodeReadinessResult.Ready()
                : EditorNodeReadinessResult.NotReady("等待" +
                    string.Join("、", waitingInputs) + "。");
        }

        private void ConfigureInputs(int count)
        {
            while (_inputs.Count > count)
            {
                int lastIndex = _inputs.Count - 1;
                STNodeOption input = _inputs[lastIndex];
                input.DataTransfer -= InputOnDataTransfer;
                if (InputOptions.Contains(input)) InputOptions.Remove(input);
                _inputs.RemoveAt(lastIndex);
            }

            while (_inputs.Count < count)
            {
                var input = new STNodeOption("输入 " + (_inputs.Count + 1),
                    typeof(object), true);
                input.DataTransfer += InputOnDataTransfer;
                _inputs.Add(input);
                InputOptions.Add(input);
            }

            BuildSize(true, true, false);
            if (Owner != null)
            {
                Owner.Invalidate();
            }
        }

        private static bool GetBooleanValue(STNodeOption input)
        {
            if (input.Data is bool value) return value;
            throw new InvalidOperationException("“" + input.Text +
                                                "”必须是 bool 值。");
        }

        private static string GetModeText(BoolJudgmentMode mode)
        {
            return mode == BoolJudgmentMode.And ? "且(&)" : "或(|)";
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

    public sealed class BoolJudgmentModePropertyDescriptor :
        STNodePropertyDescriptor
    {
        protected override object GetValueFromString(string text)
        {
            switch ((text ?? string.Empty).Trim())
            {
                case "&":
                case "且":
                case "And":
                    return BoolJudgmentMode.And;
                case "|":
                case "或":
                case "Or":
                    return BoolJudgmentMode.Or;
                default:
                    return Enum.Parse(typeof(BoolJudgmentMode), text, true);
            }
        }

        protected override string GetStringFromValue()
        {
            return GetSymbol((BoolJudgmentMode)GetValue(null));
        }

        protected override string GetSelectItemText(object value)
        {
            return GetSymbol((BoolJudgmentMode)value);
        }

        private static string GetSymbol(BoolJudgmentMode mode)
        {
            return mode == BoolJudgmentMode.And ? "&" : "|";
        }
    }
}
