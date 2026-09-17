using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FrmServices.Services.EditorServices;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public enum SerialPortOperationMode
    {
        Read,
        Write
    }

    public enum SerialPortWriteValueSource
    {
        Input,
        FixedValue
    }

    [STNode("内置通讯", "xioa", null, null,
        "读取内置原始串口收到的文本并传递，或向已打开的串口发送输入值 / 固定文本。")]
    public sealed class CoCommSerialNode : WorkflowNode, IEditorExecutableNode, IEditorNodeReadiness
    {
        public CoCommSerialNode()
        {
            SetNodeTypeTitle("原始串口");
            TitleColor = System.Drawing.Color.FromArgb(220, 0, 132, 137);
            LetGetOptions = true;
            Input = InputOptions.Add("输入", typeof(object), true);
            Output = OutputOptions.Add("输出", typeof(object), false);
            Input.DataTransfer += InputOnDataTransfer;
        }

        public STNodeOption Input { get; }
        public STNodeOption Output { get; }

        [STNodeProperty("通讯名称", "内置通讯中的原始串口名称或配置 ID，例如 COM1；使用已经启动的串口。")]
        public string CommunicationKey { get; set; } = string.Empty;

        [STNodeProperty("操作模式", "Read：输入触发后一直等待接收数据并输出，可停止流程取消等待；Write：向已打开的串口发送文本。")]
        public SerialPortOperationMode OperationMode { get; set; } = SerialPortOperationMode.Read;

        [STNodeProperty("写入取值方式", "Input：将上游输入值转换为文本发送；FixedValue：发送固定文本，输入口仅用于触发。")]
        public SerialPortWriteValueSource ValueSource { get; set; } = SerialPortWriteValueSource.Input;

        [STNodeProperty("固定值", "写入取值方式为 FixedValue 时发送的文本；不自动添加换行，使用内置通讯配置中的编码。")]
        public string FixedValue { get; set; } = string.Empty;

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Output.Data = null;
            context.CancellationToken.ThrowIfCancellationRequested();
            try
            {
                if (!Enum.IsDefined(typeof(SerialPortOperationMode), OperationMode))
                    throw new InvalidOperationException("串口操作模式无效。");
                var port = context.ResolveSerialPort(CommunicationKey);
                string text;
                if (OperationMode == SerialPortOperationMode.Read)
                {
                    text = port.ReadMessage(0, context.CancellationToken);
                }
                else
                {
                    if (!Enum.IsDefined(typeof(SerialPortWriteValueSource), ValueSource))
                        throw new InvalidOperationException("串口写入取值方式无效。");
                    object value = ValueSource == SerialPortWriteValueSource.Input ? Input.Data : FixedValue;
                    if (value == null) throw new InvalidOperationException("串口写入值不能为空。");
                    if (!(value is string) && !(value is IConvertible) && !(value is IFormattable))
                        throw new InvalidOperationException("串口写入值必须是文本、数值或可转换为文本的标量。");
                    text = Convert.ToString(value, CultureInfo.InvariantCulture);
                    if (string.IsNullOrEmpty(text)) throw new InvalidOperationException("串口发送文本不能为空。");
                    Task send = port.Send(text);
                    _ = send.ContinueWith(task =>
                        {
                            var ignored = task.Exception;
                        }, CancellationToken.None,
                        TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
                        TaskScheduler.Default);
                    send.Wait(context.CancellationToken);
                    send.GetAwaiter().GetResult();
                }

                context.CancellationToken.ThrowIfCancellationRequested();
                Output.Data = text;
                Output.TransferData();
                return EditorNodeExecutionResult.Success("原始串口“" + CommunicationKey.Trim() + "”" +
                                                         (OperationMode == SerialPortOperationMode.Read ? "读取" : "发送") +
                                                         "成功，已传递文本。", Output);
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

        public EditorNodeReadinessResult CanExecute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (!context.IsInputActivated(Input) &&
                !(Input.GetConnectedOption() != null && GlobalDataNode.IsReadSource(Input)))
                return EditorNodeReadinessResult.NotReady("等待输入触发信号。");
            if (OperationMode == SerialPortOperationMode.Write && ValueSource == SerialPortWriteValueSource.Input &&
                Input.Data == null)
                return EditorNodeReadinessResult.NotReady("等待需要发送的输入值。");
            return EditorNodeReadinessResult.Ready();
        }

        private void InputOnDataTransfer(object sender, STNodeOptionEventArgs e)
        {
            Input.Data = e.Status == ConnectionStatus.Connected && e.TargetOption != null ? e.TargetOption.Data : null;
        }
    }
}