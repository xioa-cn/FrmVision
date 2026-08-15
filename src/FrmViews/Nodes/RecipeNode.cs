using System;
using System.IO;
using FrmCommon;
using FrmServices.Services.EditorServices;
using FrmVpComponents.Services;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public sealed class RecipeNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public bool ShouldContinue { get; set; }
        public string ProductionKey { get; set; }
        public string RecipeName { get; set; }
        public string RecipeDirectory { get; set; }
        public int CameraToolCount { get; set; }
        public int VisionToolCount { get; set; }
        public string Message { get; set; }
    }

    [STNode("流程控制", "xioa", null, null,
        "切换指定产品配方下的相机工具和视觉工具，输入和输出均支持多连接。")]
    public class RecipeNode : WorkflowNode, IEditorExecutableNode
    {
        public RecipeNode()
        {
            SetNodeTypeTitle("配方切换");
            TitleColor = System.Drawing.Color.FromArgb(220, 14, 116, 144);
            LetGetOptions = true;
            Input = InputOptions.Add("输入", typeof(object), false);
            Output = OutputOptions.Add("输出", typeof(object), false);
        }

        public STNodeOption Input { get; }
        public STNodeOption Output { get; }

        [STNodeProperty("产品 Key", "BlockTool 中相机工具和视觉工具所属的产品 Key。")]
        public string ProductionKey { get; set; } = string.Empty;

        [STNodeProperty("配方名称", "产品型录下需要切换到的配方目录名称。")]
        public string RecipeName { get; set; } = string.Empty;

        public RecipeNodeExecutionResult Execute()
        {
            return Execute(BlockTool.Instance);
        }

        public RecipeNodeExecutionResult Execute(BlockTool blockTool)
        {
            string recipeDirectory = null;
            try
            {
                ValidateConfiguration(blockTool);
                recipeDirectory = GetRecipeDirectory();
                string cameraToolDirectory = Path.Combine(
                    recipeDirectory, "相机工具");
                string visionToolDirectory = Path.Combine(
                    recipeDirectory, "视觉工具");
                RecipeToolSwitchResult switchResult = blockTool.SwitchRecipeTools(
                    ProductionKey, cameraToolDirectory, visionToolDirectory);

                var result = new RecipeNodeExecutionResult
                {
                    IsSuccess = true,
                    ShouldContinue = true,
                    ProductionKey = ProductionKey.Trim(),
                    RecipeName = RecipeName.Trim(),
                    RecipeDirectory = recipeDirectory,
                    CameraToolCount = switchResult.CameraToolCount,
                    VisionToolCount = switchResult.VisionToolCount,
                    Message = "配方切换成功：相机工具 " +
                              switchResult.CameraToolCount + " 个，视觉工具 " +
                              switchResult.VisionToolCount + " 个。"
                };
                Output.Data = result;
                Output.TransferData();
                return result;
            }
            catch (Exception ex)
            {
                return new RecipeNodeExecutionResult
                {
                    IsSuccess = false,
                    ShouldContinue = false,
                    ProductionKey = (ProductionKey ?? string.Empty).Trim(),
                    RecipeName = (RecipeName ?? string.Empty).Trim(),
                    RecipeDirectory = recipeDirectory,
                    Message = ex.GetBaseException().Message
                };
            }
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();
            RecipeNodeExecutionResult result = Execute();
            if (result.IsSuccess)
                context.NotifyRecipeChanged(result.ProductionKey, result.RecipeName);
            return result.IsSuccess
                ? EditorNodeExecutionResult.Success(result.Message, Output)
                : EditorNodeExecutionResult.Failure(result.Message);
        }

        private void ValidateConfiguration(BlockTool blockTool)
        {
            if (blockTool == null) throw new ArgumentNullException(nameof(blockTool));
            ValidateDirectoryName(ProductionKey, "产品 Key");
            ValidateDirectoryName(RecipeName, "配方名称");
            if (GlobalConfig.Instance == null ||
                string.IsNullOrWhiteSpace(GlobalConfig.Instance.ConfigCommonDir))
                throw new InvalidOperationException("未配置 ConfigCommonDir。");
        }

        private string GetRecipeDirectory()
        {
            string catalogDirectory = Path.GetFullPath(Path.Combine(
                GlobalConfig.Instance.ConfigCommonDir, "产品型录"));
            string productDirectory = GetChildDirectory(
                catalogDirectory, ProductionKey.Trim());
            string recipeDirectory = GetChildDirectory(
                productDirectory, RecipeName.Trim());
            if (!Directory.Exists(recipeDirectory))
                throw new DirectoryNotFoundException(
                    "配方目录不存在：" + recipeDirectory);
            return recipeDirectory;
        }

        private static string GetChildDirectory(string parentDirectory, string childName)
        {
            string parent = Path.GetFullPath(parentDirectory)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) +
                Path.DirectorySeparatorChar;
            string child = Path.GetFullPath(Path.Combine(parent, childName));
            if (!child.StartsWith(parent, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("配方目录超出产品型录范围。");
            return child;
        }

        private static void ValidateDirectoryName(string value, string propertyName)
        {
            string name = (value ?? string.Empty).Trim();
            if (name.Length == 0)
                throw new InvalidOperationException(propertyName + "不能为空。");
            if (!string.Equals(Path.GetFileName(name), name,
                    StringComparison.Ordinal) ||
                name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                throw new InvalidOperationException(
                    propertyName + "必须是有效的单级目录名称。");
        }
    }
}
