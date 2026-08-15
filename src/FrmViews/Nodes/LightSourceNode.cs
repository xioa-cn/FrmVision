using System;
using System.Windows.Forms;
using FrmServices.Communication;
using FrmServices.Services.EditorServices;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public sealed class LightSourceNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public bool ShouldContinue { get; set; }
        public string LightSourceKey { get; set; }
        public string Command { get; set; }
        public string Message { get; set; }
    }

    [STNode("设备通讯", "xioa", null, null,
        "向指定光源发送字符串指令，输入和输出端口均支持多连接。")]
    public class LightSourceNode : WorkflowNode, IEditorExecutableNode
    {
        public LightSourceNode()
        {
            SetNodeTypeTitle("光源工具");
            TitleColor = System.Drawing.Color.FromArgb(220, 230, 126, 34);
            LetGetOptions = true;
            Input = InputOptions.Add("输入", typeof(object), false);
            Output = OutputOptions.Add("输出", typeof(object), false);
            InitializeTriggerMenu();
        }

        public STNodeOption Input { get; }
        public STNodeOption Output { get; }

        [STNodeProperty("光源 Key", "用于查找光源通讯实例的唯一 Key。")]
        public string LightSourceKey { get; set; } = string.Empty;

        [STNodeProperty("发送指令", "发送给光源的字符串指令内容。")]
        public string Command { get; set; } = string.Empty;

        public Func<string, LightSourceFrmVpCommunication> LightSourceResolver
        {
            get;
            set;
        }

        public LightSourceNodeExecutionResult Execute(
            LightSourceFrmVpCommunication lightSource)
        {
            LightSourceNodeExecutionResult executionResult;
            try
            {
                executionResult = SendCommand(lightSource);
            }
            catch (Exception ex)
            {
                executionResult = new LightSourceNodeExecutionResult
                {
                    IsSuccess = false,
                    ShouldContinue = false,
                    LightSourceKey = LightSourceKey,
                    Command = Command,
                    Message = ex.Message
                };
            }

            if (executionResult.ShouldContinue)
            {
                Output.Data = executionResult;
                Output.TransferData();
            }

            return executionResult;
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();
            LightSourceNodeExecutionResult result =
                Execute(context.ResolveLightSource(LightSourceKey));
            return result.IsSuccess
                ? EditorNodeExecutionResult.Success(result.Message, Output)
                : EditorNodeExecutionResult.Failure(result.Message);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.Button == MouseButtons.Left) TriggerCommand();
        }

        private void InitializeTriggerMenu()
        {
            var menu = new ContextMenuStrip();
            var triggerItem = new ToolStripMenuItem("触发指令");
            triggerItem.Click += (sender, args) => TriggerCommand();
            menu.Items.Add(triggerItem);
            ContextMenuStrip = menu;
        }

        private void TriggerCommand()
        {
            LightSourceNodeExecutionResult result;
            try
            {
                if (LightSourceResolver == null)
                    throw new InvalidOperationException("未配置光源通讯解析器。");

                string key = (LightSourceKey ?? string.Empty).Trim();
                if (key.Length == 0)
                    throw new InvalidOperationException("光源 Key 不能为空。");

                LightSourceFrmVpCommunication lightSource =
                    LightSourceResolver(key);
                if (lightSource == null)
                    throw new InvalidOperationException("未找到光源通讯实例：" + key + "。");

                result = SendCommand(lightSource);
            }
            catch (Exception ex)
            {
                result = new LightSourceNodeExecutionResult
                {
                    IsSuccess = false,
                    ShouldContinue = false,
                    LightSourceKey = LightSourceKey,
                    Command = Command,
                    Message = ex.GetBaseException().Message
                };
            }

            MessageBox.Show(Owner == null ? null : Owner.FindForm(),
                result.IsSuccess
                    ? $"指令已发送。{Command}" + FormatResultMessage(result.Message)
                    : "触发指令失败：" + result.Message,
                $"光源工具{LightSourceKey}", MessageBoxButtons.OK,
                result.IsSuccess ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        private void ValidateConfiguration(
            LightSourceFrmVpCommunication lightSource)
        {
            if (lightSource == null)
                throw new ArgumentNullException(nameof(lightSource));
            if (string.IsNullOrWhiteSpace(LightSourceKey))
                throw new InvalidOperationException("光源 Key 不能为空。");
            if (string.IsNullOrEmpty(Command))
                throw new InvalidOperationException("发送指令不能为空。");
        }

        private LightSourceNodeExecutionResult SendCommand(
            LightSourceFrmVpCommunication lightSource)
        {
            ValidateConfiguration(lightSource);
            var sendResult = lightSource.Write(string.Empty, Command);
            return new LightSourceNodeExecutionResult
            {
                IsSuccess = sendResult.IsSuccess,
                ShouldContinue = sendResult.IsSuccess,
                LightSourceKey = LightSourceKey,
                Command = Command,
                Message = sendResult.Message
            };
        }

        private static string FormatResultMessage(string message)
        {
            return string.IsNullOrWhiteSpace(message)
                ? string.Empty
                : Environment.NewLine + message;
        }
    }
}
