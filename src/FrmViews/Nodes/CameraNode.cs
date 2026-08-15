using System;
using System.Windows.Forms;
using Cognex.VisionPro;
using FrmServices.Services.EditorServices;
using FrmServices.Utils;
using FrmVpComponents.Services;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public sealed class CameraNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public bool ShouldContinue { get; set; }
        public string ProductionKey { get; set; }
        public string CameraKey { get; set; }
        public ICogImage Image { get; set; }
        public string Message { get; set; }
    }

    [STNode("视觉图像", "xioa", null, null,
        "运行指定的 VisionPro 相机工具，并将 ICogImage 输出给后续节点。")]
    public class CameraNode : WorkflowNode, IEditorExecutableNode
    {
        public CameraNode()
        {
            SetNodeTypeTitle("相机工具");
            EnableExecutionLog = true;
            TitleColor = System.Drawing.Color.FromArgb(220, 30, 120, 180);
            LetGetOptions = true;
            Input = InputOptions.Add("输入", typeof(object), false);
            OutputImage = OutputOptions.Add("图像", typeof(ICogImage), false);
            InitializeToolWindowMenu();
        }

        public STNodeOption Input { get; }
        public STNodeOption OutputImage { get; }

        [STNodeProperty("产品 Key", "BlockTool 中已加载相机工具所属的产品 Key。")]
        public string ProductionKey { get; set; } = string.Empty;

        [STNodeProperty("相机 Key", "产品下 CogAcqFifoTool 的唯一 Key。")]
        public string CameraKey { get; set; } = string.Empty;

        public CameraNodeExecutionResult Execute()
        {
            return Execute(BlockTool.Instance);
        }

        public CameraNodeExecutionResult Execute(BlockTool blockTool)
        {
            CameraNodeExecutionResult executionResult;
            try
            {
                ValidateConfiguration(blockTool);
                ICogImage image = blockTool.UseCogAcqFifo(
                    ProductionKey,
                    CameraKey,
                    camera =>
                    {
                        camera.Run();
                        if (camera.RunStatus != null &&
                            camera.RunStatus.Exception != null)
                            throw new InvalidOperationException(
                                "相机采图失败。", camera.RunStatus.Exception);
                        return camera.OutputImage;
                    });

                if (image == null)
                    throw new InvalidOperationException("相机采图完成，但没有输出图像。");

                executionResult = new CameraNodeExecutionResult
                {
                    IsSuccess = true,
                    ShouldContinue = true,
                    ProductionKey = ProductionKey,
                    CameraKey = CameraKey,
                    Image = image,
                    Message = "相机采图成功。"
                };

                OutputImage.Data = image;
                OutputImage.TransferData();
            }
            catch (Exception ex)
            {
                executionResult = new CameraNodeExecutionResult
                {
                    IsSuccess = false,
                    ShouldContinue = false,
                    ProductionKey = ProductionKey,
                    CameraKey = CameraKey,
                    Message = ex.GetBaseException().Message
                };
            }

            return executionResult;
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();
            CameraNodeExecutionResult result = Execute();
            return result.IsSuccess
                ? EditorNodeExecutionResult.Success(result.Message, OutputImage)
                : EditorNodeExecutionResult.Failure(result.Message);
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
            var openItem = new ToolStripMenuItem("打开相机工具窗口");
            openItem.Click += (sender, args) => OpenToolWindow();
            menu.Items.Add(openItem);
            ContextMenuStrip = menu;
        }

        private void OpenToolWindow()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ProductionKey))
                    throw new InvalidOperationException("产品 Key 不能为空。");
                if (string.IsNullOrWhiteSpace(CameraKey))
                    throw new InvalidOperationException("相机 Key 不能为空。");

                string parameterName = PictureUtils.GetCurrentParameterName(
                    ProductionKey);
                if (string.IsNullOrWhiteSpace(parameterName))
                    throw new InvalidOperationException("未找到产品“" +
                        ProductionKey.Trim() + "”当前正在使用的参数名称。");
                string path = ConfigDirUtils.GetCogAcqFifoUtilsDir(
                    ProductionKey.Trim(), parameterName.Trim(), CameraKey.Trim());
                BlockToolUtils.OpenCogAcqFifo(path, ProductionKey.Trim(),
                    CameraKey.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(Owner == null ? null : Owner.FindForm(),
                    "无法打开相机工具窗口：" + ex.GetBaseException().Message,
                    "相机工具", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ValidateConfiguration(BlockTool blockTool)
        {
            if (blockTool == null)
                throw new ArgumentNullException(nameof(blockTool));
            if (string.IsNullOrWhiteSpace(ProductionKey))
                throw new InvalidOperationException("产品 Key 不能为空。");
            if (string.IsNullOrWhiteSpace(CameraKey))
                throw new InvalidOperationException("相机 Key 不能为空。");
        }
    }
}
