using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Cognex.VisionPro;
using FrmServices.Services.EditorServices;
using FrmServices.Utils;
using FrmViews.Views;
using FrmVpComponents.Services;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public enum VisionResultValueType
    {
        Int,
        UInt,
        Short,
        UShort,
        Long,
        ULong,
        Double,
        Float,
        Byte,
        Boolean,
        String
    }

    public sealed class VisionNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public bool ShouldContinue { get; set; }
        public string CameraKey { get; set; }
        public string VisionToolKey { get; set; }
        public ICogImage InputImage { get; set; }
        public ICogRecords VisionRecords { get; set; }
        public object ResultValue { get; set; }
        public Dictionary<string, object> OutputValues { get; set; }
        public string[] MatchedConditions { get; set; }
        public string[] MatchedOutputs { get; set; }
        public string Message { get; set; }
    }

    [STNode("视觉图像", "xioa", null, null,
        "Result 满足用户配置的条件时，输出指定的视觉工具结果值。")]
    public class VisionNode : WorkflowNode, IEditorExecutableNode,
        IEditorNodeReadiness
    {
        private const string ResultKey = "Result";
        private const string VisionImageOutputName = "视觉图片";
        private readonly STNodeOption _resultOutput;
        private readonly STNodeOption _visionImageOutput;
        private readonly List<KeyInput> _keyInputs = new List<KeyInput>();
        private readonly List<KeyOutput> _keyOutputs = new List<KeyOutput>();
        private readonly List<ResultCondition> _resultConditions =
            new List<ResultCondition>();
        private STNodeOption _inputImage;
        private bool _isImageInputEnabled = true;
        private string _imageInputKey = string.Empty;
        private string _inputKeysText = string.Empty;
        private string _outputKeysText = string.Empty;
        private string _resultConditionsText = string.Empty;
        private VisionResultValueType _resultValueType = VisionResultValueType.String;

        public VisionNode()
        {
            SetNodeTypeTitle("视觉工具");
            EnableExecutionLog = true;
            TitleColor = System.Drawing.Color.FromArgb(220, 116, 78, 170);
            LetGetOptions = true;
            SetImageInputKey("InputImage");
            _visionImageOutput = OutputOptions.Add(
                VisionImageOutputName, typeof(ICogRecords), false);
            _resultOutput = OutputOptions.Add(ResultKey, typeof(object), false);
            SetInputKeys(string.Empty);
            SetOutputKeys(string.Empty);
            SetResultConditions("=True");
            InitializeToolWindowMenu();
        }

        public STNodeOption InputImage => _inputImage;
        public STNodeOption ResultOutput => _resultOutput;
        public STNodeOption VisionImageOutput => _visionImageOutput;

        [STNodeProperty("相机 Key", "BlockTool 中视觉工具所属的相机或产品分组 Key。")]
        public string CameraKey { get; set; } = string.Empty;

        [STNodeProperty("视觉工具 Key", "相机分组下 CogToolBlock 的唯一 Key。")]
        public string VisionToolKey { get; set; } = string.Empty;

        [STNodeProperty("启用图像输入",
            "是否创建 ICogImage 图像输入端口；关闭后保留图像输入 Key 配置。")]
        public bool IsImageInputEnabled
        {
            get => _isImageInputEnabled;
            set => SetImageInputEnabled(value);
        }

        [STNodeProperty("图像输入 Key",
            "启用图像输入时，CogToolBlock 接收 ICogImage 的输入端名称。")]
        public string ImageInputKey
        {
            get => _imageInputKey;
            set => SetImageInputKey(value);
        }

        [STNodeProperty("输入 Key",
            "配置需要写入 CogToolBlock Inputs 的其他必需输入 Key。",
            DescriptorType = typeof(VisionInputKeysPropertyDescriptor))]
        public string InputKeys
        {
            get => _inputKeysText;
            set => SetInputKeys(value);
        }

        [STNodeProperty("输出 Key", "配置需要返回的其他 Outputs Key。",
            DescriptorType = typeof(VisionOutputKeysPropertyDescriptor))]
        public string OutputKeys
        {
            get => _outputKeysText;
            set => SetOutputKeys(value);
        }

        [STNodeProperty("Result 类型", "Outputs[Result] 用于条件比较时的数据类型。")]
        public VisionResultValueType ResultValueType
        {
            get => _resultValueType;
            set => _resultValueType = value;
        }

        [STNodeProperty("Result 条件", "Result 满足任一条件时才返回配置的输出 Key。",
            DescriptorType = typeof(VisionResultConditionsPropertyDescriptor))]
        public string ResultConditions
        {
            get => _resultConditionsText;
            set => SetResultConditions(value);
        }

        public VisionNodeExecutionResult Execute()
        {
            return Execute(BlockTool.Instance,
                _inputImage == null ? null : _inputImage.Data as ICogImage);
        }

        public VisionNodeExecutionResult Execute(ICogImage image)
        {
            return Execute(BlockTool.Instance, image);
        }

        public VisionNodeExecutionResult Execute(BlockTool blockTool, ICogImage image)
        {
            try
            {
                ValidateConfiguration(blockTool, image);
                VisionToolExecutionData toolExecution = blockTool.UseCogToolBlock(
                    CameraKey,
                    VisionToolKey,
                    tool =>
                    {
                        if (_inputImage != null)
                            tool.Inputs[_imageInputKey].Value = image;
                        foreach (KeyInput input in _keyInputs)
                            tool.Inputs[input.Key].Value = input.Option.Data;
                        tool.Run();
                        if (tool.RunStatus != null && tool.RunStatus.Exception != null)
                            throw new InvalidOperationException(
                                "视觉工具运行失败。", tool.RunStatus.Exception);

                        ICogRecord lastRunRecord = tool.CreateLastRunRecord();
                        if (lastRunRecord == null || lastRunRecord.SubRecords == null ||
                            lastRunRecord.SubRecords.Count == 0)
                            throw new InvalidOperationException(
                                "视觉工具没有生成可输出的视觉记录。");
                        ICogRecords visionRecords = lastRunRecord.SubRecords;

                        object resultValue = tool.Outputs[ResultKey].Value;
                        List<ResultCondition> matchedConditions =
                            GetMatchedConditions(resultValue);
                        var values = new Dictionary<string, object>(StringComparer.Ordinal)
                        {
                            [ResultKey] = resultValue
                        };

                        if (matchedConditions.Count > 0)
                        {
                            foreach (KeyOutput output in _keyOutputs)
                                values.Add(output.Key, tool.Outputs[output.Key].Value);
                        }

                        return new VisionToolExecutionData(
                            resultValue, visionRecords, values, matchedConditions);
                    });

                bool shouldOutputOtherValues =
                    toolExecution.MatchedConditions.Count > 0;
                var executionResult = new VisionNodeExecutionResult
                {
                    IsSuccess = true,
                    ShouldContinue = true,
                    CameraKey = CameraKey,
                    VisionToolKey = VisionToolKey,
                    InputImage = image,
                    VisionRecords = toolExecution.VisionRecords,
                    ResultValue = toolExecution.ResultValue,
                    OutputValues = toolExecution.OutputValues,
                    MatchedConditions = toolExecution.MatchedConditions
                        .Select(item => item.Label).ToArray(),
                    MatchedOutputs = new[] { ResultKey, VisionImageOutputName }.Concat(
                        shouldOutputOtherValues
                            ? _keyOutputs.Select(item => item.Key)
                            : Enumerable.Empty<string>()).ToArray(),
                    Message = shouldOutputOtherValues
                        ? "已返回 Result、视觉图片和配置的其他输出。"
                        : "Result 不满足条件，仅返回 Result 和视觉图片。"
                };

                _resultOutput.Data = toolExecution.ResultValue;
                _resultOutput.TransferData();
                _visionImageOutput.Data = toolExecution.VisionRecords;
                _visionImageOutput.TransferData();

                if (shouldOutputOtherValues)
                {
                    foreach (KeyOutput output in _keyOutputs)
                    {
                        output.Option.Data = toolExecution.OutputValues[output.Key];
                        output.Option.TransferData();
                    }
                }

                return executionResult;
            }
            catch (Exception ex)
            {
                return new VisionNodeExecutionResult
                {
                    IsSuccess = false,
                    ShouldContinue = false,
                    CameraKey = CameraKey,
                    VisionToolKey = VisionToolKey,
                    InputImage = image,
                    OutputValues = new Dictionary<string, object>(StringComparer.Ordinal),
                    MatchedConditions = new string[0],
                    MatchedOutputs = new string[0],
                    Message = ex.GetBaseException().Message
                };
            }
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();
            VisionNodeExecutionResult result = Execute();
            if (!result.IsSuccess)
                return EditorNodeExecutionResult.Failure(result.Message);

            var activeNames = new HashSet<string>(
                result.MatchedOutputs ?? new string[0], StringComparer.Ordinal);
            STNodeOption[] activeOutputs = (GetOutputOptions() ?? new STNodeOption[0])
                .Where(option => activeNames.Contains(option.Text)).ToArray();
            return EditorNodeExecutionResult.Success(result.Message, activeOutputs);
        }

        public EditorNodeReadinessResult CanExecute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            string[] waitingKeys = GetConfiguredInputs()
                .Where(input => !context.IsInputActivated(input.Option) &&
                                !GlobalDataNode.IsReadSource(input.Option))
                .Select(input => input.Key)
                .ToArray();
            return waitingKeys.Length == 0
                ? EditorNodeReadinessResult.Ready()
                : EditorNodeReadinessResult.NotReady(
                    "等待视觉工具输入：" + string.Join("、", waitingKeys) + "。");
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.Button == MouseButtons.Left) OpenToolWindow();
        }

        private void InitializeToolWindowMenu()
        {
            var menu = new ContextMenuStrip();
            var openItem = new ToolStripMenuItem("打开视觉工具窗口");
            openItem.Click += (sender, args) => OpenToolWindow();
            menu.Items.Add(openItem);
            ContextMenuStrip = menu;
        }

        private void OpenToolWindow()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CameraKey))
                    throw new InvalidOperationException("相机 Key 不能为空。");
                if (string.IsNullOrWhiteSpace(VisionToolKey))
                    throw new InvalidOperationException("视觉工具 Key 不能为空。");

                string parameterName = PictureUtils.GetCurrentParameterName(
                    CameraKey);
                if (string.IsNullOrWhiteSpace(parameterName))
                    throw new InvalidOperationException("未找到产品“" +
                        CameraKey.Trim() + "”当前正在使用的参数名称。");
                string path = ConfigDirUtils.GetBlockToolUtilsDir(
                    CameraKey.Trim(), parameterName.Trim(), VisionToolKey.Trim());
                BlockToolUtils.OpenCogToolBlock(path, CameraKey.Trim(),
                    VisionToolKey.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(Owner == null ? null : Owner.FindForm(),
                    "无法打开视觉工具窗口：" + ex.GetBaseException().Message,
                    "视觉工具", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InputImageOnDataTransfer(object sender, STNodeOptionEventArgs e)
        {
            var input = sender as STNodeOption;
            if (input == null) return;
            input.Data = e.Status == ConnectionStatus.Connected &&
                         e.TargetOption != null
                ? e.TargetOption.Data
                : null;
        }

        private void SetImageInputKey(string value)
        {
            string key = (value ?? string.Empty).Trim();
            if (key.IndexOfAny(new[] { ';', '\r', '\n' }) >= 0)
                throw new FormatException("图像输入 Key 不能包含分号或换行。");
            if (_isImageInputEnabled && key.Length > 0 &&
                _keyInputs.Any(input =>
                    string.Equals(input.Key, key,
                        StringComparison.OrdinalIgnoreCase)))
                throw new FormatException("图像输入 Key 与其他输入 Key 重复：" + key);

            _imageInputKey = key;
            ConfigureImageInputPort();
        }

        private void SetImageInputEnabled(bool value)
        {
            if (value)
            {
                if (string.IsNullOrWhiteSpace(_imageInputKey))
                    throw new InvalidOperationException(
                        "启用图像输入前必须配置图像输入 Key。");
                if (_keyInputs.Any(input => string.Equals(input.Key,
                        _imageInputKey, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException(
                        "图像输入 Key 与其他输入 Key 重复：" + _imageInputKey);
            }

            _isImageInputEnabled = value;
            ConfigureImageInputPort();
        }

        private void ConfigureImageInputPort()
        {
            if (!_isImageInputEnabled)
            {
                if (_inputImage != null)
                {
                    _inputImage.DataTransfer -= InputImageOnDataTransfer;
                    if (InputOptions.Contains(_inputImage))
                        InputOptions.Remove(_inputImage);
                    _inputImage = null;
                }
                RefreshInputPortLayout();
                return;
            }

            if (_inputImage == null)
            {
                _inputImage = new STNodeOption("图像", typeof(ICogImage), true);
                _inputImage.DataTransfer += InputImageOnDataTransfer;
                if (InputOptions.Count == 0)
                    InputOptions.Add(_inputImage);
                else
                    InputOptions.Insert(0, _inputImage);
            }
            RefreshInputPortLayout();
        }

        private void SetInputKeys(string value)
        {
            List<string> keys = ParseInputKeys(value,
                _isImageInputEnabled ? _imageInputKey : null);
            var replacements = new List<KeyInput>();

            foreach (string key in keys)
            {
                KeyInput existing = _keyInputs.FirstOrDefault(input =>
                    string.Equals(input.Key, key, StringComparison.Ordinal));
                replacements.Add(existing ?? new KeyInput(key, CreateKeyInput(key)));
            }

            var retainedOptions = new HashSet<STNodeOption>(
                replacements.Select(input => input.Option));
            foreach (KeyInput oldInput in _keyInputs)
            {
                if (retainedOptions.Contains(oldInput.Option)) continue;
                oldInput.Option.DataTransfer -= KeyInputOnDataTransfer;
                if (InputOptions.Contains(oldInput.Option))
                    InputOptions.Remove(oldInput.Option);
            }

            _keyInputs.Clear();
            _keyInputs.AddRange(replacements);
            foreach (KeyInput input in _keyInputs)
                if (!InputOptions.Contains(input.Option))
                    InputOptions.Add(input.Option);

            _inputKeysText = string.Join(";", keys.ToArray());
            RefreshInputPortLayout();
        }

        private static List<string> ParseInputKeys(string value,
            string imageInputKey)
        {
            var keys = new List<string>();
            var uniqueKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string[] tokens = (value ?? string.Empty).Split(
                new[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string rawToken in tokens)
            {
                string key = rawToken.Trim();
                if (key.Length == 0) continue;
                if (!string.IsNullOrEmpty(imageInputKey) &&
                    string.Equals(key, imageInputKey,
                        StringComparison.OrdinalIgnoreCase))
                    throw new FormatException(
                        "输入 Key 与图像输入 Key 重复：" + key);
                if (!uniqueKeys.Add(key))
                    throw new FormatException("输入 Key 不能重复：" + key);
                keys.Add(key);
            }
            return keys;
        }

        private STNodeOption CreateKeyInput(string key)
        {
            var option = new STNodeOption(key, typeof(object), true);
            option.DataTransfer += KeyInputOnDataTransfer;
            return option;
        }

        private static void KeyInputOnDataTransfer(object sender,
            STNodeOptionEventArgs e)
        {
            var option = sender as STNodeOption;
            if (option == null) return;
            option.Data = e.Status == ConnectionStatus.Connected &&
                          e.TargetOption != null
                ? e.TargetOption.Data
                : null;
        }

        private IEnumerable<KeyInput> GetConfiguredInputs()
        {
            if (_inputImage != null)
                yield return new KeyInput(_imageInputKey, _inputImage);
            foreach (KeyInput input in _keyInputs)
                yield return input;
        }

        private void RefreshInputPortLayout()
        {
            BuildSize(true, true, false);
            SetOptionsLocation();
            Invalidate();
        }

        private void SetOutputKeys(string value)
        {
            List<string> keys = ParseOutputKeys(value);
            var desiredKeys = new HashSet<string>(keys, StringComparer.Ordinal);

            foreach (KeyOutput existing in _keyOutputs.ToArray())
            {
                if (desiredKeys.Contains(existing.Key)) continue;
                if (OutputOptions.Contains(existing.Option))
                    OutputOptions.Remove(existing.Option);
                _keyOutputs.Remove(existing);
            }

            foreach (string key in keys)
            {
                if (_keyOutputs.Any(item => item.Key == key)) continue;
                var option = new STNodeOption(key, typeof(object), false);
                _keyOutputs.Add(new KeyOutput(key, option));
                OutputOptions.Insert(OutputOptions.IndexOf(_resultOutput), option);
            }

            _outputKeysText = string.Join(";", keys.ToArray());
        }

        private static List<string> ParseOutputKeys(string value)
        {
            var keys = new List<string>();
            var uniqueKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string[] tokens = (value ?? string.Empty).Split(
                new[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string rawToken in tokens)
            {
                string key = rawToken.Trim();
                if (key.Length == 0) continue;
                if (string.Equals(key, ResultKey, StringComparison.OrdinalIgnoreCase))
                    throw new FormatException("Result 是固定判断 Key，不需要添加到输出 Key。");
                if (!uniqueKeys.Add(key))
                    throw new FormatException("输出 Key 不能重复：" + key);
                keys.Add(key);
            }
            return keys;
        }

        private void SetResultConditions(string value)
        {
            List<ResultCondition> conditions = ParseResultConditions(value);
            _resultConditions.Clear();
            _resultConditions.AddRange(conditions);
            _resultConditionsText = string.Join(";",
                conditions.Select(item => item.Label).ToArray());
        }

        private static List<ResultCondition> ParseResultConditions(string value)
        {
            var conditions = new List<ResultCondition>();
            var labels = new HashSet<string>(StringComparer.Ordinal);
            string[] tokens = (value ?? string.Empty).Split(
                new[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string rawToken in tokens)
            {
                string token = rawToken.Trim();
                if (token.Length == 0) continue;
                char comparisonOperator = token[0];
                if (comparisonOperator != '>' && comparisonOperator != '=' &&
                    comparisonOperator != '<')
                    throw new FormatException("Result 条件必须以 >、= 或 < 开头。");

                int valueIndex = comparisonOperator == '=' &&
                                 token.StartsWith("==", StringComparison.Ordinal) ? 2 : 1;
                string expectedValue = token.Substring(valueIndex).Trim();
                if (expectedValue.Length == 0)
                    throw new FormatException("Result 比较值不能为空。");
                string label = comparisonOperator + expectedValue;
                if (!labels.Add(label))
                    throw new FormatException("Result 条件不能重复：" + label);
                conditions.Add(new ResultCondition(
                    comparisonOperator, expectedValue, label));
            }
            return conditions;
        }

        private List<ResultCondition> GetMatchedConditions(object resultValue)
        {
            if (resultValue == null)
                throw new InvalidOperationException("视觉工具的 Result 输出值为空。");

            var matchedConditions = new List<ResultCondition>();
            foreach (ResultCondition condition in _resultConditions)
            {
                if (!SupportsOrdering(_resultValueType) && condition.Operator != '=')
                    continue;
                int comparison = CompareResultValue(resultValue, condition.ExpectedValue);
                if (IsMatch(comparison, condition.Operator))
                    matchedConditions.Add(condition);
            }
            return matchedConditions;
        }

        private int CompareResultValue(object actualValue, string expectedValue)
        {
            switch (_resultValueType)
            {
                case VisionResultValueType.Int:
                    return Convert.ToInt32(actualValue, CultureInfo.InvariantCulture)
                        .CompareTo(int.Parse(expectedValue, NumberStyles.Integer,
                            CultureInfo.InvariantCulture));
                case VisionResultValueType.UInt:
                    return Convert.ToUInt32(actualValue, CultureInfo.InvariantCulture)
                        .CompareTo(uint.Parse(expectedValue, NumberStyles.Integer,
                            CultureInfo.InvariantCulture));
                case VisionResultValueType.Short:
                    return Convert.ToInt16(actualValue, CultureInfo.InvariantCulture)
                        .CompareTo(short.Parse(expectedValue, NumberStyles.Integer,
                            CultureInfo.InvariantCulture));
                case VisionResultValueType.UShort:
                    return Convert.ToUInt16(actualValue, CultureInfo.InvariantCulture)
                        .CompareTo(ushort.Parse(expectedValue, NumberStyles.Integer,
                            CultureInfo.InvariantCulture));
                case VisionResultValueType.Long:
                    return Convert.ToInt64(actualValue, CultureInfo.InvariantCulture)
                        .CompareTo(long.Parse(expectedValue, NumberStyles.Integer,
                            CultureInfo.InvariantCulture));
                case VisionResultValueType.ULong:
                    return Convert.ToUInt64(actualValue, CultureInfo.InvariantCulture)
                        .CompareTo(ulong.Parse(expectedValue, NumberStyles.Integer,
                            CultureInfo.InvariantCulture));
                case VisionResultValueType.Double:
                    return Convert.ToDouble(actualValue, CultureInfo.InvariantCulture)
                        .CompareTo(double.Parse(expectedValue, NumberStyles.Float |
                            NumberStyles.AllowThousands, CultureInfo.InvariantCulture));
                case VisionResultValueType.Float:
                    return Convert.ToSingle(actualValue, CultureInfo.InvariantCulture)
                        .CompareTo(float.Parse(expectedValue, NumberStyles.Float |
                            NumberStyles.AllowThousands, CultureInfo.InvariantCulture));
                case VisionResultValueType.Byte:
                    return Convert.ToByte(actualValue, CultureInfo.InvariantCulture)
                        .CompareTo(byte.Parse(expectedValue, NumberStyles.Integer,
                            CultureInfo.InvariantCulture));
                case VisionResultValueType.Boolean:
                    return Convert.ToBoolean(actualValue, CultureInfo.InvariantCulture)
                        .CompareTo(bool.Parse(expectedValue));
                case VisionResultValueType.String:
                    return StringComparer.Ordinal.Compare(
                        Convert.ToString(actualValue, CultureInfo.InvariantCulture), expectedValue);
                default:
                    throw new InvalidOperationException("不支持的 Result 数据类型。");
            }
        }

        private void ValidateConfiguration(BlockTool blockTool, ICogImage image)
        {
            if (blockTool == null)
                throw new ArgumentNullException(nameof(blockTool));
            if (_inputImage != null && image == null)
                throw new InvalidOperationException("视觉工具输入图像不能为空。");
            if (string.IsNullOrWhiteSpace(CameraKey))
                throw new InvalidOperationException("相机 Key 不能为空。");
            if (string.IsNullOrWhiteSpace(VisionToolKey))
                throw new InvalidOperationException("视觉工具 Key 不能为空。");
            if (_inputImage != null && string.IsNullOrWhiteSpace(_imageInputKey))
                throw new InvalidOperationException("图像输入 Key 不能为空。");
            KeyInput emptyInput = _keyInputs.FirstOrDefault(input =>
                input.Option.Data == null);
            if (emptyInput != null)
                throw new InvalidOperationException(
                    "视觉工具输入不能为空：" + emptyInput.Key + "。");
            if (_resultConditions.Count == 0)
                throw new InvalidOperationException("至少需要配置一个 Result 条件。");
            if (!SupportsOrdering(_resultValueType) &&
                _resultConditions.Any(item => item.Operator != '='))
                throw new InvalidOperationException("布尔值和字符串 Result 仅支持 = 条件。");
        }

        private static bool SupportsOrdering(VisionResultValueType valueType)
        {
            return valueType != VisionResultValueType.Boolean &&
                   valueType != VisionResultValueType.String;
        }

        private static bool IsMatch(int comparison, char comparisonOperator)
        {
            switch (comparisonOperator)
            {
                case '>': return comparison > 0;
                case '=': return comparison == 0;
                case '<': return comparison < 0;
                default: return false;
            }
        }

        private sealed class KeyOutput
        {
            public KeyOutput(string key, STNodeOption option)
            {
                Key = key;
                Option = option;
            }

            public string Key { get; }
            public STNodeOption Option { get; }
        }

        private sealed class KeyInput
        {
            public KeyInput(string key, STNodeOption option)
            {
                Key = key;
                Option = option;
            }

            public string Key { get; }
            public STNodeOption Option { get; }
        }

        private sealed class ResultCondition
        {
            public ResultCondition(char comparisonOperator,
                string expectedValue, string label)
            {
                Operator = comparisonOperator;
                ExpectedValue = expectedValue;
                Label = label;
            }

            public char Operator { get; }
            public string ExpectedValue { get; }
            public string Label { get; }
        }

        private sealed class VisionToolExecutionData
        {
            public VisionToolExecutionData(object resultValue,
                ICogRecords visionRecords,
                Dictionary<string, object> outputValues,
                List<ResultCondition> matchedConditions)
            {
                ResultValue = resultValue;
                VisionRecords = visionRecords;
                OutputValues = outputValues;
                MatchedConditions = matchedConditions;
            }

            public object ResultValue { get; }
            public ICogRecords VisionRecords { get; }
            public Dictionary<string, object> OutputValues { get; }
            public List<ResultCondition> MatchedConditions { get; }
        }
    }

    public sealed class VisionInputKeysPropertyDescriptor :
        STNodePropertyDescriptor
    {
        protected override void OnMouseClick(MouseEventArgs e)
        {
            var node = Node as VisionNode;
            if (node == null) return;

            using (var editor = new PlcComparisonOutputEditorFrm(
                       PlcReadValueType.String, node.InputKeys,
                       "编辑输入 Key", true,
                       node.IsImageInputEnabled ? node.ImageInputKey : null,
                       "输入 Key"))
            {
                if (editor.ShowDialog(Control.FindForm()) != DialogResult.OK) return;
                SetValue(editor.ComparisonOutputs);
            }
        }
    }

    public sealed class VisionOutputKeysPropertyDescriptor :
        STNodePropertyDescriptor
    {
        protected override void OnMouseClick(MouseEventArgs e)
        {
            var node = Node as VisionNode;
            if (node == null) return;

            using (var editor = new PlcComparisonOutputEditorFrm(
                       PlcReadValueType.String, node.OutputKeys,
                       "编辑输出 Key", true, "Result"))
            {
                if (editor.ShowDialog(Control.FindForm()) != DialogResult.OK) return;
                SetValue(editor.ComparisonOutputs);
            }
        }
    }

    public sealed class VisionResultConditionsPropertyDescriptor :
        STNodePropertyDescriptor
    {
        protected override void OnMouseClick(MouseEventArgs e)
        {
            var node = Node as VisionNode;
            if (node == null) return;

            PlcReadValueType editorValueType =
                node.ResultValueType == VisionResultValueType.Boolean ||
                node.ResultValueType == VisionResultValueType.String
                    ? PlcReadValueType.String
                    : PlcReadValueType.Int;
            using (var editor = new PlcComparisonOutputEditorFrm(
                       editorValueType, node.ResultConditions,
                       "编辑 Result 条件"))
            {
                if (editor.ShowDialog(Control.FindForm()) != DialogResult.OK) return;
                SetValue(editor.ComparisonOutputs);
            }
        }
    }
}
