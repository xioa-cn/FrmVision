using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using FrmServices.Communication;
using FrmServices.Services.EditorServices;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public enum PlcOperationMode
    {
        Read,
        Write
    }

    public enum PlcReadValueType
    {
        Int,
        UInt,
        Short,
        UShort,
        Long,
        ULong,
        String
    }

    public enum PlcWriteValueType
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
        String
    }

    public enum PlcWriteValueSource
    {
        Input,
        FixedValue
    }

    public sealed class PlcWriteItemExecutionResult
    {
        public string Key { get; set; }
        public string Address { get; set; }
        public PlcWriteValueType ValueType { get; set; }
        public PlcWriteValueSource ValueSource { get; set; }
        public object Value { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public sealed class PlcNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public bool ShouldContinue { get; set; }
        public bool? ComparisonMatched { get; set; }
        public string MatchedOutput { get; set; }
        public string[] MatchedOutputs { get; set; }
        public object Value { get; set; }
        public PlcWriteItemExecutionResult[] WriteResults { get; set; }
        public string Message { get; set; }
    }

    internal sealed class PlcWriteItemDefinition
    {
        public string Key { get; set; }
        public string Address { get; set; }
        public PlcWriteValueType ValueType { get; set; }
        public PlcWriteValueSource ValueSource { get; set; }
        public string FixedValue { get; set; }
    }

    internal static class PlcWriteItemSerializer
    {
        public static string Serialize(IEnumerable<PlcWriteItemDefinition> items)
        {
            var root = new XElement("Writes",
                items.Select(item => new XElement("Write",
                    new XAttribute("Key", item.Key ?? string.Empty),
                    new XAttribute("Address", item.Address ?? string.Empty),
                    new XAttribute("Type", item.ValueType),
                    new XAttribute("Source", item.ValueSource),
                    new XAttribute("Value", item.FixedValue ?? string.Empty))));
            return root.ToString(SaveOptions.DisableFormatting);
        }

        public static List<PlcWriteItemDefinition> Parse(string value)
        {
            var definitions = new List<PlcWriteItemDefinition>();
            if (string.IsNullOrWhiteSpace(value)) return definitions;

            XElement root;
            try
            {
                root = XElement.Parse(value);
            }
            catch (Exception ex)
            {
                throw new FormatException("写入配置格式不正确。", ex);
            }

            if (!string.Equals(root.Name.LocalName, "Writes", StringComparison.Ordinal))
                throw new FormatException("写入配置缺少 Writes 根节点。");

            var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (XElement element in root.Elements("Write"))
            {
                string key = GetAttribute(element, "Key").Trim();
                string address = GetAttribute(element, "Address").Trim();
                if (key.Length == 0)
                    throw new FormatException("写入配置的 Key 不能为空。");
                if (!keys.Add(key))
                    throw new FormatException("写入配置的 Key 不能重复：" + key);
                if (address.Length == 0)
                    throw new FormatException("写入项 " + key + " 的地址不能为空。");

                PlcWriteValueType valueType;
                if (!Enum.TryParse(GetAttribute(element, "Type"), true, out valueType))
                    throw new FormatException("写入项 " + key + " 的数据类型无效。");

                PlcWriteValueSource valueSource;
                if (!Enum.TryParse(GetAttribute(element, "Source"), true, out valueSource))
                    throw new FormatException("写入项 " + key + " 的取值方式无效。");

                definitions.Add(new PlcWriteItemDefinition
                {
                    Key = key,
                    Address = address,
                    ValueType = valueType,
                    ValueSource = valueSource,
                    FixedValue = GetOptionalAttribute(element, "Value")
                });
            }

            return definitions;
        }

        private static string GetAttribute(XElement element, string name)
        {
            XAttribute attribute = element.Attribute(name);
            if (attribute == null)
                throw new FormatException("写入配置缺少 " + name + " 字段。");
            return attribute.Value;
        }

        private static string GetOptionalAttribute(XElement element, string name)
        {
            XAttribute attribute = element.Attribute(name);
            return attribute == null ? string.Empty : attribute.Value;
        }
    }

    [STNode("设备通讯", "xioa", null, null,
        "读取 PLC 点位并按条件输出，或将多个输入和固定值写入不同点位。")]
    public class PlcNode : WorkflowNode, IEditorExecutableNode, IEditorNodeReadiness
    {
        private readonly List<ComparisonOutput> _comparisonOutputs =
            new List<ComparisonOutput>();
        private readonly List<WriteItem> _writeItems = new List<WriteItem>();
        private readonly STNodeOption _readInput;
        private PlcOperationMode _operationMode = PlcOperationMode.Read;
        private PlcReadValueType _readValueType = PlcReadValueType.Int;
        private string _comparisonOutputText = string.Empty;
        private string _writeItemsText = string.Empty;
        private int _readLength = 1;

        public PlcNode()
        {
            SetNodeTypeTitle("PLC工具");
            EnableExecutionLog = true;
            TitleColor = System.Drawing.Color.FromArgb(220, 0, 132, 137);
            LetGetOptions = true;
            _readInput = new STNodeOption("输入", typeof(object), false);
            ConfigurePorts();
        }

        public STNodeOption Input => _readInput;

        [STNodeProperty("PLC Key", "用于查找 PLC 通讯实例的唯一 Key。")]
        public string PlcKey { get; set; } = string.Empty;

        [STNodeProperty("操作模式", "Read 为读取，Write 为写入。")]
        public PlcOperationMode OperationMode
        {
            get => _operationMode;
            set
            {
                if (_operationMode == value) return;
                _operationMode = value;
                ConfigurePorts();
            }
        }

        [STNodeProperty("读取点位", "PLC 的读取地址。")]
        public string Address { get; set; } = string.Empty;

        [STNodeProperty("读取类型", "数值支持 >、=、<，字符串仅使用 = 条件。")]
        public PlcReadValueType ReadValueType
        {
            get => _readValueType;
            set
            {
                if (_readValueType == value) return;
                _readValueType = value;
                if (_operationMode == PlcOperationMode.Read)
                    ApplyComparisonOutputs();
            }
        }

        [STNodeProperty("读取长度", "读取元素数量；字符串读取时作为字符长度。")]
        public int ReadLength
        {
            get => _readLength;
            set
            {
                if (value < 1 || value > ushort.MaxValue)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        "读取长度必须在 1 到 65535 之间。");
                _readLength = value;
            }
        }

        [STNodeProperty("比较输出", "编辑独立的比较输出端口。",
            DescriptorType = typeof(PlcComparisonOutputsPropertyDescriptor))]
        public string ComparisonOutputs
        {
            get => _comparisonOutputText;
            set => SetComparisonOutputs(value);
        }

        [STNodeProperty("写入配置", "配置每个写入 Key 的地址、类型和取值方式。",
            DescriptorType = typeof(PlcWriteItemsPropertyDescriptor))]
        public string WriteItems
        {
            get => _writeItemsText;
            set => SetWriteItems(value);
        }

        // 保留旧属性用于加载旧画布；新写模式统一使用 WriteItems。
        [STNodeProperty("旧写入类型", "旧版写入类型，仅用于兼容已有画布。")]
        public PlcWriteValueType WriteValueType { get; set; } = PlcWriteValueType.Int;

        [STNodeProperty("旧写入值", "旧版固定写入值，仅用于兼容已有画布。")]
        public string WriteValue { get; set; } = string.Empty;

        public PlcNodeExecutionResult Execute(PlcFrmVpCommunication plc)
        {
            PlcNodeExecutionResult result;
            var targetOutputs = new List<STNodeOption>();
            try
            {
                ValidateConfiguration(plc);
                result = OperationMode == PlcOperationMode.Read
                    ? ExecuteRead(plc, targetOutputs)
                    : ExecuteWrite(plc);
            }
            catch (Exception ex)
            {
                result = new PlcNodeExecutionResult
                {
                    IsSuccess = false,
                    ShouldContinue = false,
                    MatchedOutputs = new string[0],
                    WriteResults = new PlcWriteItemExecutionResult[0],
                    Message = ex.GetBaseException().Message
                };
            }

            foreach (STNodeOption output in targetOutputs)
            {
                output.Data = result;
                output.TransferData();
            }
            return result;
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();
            PlcNodeExecutionResult result = Execute(context.ResolvePlc(PlcKey));
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
            if (_operationMode != PlcOperationMode.Write)
                return EditorNodeReadinessResult.Ready();

            string[] waitingKeys = _writeItems
                .Where(item => item.Definition.ValueSource == PlcWriteValueSource.Input &&
                               (item.Option == null ||
                                 (!context.IsInputActivated(item.Option) &&
                                  !GlobalDataNode.IsReadSource(item.Option)) ||
                                item.Option.Data == null))
                .Select(item => item.Definition.Key)
                .ToArray();
            return waitingKeys.Length == 0
                ? EditorNodeReadinessResult.Ready()
                : EditorNodeReadinessResult.NotReady(
                    "等待写入输入：" + string.Join("、", waitingKeys) + "。");
        }

        public int CompareReadValue(object actualValue, string expectedValue)
        {
            if (actualValue == null)
                throw new ArgumentNullException(nameof(actualValue));

            switch (ReadValueType)
            {
                case PlcReadValueType.Int:
                    return Convert.ToInt32(actualValue, CultureInfo.InvariantCulture)
                        .CompareTo(ParseInt32(expectedValue));
                case PlcReadValueType.UInt:
                    return Convert.ToUInt32(actualValue, CultureInfo.InvariantCulture)
                        .CompareTo(ParseUInt32(expectedValue));
                case PlcReadValueType.Short:
                    return Convert.ToInt16(actualValue, CultureInfo.InvariantCulture)
                        .CompareTo(ParseInt16(expectedValue));
                case PlcReadValueType.UShort:
                    return Convert.ToUInt16(actualValue, CultureInfo.InvariantCulture)
                        .CompareTo(ParseUInt16(expectedValue));
                case PlcReadValueType.Long:
                    return Convert.ToInt64(actualValue, CultureInfo.InvariantCulture)
                        .CompareTo(ParseInt64(expectedValue));
                case PlcReadValueType.ULong:
                    return Convert.ToUInt64(actualValue, CultureInfo.InvariantCulture)
                        .CompareTo(ParseUInt64(expectedValue));
                case PlcReadValueType.String:
                    return StringComparer.Ordinal.Compare(
                        Convert.ToString(actualValue, CultureInfo.InvariantCulture), expectedValue);
                default:
                    throw new InvalidOperationException("不支持的读取类型。");
            }
        }

        public object ParseWriteValue()
        {
            return ConvertWriteValue(WriteValue, WriteValueType);
        }

        internal static object ConvertWriteValue(object value,
            PlcWriteValueType valueType)
        {
            if (value == null)
                throw new InvalidOperationException("写入值不能为空。");

            switch (valueType)
            {
                case PlcWriteValueType.Int:
                    return Convert.ToInt32(value, CultureInfo.InvariantCulture);
                case PlcWriteValueType.UInt:
                    return Convert.ToUInt32(value, CultureInfo.InvariantCulture);
                case PlcWriteValueType.Short:
                    return Convert.ToInt16(value, CultureInfo.InvariantCulture);
                case PlcWriteValueType.UShort:
                    return Convert.ToUInt16(value, CultureInfo.InvariantCulture);
                case PlcWriteValueType.Long:
                    return Convert.ToInt64(value, CultureInfo.InvariantCulture);
                case PlcWriteValueType.ULong:
                    return Convert.ToUInt64(value, CultureInfo.InvariantCulture);
                case PlcWriteValueType.Double:
                    return Convert.ToDouble(value, CultureInfo.InvariantCulture);
                case PlcWriteValueType.Float:
                    return Convert.ToSingle(value, CultureInfo.InvariantCulture);
                case PlcWriteValueType.Byte:
                    return Convert.ToByte(value, CultureInfo.InvariantCulture);
                case PlcWriteValueType.String:
                    return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
                default:
                    throw new InvalidOperationException("不支持的写入类型。");
            }
        }

        protected override bool IsPropertyVisible(string propertyName)
        {
            if (propertyName == nameof(WriteValueType) ||
                propertyName == nameof(WriteValue))
                return false;

            if (_operationMode == PlcOperationMode.Read)
                return propertyName != nameof(WriteItems);

            return propertyName != nameof(Address) &&
                   propertyName != nameof(ReadValueType) &&
                   propertyName != nameof(ReadLength) &&
                   propertyName != nameof(ComparisonOutputs);
        }

        protected override void OnLoadNode(Dictionary<string, byte[]> dic)
        {
            base.OnLoadNode(dic);
            if (_operationMode != PlcOperationMode.Write ||
                _writeItems.Count > 0 || string.IsNullOrWhiteSpace(Address))
                return;

            SetWriteItems(PlcWriteItemSerializer.Serialize(new[]
            {
                new PlcWriteItemDefinition
                {
                    Key = "写入1",
                    Address = Address.Trim(),
                    ValueType = WriteValueType,
                    ValueSource = PlcWriteValueSource.FixedValue,
                    FixedValue = WriteValue ?? string.Empty
                }
            }));
        }

        private void SetComparisonOutputs(string value)
        {
            List<ComparisonDefinition> definitions = ParseComparisonDefinitions(value);
            var desiredLabels = new HashSet<string>(
                definitions.Select(item => item.Label), StringComparer.Ordinal);

            foreach (ComparisonOutput existing in _comparisonOutputs.ToArray())
            {
                if (desiredLabels.Contains(existing.Label)) continue;
                if (OutputOptions.Contains(existing.Option))
                    OutputOptions.Remove(existing.Option);
                _comparisonOutputs.Remove(existing);
            }

            foreach (ComparisonDefinition definition in definitions)
            {
                if (_comparisonOutputs.Any(item => item.Label == definition.Label)) continue;
                _comparisonOutputs.Add(new ComparisonOutput(definition.Operator,
                    definition.ExpectedValue,
                    new STNodeOption(definition.Label, typeof(object), false)));
            }

            _comparisonOutputText = string.Join(";",
                definitions.Select(item => item.Label).ToArray());
            if (_operationMode == PlcOperationMode.Read)
                ApplyComparisonOutputs();
        }

        private void SetWriteItems(string value)
        {
            List<PlcWriteItemDefinition> definitions =
                PlcWriteItemSerializer.Parse(value);
            var replacements = new List<WriteItem>();

            foreach (PlcWriteItemDefinition definition in definitions)
            {
                WriteItem existing = _writeItems.FirstOrDefault(item =>
                    item.Option != null && item.Definition.Key == definition.Key);
                STNodeOption option = existing == null
                    ? CreateWriteInput(definition.Key)
                    : existing.Option;
                replacements.Add(new WriteItem(definition, option));
            }

            var retainedOptions = new HashSet<STNodeOption>(
                replacements.Where(item => item.Option != null)
                    .Select(item => item.Option));
            foreach (WriteItem oldItem in _writeItems)
            {
                if (oldItem.Option == null || retainedOptions.Contains(oldItem.Option))
                    continue;
                oldItem.Option.DataTransfer -= WriteInputOnDataTransfer;
                if (InputOptions.Contains(oldItem.Option))
                    InputOptions.Remove(oldItem.Option);
            }

            _writeItems.Clear();
            _writeItems.AddRange(replacements);
            _writeItemsText = PlcWriteItemSerializer.Serialize(definitions);
            if (_operationMode == PlcOperationMode.Write)
                ApplyWriteInputs();
        }

        private static List<ComparisonDefinition> ParseComparisonDefinitions(string value)
        {
            var definitions = new List<ComparisonDefinition>();
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
                    throw new FormatException("比较条件必须以 >、= 或 < 开头。");

                int valueIndex = comparisonOperator == '=' &&
                                 token.StartsWith("==", StringComparison.Ordinal) ? 2 : 1;
                string expectedValue = token.Substring(valueIndex).Trim();
                if (expectedValue.Length == 0)
                    throw new FormatException("比较值不能为空。");
                string label = comparisonOperator + expectedValue;
                if (!labels.Add(label))
                    throw new FormatException("比较条件不能重复：" + label);
                definitions.Add(new ComparisonDefinition(comparisonOperator,
                    expectedValue, label));
            }
            return definitions;
        }

        private void ConfigurePorts()
        {
            while (InputOptions.Count > 0)
                InputOptions.RemoveAt(InputOptions.Count - 1);
            while (OutputOptions.Count > 0)
                OutputOptions.RemoveAt(OutputOptions.Count - 1);

            if (_operationMode == PlcOperationMode.Read)
            {
                InputOptions.Add(_readInput);
                ApplyComparisonOutputs();
                return;
            }

            ApplyWriteInputs();
        }

        private void ApplyComparisonOutputs()
        {
            foreach (ComparisonOutput output in _comparisonOutputs)
            {
                bool applicable = _readValueType != PlcReadValueType.String ||
                                  output.Operator == '=';
                bool isActive = OutputOptions.Contains(output.Option);
                if (applicable && !isActive)
                    OutputOptions.Add(output.Option);
                else if (!applicable && isActive)
                    OutputOptions.Remove(output.Option);
            }
            RefreshPortLayout();
        }

        private void ApplyWriteInputs()
        {
            var desiredOptions = new HashSet<STNodeOption>(
                _writeItems.Where(item => item.Option != null)
                    .Select(item => item.Option));
            foreach (STNodeOption current in InputOptions.Cast<STNodeOption>().ToArray())
            {
                if (!desiredOptions.Contains(current))
                    InputOptions.Remove(current);
            }

            foreach (WriteItem item in _writeItems)
            {
                if (item.Option != null && !InputOptions.Contains(item.Option))
                    InputOptions.Add(item.Option);
            }
            RefreshPortLayout();
        }

        private void RefreshPortLayout()
        {
            BuildSize(true, true, false);
            SetOptionsLocation();
            Invalidate();
        }

        private STNodeOption CreateWriteInput(string key)
        {
            var option = new STNodeOption(key, typeof(object), false);
            option.DataTransfer += WriteInputOnDataTransfer;
            return option;
        }

        private static void WriteInputOnDataTransfer(object sender,
            STNodeOptionEventArgs e)
        {
            var option = sender as STNodeOption;
            if (option == null) return;
            option.Data = e.Status == ConnectionStatus.Connected &&
                          e.TargetOption != null
                ? e.TargetOption.Data
                : null;
        }

        private PlcNodeExecutionResult ExecuteRead(PlcFrmVpCommunication plc,
            List<STNodeOption> targetOutputs)
        {
            switch (ReadValueType)
            {
                case PlcReadValueType.Int: return ReadAndCompare<int>(plc, targetOutputs);
                case PlcReadValueType.UInt: return ReadAndCompare<uint>(plc, targetOutputs);
                case PlcReadValueType.Short: return ReadAndCompare<short>(plc, targetOutputs);
                case PlcReadValueType.UShort: return ReadAndCompare<ushort>(plc, targetOutputs);
                case PlcReadValueType.Long: return ReadAndCompare<long>(plc, targetOutputs);
                case PlcReadValueType.ULong: return ReadAndCompare<ulong>(plc, targetOutputs);
                case PlcReadValueType.String: return ReadAndCompare<string>(plc, targetOutputs);
                default: throw new InvalidOperationException("不支持的读取类型。");
            }
        }

        private PlcNodeExecutionResult ReadAndCompare<T>(PlcFrmVpCommunication plc,
            List<STNodeOption> targetOutputs)
        {
            var readResult = plc.Read<T>(Address, (ushort)ReadLength);
            if (!readResult.IsSuccess)
            {
                return new PlcNodeExecutionResult
                {
                    IsSuccess = false,
                    ShouldContinue = false,
                    MatchedOutputs = new string[0],
                    WriteResults = new PlcWriteItemExecutionResult[0],
                    Message = readResult.Message
                };
            }

            if (readResult.Data == null || readResult.Data.Length == 0)
            {
                return new PlcNodeExecutionResult
                {
                    IsSuccess = false,
                    ShouldContinue = false,
                    MatchedOutputs = new string[0],
                    WriteResults = new PlcWriteItemExecutionResult[0],
                    Message = "PLC 读取成功，但没有返回数据。"
                };
            }

            object value = readResult.Data[0];
            foreach (ComparisonOutput output in _comparisonOutputs)
            {
                if (_readValueType == PlcReadValueType.String && output.Operator != '=')
                    continue;
                int comparison = CompareReadValue(value, output.ExpectedValue);
                if (IsMatch(comparison, output.Operator))
                    targetOutputs.Add(output.Option);
            }

            string[] matchedLabels = targetOutputs.Select(item => item.Text).ToArray();
            bool matched = matchedLabels.Length > 0;
            return new PlcNodeExecutionResult
            {
                IsSuccess = true,
                ShouldContinue = matched,
                ComparisonMatched = matched,
                MatchedOutput = matched ? matchedLabels[0] : null,
                MatchedOutputs = matchedLabels,
                WriteResults = new PlcWriteItemExecutionResult[0],
                Value = value,
                Message = matched
                    ? "读取值匹配输出 " + string.Join("、", matchedLabels) + "。"
                    : "读取值未匹配任何比较输出。"
            };
        }

        private PlcNodeExecutionResult ExecuteWrite(PlcFrmVpCommunication plc)
        {
            var preparedValues = new List<PreparedWrite>();
            foreach (WriteItem item in _writeItems)
            {
                object rawValue;
                if (item.Definition.ValueSource == PlcWriteValueSource.FixedValue)
                {
                    rawValue = item.Definition.FixedValue ?? string.Empty;
                }
                else
                {
                    if (item.Option == null || item.Option.Data == null)
                        throw new InvalidOperationException(
                            "写入输入 " + item.Definition.Key + " 没有接收到数据。");
                    rawValue = item.Option.Data;
                }

                object value = ConvertWriteValue(rawValue,
                    item.Definition.ValueType);
                preparedValues.Add(new PreparedWrite(item.Definition, value));
            }

            var results = new List<PlcWriteItemExecutionResult>();
            var values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (PreparedWrite prepared in preparedValues)
            {
                FrmMapper.Data.Result writeResult = WriteValueToPlc(plc,
                    prepared.Definition.Address,
                    prepared.Definition.ValueType,
                    prepared.Value);
                values.Add(prepared.Definition.Key, prepared.Value);
                results.Add(new PlcWriteItemExecutionResult
                {
                    Key = prepared.Definition.Key,
                    Address = prepared.Definition.Address,
                    ValueType = prepared.Definition.ValueType,
                    ValueSource = prepared.Definition.ValueSource,
                    Value = prepared.Value,
                    IsSuccess = writeResult.IsSuccess,
                    Message = writeResult.Message
                });

                if (!writeResult.IsSuccess)
                {
                    return new PlcNodeExecutionResult
                    {
                        IsSuccess = false,
                        ShouldContinue = false,
                        MatchedOutputs = new string[0],
                        WriteResults = results.ToArray(),
                        Value = values,
                        Message = "写入项 " + prepared.Definition.Key +
                                  " 写入失败：" + writeResult.Message
                    };
                }
            }

            return new PlcNodeExecutionResult
            {
                IsSuccess = true,
                ShouldContinue = false,
                MatchedOutputs = new string[0],
                WriteResults = results.ToArray(),
                Value = values,
                Message = "已完成 " + results.Count + " 个 PLC 写入项。"
            };
        }

        internal static FrmMapper.Data.Result WriteValueToPlc(
            PlcFrmVpCommunication plc, string address,
            PlcWriteValueType valueType, object value)
        {
            switch (valueType)
            {
                case PlcWriteValueType.Int: return plc.Write(address, (int)value);
                case PlcWriteValueType.UInt: return plc.Write(address, (uint)value);
                case PlcWriteValueType.Short: return plc.Write(address, (short)value);
                case PlcWriteValueType.UShort: return plc.Write(address, (ushort)value);
                case PlcWriteValueType.Long: return plc.Write(address, (long)value);
                case PlcWriteValueType.ULong: return plc.Write(address, (ulong)value);
                case PlcWriteValueType.Double: return plc.Write(address, (double)value);
                case PlcWriteValueType.Float: return plc.Write(address, (float)value);
                case PlcWriteValueType.Byte: return plc.Write(address, (byte)value);
                case PlcWriteValueType.String: return plc.Write(address, (string)value);
                default: throw new InvalidOperationException("不支持的写入类型。");
            }
        }

        private void ValidateConfiguration(PlcFrmVpCommunication plc)
        {
            if (plc == null) throw new ArgumentNullException(nameof(plc));
            if (string.IsNullOrWhiteSpace(PlcKey))
                throw new InvalidOperationException("PLC Key 不能为空。");

            if (_operationMode == PlcOperationMode.Read)
            {
                if (string.IsNullOrWhiteSpace(Address))
                    throw new InvalidOperationException("PLC 读取点位不能为空。");
                if (_comparisonOutputs.Count == 0)
                    throw new InvalidOperationException("至少需要配置一个比较输出。");
                return;
            }

            if (_writeItems.Count == 0)
                throw new InvalidOperationException("至少需要配置一个 PLC 写入项。");
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

        private static int ParseInt32(string value) =>
            int.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);

        private static uint ParseUInt32(string value) =>
            uint.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);

        private static short ParseInt16(string value) =>
            short.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);

        private static ushort ParseUInt16(string value) =>
            ushort.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);

        private static long ParseInt64(string value) =>
            long.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);

        private static ulong ParseUInt64(string value) =>
            ulong.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);

        private sealed class ComparisonDefinition
        {
            public ComparisonDefinition(char comparisonOperator,
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

        private sealed class ComparisonOutput
        {
            public ComparisonOutput(char comparisonOperator,
                string expectedValue, STNodeOption option)
            {
                Operator = comparisonOperator;
                ExpectedValue = expectedValue;
                Option = option;
            }

            public char Operator { get; }
            public string ExpectedValue { get; }
            public STNodeOption Option { get; }
            public string Label => Option.Text;
        }

        private sealed class WriteItem
        {
            public WriteItem(PlcWriteItemDefinition definition,
                STNodeOption option)
            {
                Definition = definition;
                Option = option;
            }

            public PlcWriteItemDefinition Definition { get; }
            public STNodeOption Option { get; }
        }

        private sealed class PreparedWrite
        {
            public PreparedWrite(PlcWriteItemDefinition definition, object value)
            {
                Definition = definition;
                Value = value;
            }

            public PlcWriteItemDefinition Definition { get; }
            public object Value { get; }
        }
    }
}
