using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using FrmServices.Services.EditorServices;
using FrmViews.Views;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public enum CompensationOperation
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    internal sealed class CompensationStepDefinition
    {
        public CompensationOperation Operation { get; set; }
        public double Value { get; set; }
    }

    internal static class CompensationStepSerializer
    {
        public static string Serialize(IEnumerable<CompensationStepDefinition> steps)
        {
            var root = new XElement("CompensationSteps",
                steps.Select(step => new XElement("Step",
                    new XAttribute("Operation", step.Operation),
                    new XAttribute("Value", step.Value.ToString("R",
                        CultureInfo.InvariantCulture)))));
            return root.ToString(SaveOptions.DisableFormatting);
        }

        public static List<CompensationStepDefinition> Parse(string value)
        {
            var steps = new List<CompensationStepDefinition>();
            if (string.IsNullOrWhiteSpace(value)) return steps;

            XElement root;
            try
            {
                root = XElement.Parse(value);
            }
            catch (Exception ex)
            {
                throw new FormatException("补偿步骤配置格式不正确。", ex);
            }

            if (!string.Equals(root.Name.LocalName, "CompensationSteps",
                    StringComparison.Ordinal))
                throw new FormatException("补偿步骤配置缺少 CompensationSteps 根节点。");

            int sequence = 0;
            foreach (XElement element in root.Elements("Step"))
            {
                sequence++;
                CompensationOperation operation;
                if (!Enum.TryParse(GetAttribute(element, "Operation"), true,
                        out operation))
                    throw new FormatException("第 " + sequence + " 步的运算类型无效。");

                double operand;
                if (!double.TryParse(GetAttribute(element, "Value"),
                        NumberStyles.Float, CultureInfo.InvariantCulture, out operand) ||
                    double.IsNaN(operand) || double.IsInfinity(operand))
                    throw new FormatException("第 " + sequence + " 步的运算值无效。");

                if (operation == CompensationOperation.Divide && operand == 0D)
                    throw new DivideByZeroException("第 " + sequence + " 步的除数不能为零。");

                steps.Add(new CompensationStepDefinition
                {
                    Operation = operation,
                    Value = operand
                });
            }
            return steps;
        }

        public static string GetOperationSymbol(CompensationOperation operation)
        {
            switch (operation)
            {
                case CompensationOperation.Add: return "+";
                case CompensationOperation.Subtract: return "-";
                case CompensationOperation.Multiply: return "x";
                case CompensationOperation.Divide: return "/";
                default: return "?";
            }
        }

        private static string GetAttribute(XElement element, string name)
        {
            XAttribute attribute = element.Attribute(name);
            if (attribute == null)
                throw new FormatException("补偿步骤配置缺少 " + name + " 字段。");
            return attribute.Value;
        }
    }

    public sealed class CompensationNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public bool ShouldContinue { get; set; }
        public double InputValue { get; set; }
        public double OutputValue { get; set; }
        public int StepCount { get; set; }
        public string Message { get; set; }
    }

    [STNode("数据处理", "xioa", null, null,
        "对单个数值输入按配置顺序执行多步加、减、乘、除，并输出最终结果。")]
    public class CompensationNode : WorkflowNode, IEditorExecutableNode,
        IEditorNodeReadiness
    {
        private readonly List<CompensationStepDefinition> _steps =
            new List<CompensationStepDefinition>();
        private string _stepsText;

        public CompensationNode()
        {
            SetNodeTypeTitle("补偿工具");
            TitleColor = System.Drawing.Color.FromArgb(220, 37, 99, 235);
            LetGetOptions = true;
            Input = InputOptions.Add("输入", typeof(object), true);
            Output = OutputOptions.Add("结果", typeof(double), false);
            Input.DataTransfer += InputOnDataTransfer;
            Steps = CompensationStepSerializer.Serialize(new[]
            {
                new CompensationStepDefinition
                {
                    Operation = CompensationOperation.Add,
                    Value = 0D
                }
            });
        }

        public STNodeOption Input { get; }
        public STNodeOption Output { get; }

        [STNodeProperty("补偿步骤", "按列表顺序执行多步加、减、乘、除。",
            DescriptorType = typeof(CompensationStepsPropertyDescriptor))]
        public string Steps
        {
            get => _stepsText;
            set => SetSteps(value);
        }

        public CompensationNodeExecutionResult Execute(object inputValue)
        {
            double input = 0D;
            try
            {
                if (inputValue == null)
                    throw new InvalidOperationException("补偿输入值不能为空。");
                if (_steps.Count == 0)
                    throw new InvalidOperationException("至少需要配置一个补偿步骤。");

                input = Convert.ToDouble(inputValue, CultureInfo.InvariantCulture);
                if (double.IsNaN(input) || double.IsInfinity(input))
                    throw new InvalidOperationException("补偿输入值必须是有限数值。");

                double result = input;
                for (int index = 0; index < _steps.Count; index++)
                {
                    CompensationStepDefinition step = _steps[index];
                    switch (step.Operation)
                    {
                        case CompensationOperation.Add:
                            result += step.Value;
                            break;
                        case CompensationOperation.Subtract:
                            result -= step.Value;
                            break;
                        case CompensationOperation.Multiply:
                            result *= step.Value;
                            break;
                        case CompensationOperation.Divide:
                            if (step.Value == 0D)
                                throw new DivideByZeroException("第 " + (index + 1) +
                                    " 步的除数不能为零。");
                            result /= step.Value;
                            break;
                        default:
                            throw new InvalidOperationException("第 " + (index + 1) +
                                " 步的运算类型无效。");
                    }

                    if (double.IsNaN(result) || double.IsInfinity(result))
                        throw new OverflowException("第 " + (index + 1) +
                            " 步运算结果超出有效数值范围。");
                }

                Output.Data = result;
                Output.TransferData();
                return new CompensationNodeExecutionResult
                {
                    IsSuccess = true,
                    ShouldContinue = true,
                    InputValue = input,
                    OutputValue = result,
                    StepCount = _steps.Count,
                    Message = "补偿计算完成：" + input.ToString("G15",
                                  CultureInfo.InvariantCulture) + " -> " +
                              result.ToString("G15", CultureInfo.InvariantCulture) +
                              "，共 " + _steps.Count + " 步。"
                };
            }
            catch (Exception ex)
            {
                return new CompensationNodeExecutionResult
                {
                    IsSuccess = false,
                    ShouldContinue = false,
                    InputValue = input,
                    StepCount = _steps.Count,
                    Message = ex.GetBaseException().Message
                };
            }
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();
            CompensationNodeExecutionResult result = Execute(Input.Data);
            return result.IsSuccess
                ? EditorNodeExecutionResult.Success(result.Message, Output)
                : EditorNodeExecutionResult.Failure(result.Message);
        }

        public EditorNodeReadinessResult CanExecute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return ((context.IsInputActivated(Input) ||
                     GlobalDataNode.IsReadSource(Input)) && Input.Data != null)
                ? EditorNodeReadinessResult.Ready()
                : EditorNodeReadinessResult.NotReady("等待补偿输入值。");
        }

        private void InputOnDataTransfer(object sender, STNodeOptionEventArgs e)
        {
            Input.Data = e.Status == ConnectionStatus.Connected &&
                         e.TargetOption != null
                ? e.TargetOption.Data
                : null;
        }

        private void SetSteps(string value)
        {
            List<CompensationStepDefinition> steps =
                CompensationStepSerializer.Parse(value);
            _steps.Clear();
            _steps.AddRange(steps);
            _stepsText = CompensationStepSerializer.Serialize(steps);
        }
    }

    public sealed class CompensationStepsPropertyDescriptor :
        STNodePropertyDescriptor
    {
        protected override string GetStringFromValue()
        {
            var node = Node as CompensationNode;
            if (node == null) return "未配置";
            try
            {
                List<CompensationStepDefinition> steps =
                    CompensationStepSerializer.Parse(node.Steps);
                if (steps.Count == 0) return "未配置";
                string summary = string.Join(" -> ", steps.Take(4).Select(step =>
                    CompensationStepSerializer.GetOperationSymbol(step.Operation) +
                    step.Value.ToString("G8", CultureInfo.InvariantCulture)).ToArray());
                if (steps.Count > 4) summary += " ...";
                return steps.Count + " 步：" + summary;
            }
            catch
            {
                return "配置错误";
            }
        }

        protected override byte[] GetBytesFromValue()
        {
            var node = Node as CompensationNode;
            return Encoding.UTF8.GetBytes(node == null
                ? string.Empty
                : node.Steps ?? string.Empty);
        }

        protected override object GetValueFromBytes(byte[] byData)
        {
            return byData == null ? string.Empty : Encoding.UTF8.GetString(byData);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            var node = Node as CompensationNode;
            if (node == null) return;
            using (var editor = new CompensationStepEditorFrm(node.Steps))
            {
                if (editor.ShowDialog(Control.FindForm()) != DialogResult.OK) return;
                SetValue(editor.Steps);
            }
        }
    }
}
