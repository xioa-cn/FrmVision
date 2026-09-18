using System;
using System.Globalization;
using FrmMapper.Data;
using FrmServices.Services.CommunicationServices;
using FrmServices.Services.EditorServices;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    [STNode("内置通讯", "xioa", "1327916255@qq.com", "https://github.com/xioa-cn/",
        "读取或写入内置 Modbus RTU 服务端的数据区，与外部 Modbus 客户端共享线圈和寄存器数据。")]
    // 作者：xioa
    // 作者邮箱：1327916255@qq.com
    public sealed class CoCommModRtuSerNode : WorkflowNode, IEditorExecutableNode, IEditorNodeReadiness
    {
        public CoCommModRtuSerNode()
        {
            SetNodeTypeTitle("Modbus RTU服务器");
            TitleColor = System.Drawing.Color.FromArgb(220, 0, 132, 137);
            LetGetOptions = true;
            Input = InputOptions.Add("输入", typeof(object), true);
            Output = OutputOptions.Add("输出", typeof(object), false);
            Input.DataTransfer += InputOnDataTransfer;
        }

        public STNodeOption Input { get; }
        public STNodeOption Output { get; }

        [STNodeProperty("通讯名称", "内置通讯中已启动的 Modbus RTU 从站名称或配置 ID；复用其数据区、站号和数据格式。")]
        public string CommunicationKey { get; set; } = string.Empty;

        [STNodeProperty("操作模式", "Read：输入触发后立即读取本地数据区并输出；Write：将上游值或固定值写入本地数据区，成功后输出写入值。")]
        public ModbusServerOperationMode OperationMode { get; set; } = ModbusServerOperationMode.Read;

        [STNodeProperty("地址", "从 0 开始的地址，例如 100；Bool 默认访问线圈，数值默认访问保持寄存器。离散输入用 x=2;100，输入寄存器用 x=4;100；服务端本地均可读写。")]
        public string Address { get; set; } = "0";

        [STNodeProperty("数据类型",
            "Bool 为布尔量；Short/UShort 为 16 位，Int/UInt/Float 为 32 位，Long/ULong/Double 为 64 位；String 使用 ASCII。")]
        public ModbusServerValueType ValueType { get; set; } = ModbusServerValueType.UShort;

        [STNodeProperty("读取长度", "Bool/数值按元素数量读取，1 输出单值，大于 1 输出数组；String 按寄存器数量读取，每个寄存器 2 个 ASCII 字符。写入数量由写入值决定。")]
        public int ReadLength { get; set; } = 1;

        [STNodeProperty("写入取值方式", "Input：使用上游输入值；FixedValue：使用固定值，输入口仅作触发。支持单值和一维数组。")]
        public ModbusServerWriteValueSource ValueSource { get; set; } = ModbusServerWriteValueSource.Input;

        [STNodeProperty("固定值", "布尔值填 true/false 或 1/0；数值数组用英文逗号分隔，如 1,2,3；小数点用 .。String 将整段文本写入，不拆分逗号。")]
        public string FixedValue { get; set; } = "0";

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Output.Data = null;
            context.CancellationToken.ThrowIfCancellationRequested();
            try
            {
                if (!Enum.IsDefined(typeof(ModbusServerOperationMode), OperationMode))
                    throw new InvalidOperationException("Modbus 操作模式无效。");
                if (!Enum.IsDefined(typeof(ModbusServerValueType), ValueType))
                    throw new InvalidOperationException("Modbus 数据类型无效。");
                if (string.IsNullOrWhiteSpace(Address)) throw new InvalidOperationException("Modbus 地址不能为空。");
                if (OperationMode == ModbusServerOperationMode.Read) ValidateCount(ReadLength);
                else if (!Enum.IsDefined(typeof(ModbusServerWriteValueSource), ValueSource))
                    throw new InvalidOperationException("Modbus 写入取值方式无效。");

                var server = context.ResolveModbusRtuServer(CommunicationKey);
                object value;
                switch (ValueType)
                {
                    case ModbusServerValueType.Bool: value = ExecuteTyped<bool>(server); break;
                    case ModbusServerValueType.Short: value = ExecuteTyped<short>(server); break;
                    case ModbusServerValueType.UShort: value = ExecuteTyped<ushort>(server); break;
                    case ModbusServerValueType.Int: value = ExecuteTyped<int>(server); break;
                    case ModbusServerValueType.UInt: value = ExecuteTyped<uint>(server); break;
                    case ModbusServerValueType.Long: value = ExecuteTyped<long>(server); break;
                    case ModbusServerValueType.ULong: value = ExecuteTyped<ulong>(server); break;
                    case ModbusServerValueType.Float: value = ExecuteTyped<float>(server); break;
                    case ModbusServerValueType.Double: value = ExecuteTyped<double>(server); break;
                    case ModbusServerValueType.String: value = ExecuteString(server); break;
                    default: throw new InvalidOperationException("Modbus 数据类型无效。");
                }

                context.CancellationToken.ThrowIfCancellationRequested();
                Output.Data = value;
                Output.TransferData();
                return EditorNodeExecutionResult.Success("Modbus RTU 服务端“" + CommunicationKey.Trim() + "”地址 " +
                                                         Address.Trim() +
                                                         (OperationMode == ModbusServerOperationMode.Read
                                                             ? " 读取成功。"
                                                             : " 写入成功。"), Output);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return EditorNodeExecutionResult.Failure(ex.GetBaseException().Message);
            }
        }

        private object ExecuteTyped<T>(ContentModbusRtuServer server)
        {
            if (OperationMode == ModbusServerOperationMode.Read)
            {
                var result = server.Read<T>(Address.Trim(), (short)ReadLength);
                EnsureSuccess(result);
                if (result.Data == null || result.Data.Length != ReadLength)
                    throw new InvalidOperationException("Modbus 返回的数据长度不正确。");
                return ReadLength == 1 ? (object)result.Data[0] : result.Data;
            }

            object source = GetWriteValue();
            // Convert the entire array before writing to avoid partial updates on invalid input.
            Array items = source as Array;
            if (source is string text && text.Contains(",")) items = text.Split(',');
            if (items != null)
            {
                if (items.Rank != 1) throw new InvalidOperationException("Modbus 写入值只支持一维数组。");
                ValidateCount(items.Length);
                var values = new T[items.Length];
                int index = 0;
                foreach (object item in items) values[index++] = ConvertValue<T>(item);
                EnsureSuccess(server.Write(Address.Trim(), values));
                return values;
            }

            T value = ConvertValue<T>(source);
            EnsureSuccess(server.Write(Address.Trim(), value));
            return value;
        }

        private string ExecuteString(ContentModbusRtuServer server)
        {
            if (OperationMode == ModbusServerOperationMode.Read)
            {
                var result = server.Read<string>(Address.Trim(), (short)ReadLength);
                EnsureSuccess(result);
                if (result.Data == null || result.Data.Length != 1)
                    throw new InvalidOperationException("Modbus 未返回有效的字符串。");
                return result.Data[0];
            }

            object source = GetWriteValue();
            if (!(source is string) && !(source is IConvertible))
                throw new InvalidOperationException("String 写入值必须是文本或可转换为文本的单值。");
            string value = Convert.ToString(source, CultureInfo.InvariantCulture);
            if (string.IsNullOrEmpty(value)) throw new InvalidOperationException("Modbus 写入文本不能为空。");
            if (value.Length > 65534) throw new InvalidOperationException("Modbus 写入文本过长。");
            foreach (char character in value)
                if (character > 127)
                    throw new InvalidOperationException("Modbus String 使用 ASCII，请勿写入中文等非 ASCII 字符。");
            EnsureSuccess(server.Write(Address.Trim(), value));
            return value;
        }

        private object GetWriteValue()
        {
            object value = ValueSource == ModbusServerWriteValueSource.Input ? Input.Data : FixedValue;
            if (value == null) throw new InvalidOperationException("Modbus 写入值不能为空。");
            return value;
        }

        private static T ConvertValue<T>(object value)
        {
            if (value == null) throw new InvalidOperationException("Modbus 写入数组不能包含空值。");
            string text = Convert.ToString(value, CultureInfo.InvariantCulture)?.Trim();
            if (typeof(T) == typeof(bool))
            {
                if (text == "1") return (T)(object)true;
                if (text == "0") return (T)(object)false;
                if (bool.TryParse(text, out bool boolean)) return (T)(object)boolean;
                throw new FormatException("布尔值必须为 true、false、1 或 0。");
            }

            // Parse integers without silently rounding fractional inputs.
            T converted = (T)Convert.ChangeType(text, typeof(T), CultureInfo.InvariantCulture);
            if (converted is float single && (float.IsNaN(single) || float.IsInfinity(single)) ||
                converted is double number && (double.IsNaN(number) || double.IsInfinity(number)))
                throw new FormatException("Modbus 写入数值必须是有限数值。");
            return converted;
        }

        private void ValidateCount(int count)
        {
            int registersPerValue = ValueType == ModbusServerValueType.Long ||
                                    ValueType == ModbusServerValueType.ULong ||
                                    ValueType == ModbusServerValueType.Double ? 4 :
                ValueType == ModbusServerValueType.Int || ValueType == ModbusServerValueType.UInt ||
                ValueType == ModbusServerValueType.Float ? 2 : 1;
            if (count < 1 || count > short.MaxValue || (long)count * registersPerValue > ushort.MaxValue)
                throw new InvalidOperationException("Modbus 长度必须大于 0，且不能超过当前数据类型支持的数据区长度。");
        }

        private static void EnsureSuccess(Result result)
        {
            if (!result.IsSuccess) throw new InvalidOperationException("Modbus 操作失败：" + result.Message);
        }

        public EditorNodeReadinessResult CanExecute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (!context.IsInputActivated(Input) &&
                !(Input.GetConnectedOption() != null && GlobalDataNode.IsReadSource(Input)))
                return EditorNodeReadinessResult.NotReady("等待输入触发信号。");
            if (OperationMode == ModbusServerOperationMode.Write && ValueSource == ModbusServerWriteValueSource.Input &&
                Input.Data == null)
                return EditorNodeReadinessResult.NotReady("等待需要写入的输入值。");
            return EditorNodeReadinessResult.Ready();
        }

        private void InputOnDataTransfer(object sender, STNodeOptionEventArgs e)
        {
            Input.Data = e.Status == ConnectionStatus.Connected && e.TargetOption != null ? e.TargetOption.Data : null;
        }
    }
}