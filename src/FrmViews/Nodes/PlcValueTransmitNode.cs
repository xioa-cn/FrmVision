using System;
using System.Globalization;
using FrmServices.Communication;
using FrmServices.Services.EditorServices;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public sealed class PlcValueTransmitNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public bool ShouldContinue { get; set; }
        public object Value { get; set; }
        public string Message { get; set; }
    }

    [STNode("设备通讯", "xioa", null, null,
        "读取指定 PLC 点位，并将读取值输出给下一个节点。")]
    public class PlcValueTransmitNode : WorkflowNode, IEditorExecutableNode,
        IEditorNodeReadiness
    {
        private int _readLength = 1;

        public PlcValueTransmitNode()
        {
            SetNodeTypeTitle("读值传递");
            EnableExecutionLog = true;
            TitleColor = System.Drawing.Color.FromArgb(220, 0, 132, 137);
            LetGetOptions = true;
            Input = InputOptions.Add("输入", typeof(object), true);
            Output = OutputOptions.Add("输出", typeof(object), false);
            Input.DataTransfer += InputOnDataTransfer;
        }

        public STNodeOption Input { get; }
        public STNodeOption Output { get; }

        [STNodeProperty("PLC Key", "用于查找 PLC 通讯实例的唯一 Key。")]
        public string PlcKey { get; set; } = string.Empty;

        [STNodeProperty("读取点位", "PLC 的读取地址。")]
        public string Address { get; set; } = string.Empty;

        [STNodeProperty("读取类型", "PLC 点位的数据类型。")]
        public PlcReadValueType ReadValueType { get; set; } = PlcReadValueType.Int;

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

        public PlcValueTransmitNodeExecutionResult Execute(
            PlcFrmVpCommunication plc)
        {
            try
            {
                ValidateConfiguration(plc);
                object value = ReadValue(plc);
                Output.Data = value;
                Output.TransferData();

                return new PlcValueTransmitNodeExecutionResult
                {
                    IsSuccess = true,
                    ShouldContinue = true,
                    Value = value,
                    Message = "PLC 点位 " + Address.Trim() + " 读取成功，值：" +
                              FormatValue(value) + "。"
                };
            }
            catch (Exception ex)
            {
                return new PlcValueTransmitNodeExecutionResult
                {
                    IsSuccess = false,
                    ShouldContinue = false,
                    Message = ex.GetBaseException().Message
                };
            }
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();
            PlcValueTransmitNodeExecutionResult result =
                Execute(context.ResolvePlc(PlcKey));
            return result.IsSuccess
                ? EditorNodeExecutionResult.Success(result.Message, Output)
                : EditorNodeExecutionResult.Failure(result.Message);
        }

        public EditorNodeReadinessResult CanExecute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            return context.IsInputActivated(Input)
                ? EditorNodeReadinessResult.Ready()
                : EditorNodeReadinessResult.NotReady("等待输入触发信号。");
        }

        private object ReadValue(PlcFrmVpCommunication plc)
        {
            switch (ReadValueType)
            {
                case PlcReadValueType.Int: return Read<int>(plc);
                case PlcReadValueType.UInt: return Read<uint>(plc);
                case PlcReadValueType.Short: return Read<short>(plc);
                case PlcReadValueType.UShort: return Read<ushort>(plc);
                case PlcReadValueType.Long: return Read<long>(plc);
                case PlcReadValueType.ULong: return Read<ulong>(plc);
                case PlcReadValueType.String: return Read<string>(plc);
                default: throw new InvalidOperationException("不支持的读取类型。");
            }
        }

        private object Read<T>(PlcFrmVpCommunication plc)
        {
            var readResult = plc.Read<T>(Address.Trim(), (ushort)ReadLength);
            if (!readResult.IsSuccess)
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(
                    readResult.Message) ? "PLC 读取失败。" : readResult.Message);
            if (readResult.Data == null || readResult.Data.Length == 0)
                throw new InvalidOperationException("PLC 读取成功，但没有返回数据。");

            return ReadLength == 1 ? (object)readResult.Data[0] : readResult.Data;
        }

        private void ValidateConfiguration(PlcFrmVpCommunication plc)
        {
            if (plc == null)
                throw new ArgumentNullException(nameof(plc));
            if (string.IsNullOrWhiteSpace(PlcKey))
                throw new InvalidOperationException("PLC Key 不能为空。");
            if (string.IsNullOrWhiteSpace(Address))
                throw new InvalidOperationException("PLC 读取点位不能为空。");
            if (ReadLength < 1 || ReadLength > ushort.MaxValue)
                throw new InvalidOperationException(
                    "读取长度必须在 1 到 65535 之间。");
        }

        private static string FormatValue(object value)
        {
            var values = value as Array;
            if (values == null)
                return Convert.ToString(value, CultureInfo.InvariantCulture) ??
                       string.Empty;

            var items = new string[values.Length];
            for (int index = 0; index < values.Length; index++)
            {
                items[index] = Convert.ToString(values.GetValue(index),
                    CultureInfo.InvariantCulture) ?? string.Empty;
            }
            return "[" + string.Join(", ", items) + "]";
        }

        private void InputOnDataTransfer(object sender, STNodeOptionEventArgs e)
        {
            Input.Data = e.Status == ConnectionStatus.Connected &&
                         e.TargetOption != null
                ? e.TargetOption.Data
                : null;
        }
    }
}
