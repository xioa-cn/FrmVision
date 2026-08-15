using System;
using FrmMapper.Data;
using FrmServices.Communication;
using FrmServices.Services.EditorServices;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public sealed class PlcWriteTransmitNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public string PlcKey { get; set; }
        public string Address { get; set; }
        public PlcWriteValueType ValueType { get; set; }
        public PlcWriteValueSource ValueSource { get; set; }
        public object Value { get; set; }
        public string Message { get; set; }
    }

    [STNode("设备通讯", "xioa", null, null,
        "将输入值或固定值写入 PLC，写入成功后把实际写入值传递给下游。")]
    public sealed class PlcWriteTransmitNode : WorkflowNode,
        IEditorExecutableNode, IEditorNodeReadiness
    {
        public PlcWriteTransmitNode()
        {
            SetNodeTypeTitle("写值传递");
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

        [STNodeProperty("写入地址", "需要写入的 PLC 点位地址。")]
        public string Address { get; set; } = string.Empty;

        [STNodeProperty("写入类型", "写入 PLC 前使用的数据类型。")]
        public PlcWriteValueType ValueType { get; set; } = PlcWriteValueType.Int;

        [STNodeProperty("取值方式", "Input 使用输入值，FixedValue 使用固定值。")]
        public PlcWriteValueSource ValueSource { get; set; } =
            PlcWriteValueSource.Input;

        [STNodeProperty("固定值", "取值方式为 FixedValue 时写入的值。")]
        public string FixedValue { get; set; } = string.Empty;

        public PlcWriteTransmitNodeExecutionResult Execute(
            PlcFrmVpCommunication plc)
        {
            object value = null;
            try
            {
                ValidateConfiguration(plc);
                object rawValue = ValueSource == PlcWriteValueSource.Input
                    ? Input.Data
                    : (object)(FixedValue ?? string.Empty);
                value = PlcNode.ConvertWriteValue(rawValue, ValueType);
                Result writeResult = PlcNode.WriteValueToPlc(
                    plc, Address.Trim(), ValueType, value);
                if (writeResult == null || !writeResult.IsSuccess)
                {
                    return CreateResult(false, value,
                        writeResult == null || string.IsNullOrWhiteSpace(writeResult.Message)
                            ? "PLC 写入失败。"
                            : writeResult.Message);
                }

                Output.Data = value;
                Output.TransferData();
                return CreateResult(true, value,
                    "PLC 点位 " + Address.Trim() + " 写入成功，写入值：" +
                    Convert.ToString(value,
                        System.Globalization.CultureInfo.InvariantCulture) + "。");
            }
            catch (Exception ex)
            {
                return CreateResult(false, value, ex.GetBaseException().Message);
            }
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();
            PlcWriteTransmitNodeExecutionResult result =
                Execute(context.ResolvePlc(PlcKey));
            return result.IsSuccess
                ? EditorNodeExecutionResult.Success(result.Message, Output)
                : EditorNodeExecutionResult.Failure(result.Message);
        }

        public EditorNodeReadinessResult CanExecute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (!context.IsInputActivated(Input) &&
                !GlobalDataNode.IsReadSource(Input))
                return EditorNodeReadinessResult.NotReady("等待输入触发信号。");
            if (ValueSource == PlcWriteValueSource.Input && Input.Data == null)
                return EditorNodeReadinessResult.NotReady("等待需要写入的输入值。");
            return EditorNodeReadinessResult.Ready();
        }

        private void ValidateConfiguration(PlcFrmVpCommunication plc)
        {
            if (plc == null) throw new ArgumentNullException(nameof(plc));
            if (string.IsNullOrWhiteSpace(PlcKey))
                throw new InvalidOperationException("PLC Key 不能为空。");
            if (string.IsNullOrWhiteSpace(Address))
                throw new InvalidOperationException("PLC 写入地址不能为空。");
            if (ValueSource == PlcWriteValueSource.Input && Input.Data == null)
                throw new InvalidOperationException("PLC 写入输入值不能为空。");
        }

        private PlcWriteTransmitNodeExecutionResult CreateResult(bool isSuccess,
            object value, string message)
        {
            return new PlcWriteTransmitNodeExecutionResult
            {
                IsSuccess = isSuccess,
                PlcKey = (PlcKey ?? string.Empty).Trim(),
                Address = (Address ?? string.Empty).Trim(),
                ValueType = ValueType,
                ValueSource = ValueSource,
                Value = value,
                Message = message ?? string.Empty
            };
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
