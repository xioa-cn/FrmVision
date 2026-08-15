using System;
using System.IO;
using System.Linq;
using Cognex.VisionPro;
using FrmServices.Services.EditorServices;
using FrmVpComponents.Services;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public sealed class SaveImageNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public string FilePath { get; set; }
        public int RecordIndex { get; set; }
        public ICogRecord Record { get; set; }
        public ICogImage Image { get; set; }
        public string Message { get; set; }
    }

    [STNode("视觉图像", "xioa", null, null,
        "接收完整文件路径，以及相机图像或视觉记录集合并保存图片。")]
    public sealed class SaveImageNode : WorkflowNode, IEditorExecutableNode,
        IEditorNodeReadiness
    {
        public SaveImageNode()
        {
            SetNodeTypeTitle("图片保存");
            EnableExecutionLog = true;
            TitleColor = System.Drawing.Color.FromArgb(220, 22, 125, 103);
            LetGetOptions = true;

            FilePathInput = InputOptions.Add("路径和名称", typeof(object), true);
            ImageInput = InputOptions.Add("图像/视觉记录", typeof(object), true);
            FilePathInput.DataTransfer += InputOnDataTransfer;
            ImageInput.DataTransfer += InputOnDataTransfer;
        }

        public STNodeOption FilePathInput { get; }
        public STNodeOption ImageInput { get; }

        [STNodeProperty("记录索引", "需要保存的视觉子记录索引，从 0 开始。")]
        public int RecordIndex { get; set; }

        [STNodeProperty("图片格式", "选择保存图片的文件格式。")]
        public ImageFileType ImageType { get; set; } = ImageFileType.Bmp;

        [STNodeProperty("保存视觉图形",
            "开启后，视觉记录输入将保存检测框、文字等叠加图形；相机图片输入仍保存原图。")]
        public bool SaveWithGraphics { get; set; }

        public SaveImageNodeExecutionResult LastExecutionResult
        {
            get;
            private set;
        }

        public SaveImageNodeExecutionResult Execute()
        {
            ICogRecord selectedRecord = null;
            ICogImage image = null;
            string normalizedFilePath = string.Empty;
            try
            {
                string filePath = FilePathInput.Data as string;
                if (string.IsNullOrWhiteSpace(filePath))
                    throw new InvalidOperationException(
                        "图片保存节点的路径和名称不能为空。");

                image = ResolveImage(ImageInput.Data, out selectedRecord);

                string directory;
                string imageName;
                normalizedFilePath = ParseFilePath(filePath, out directory,
                    out imageName, ImageType);

                bool saveRecordGraphics = SaveWithGraphics &&
                                          selectedRecord != null;
                string errorMessage;
                bool saved = saveRecordGraphics
                    ? ImageTools.TrySaveRecordWithGraphics(selectedRecord,
                        normalizedFilePath, ImageType, out errorMessage)
                    : ImageTools.TrySaveImage(image, directory, ImageType,
                        imageName, out errorMessage);
                if (!saved)
                    throw new InvalidOperationException(errorMessage);

                LastExecutionResult = new SaveImageNodeExecutionResult
                {
                    IsSuccess = true,
                    FilePath = normalizedFilePath,
                    RecordIndex = RecordIndex,
                    Record = selectedRecord,
                    Image = image,
                    Message = saveRecordGraphics
                        ? "带视觉图形的图片已保存：" + normalizedFilePath
                        : "图片已保存：" + normalizedFilePath
                };
            }
            catch (Exception ex)
            {
                LastExecutionResult = new SaveImageNodeExecutionResult
                {
                    IsSuccess = false,
                    FilePath = normalizedFilePath,
                    RecordIndex = RecordIndex,
                    Record = selectedRecord,
                    Image = image,
                    Message = ex.GetBaseException().Message
                };
            }

            return LastExecutionResult;
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();

            SaveImageNodeExecutionResult result = Execute();
            return result.IsSuccess
                ? EditorNodeExecutionResult.Success(result.Message)
                : EditorNodeExecutionResult.Failure(result.Message);
        }

        public EditorNodeReadinessResult CanExecute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            bool pathReady = IsPathInputReady(context);
            bool imageReady = context.IsInputActivated(ImageInput) ||
                              GlobalDataNode.IsReadSource(ImageInput);
            if (pathReady && imageReady)
                return EditorNodeReadinessResult.Ready();
            if (!pathReady && !imageReady)
                return EditorNodeReadinessResult.NotReady(
                    "等待路径和名称、图像或视觉记录。");
            return EditorNodeReadinessResult.NotReady(pathReady
                ? "等待图像或视觉记录。"
                : "等待路径和名称。");
        }

        private ICogImage ResolveImage(object input,
            out ICogRecord selectedRecord)
        {
            selectedRecord = null;
            var directImage = input as ICogImage;
            if (directImage != null) return directImage;

            var records = input as ICogRecords;
            if (records == null)
                throw new InvalidOperationException(
                    "图片输入必须是 ICogImage 或 ICogRecords。");
            if (records.Count == 0)
                throw new InvalidOperationException("视觉记录集合为空。");
            if (RecordIndex < 0 || RecordIndex >= records.Count)
                throw new InvalidOperationException("记录索引 " + RecordIndex +
                    " 超出范围，当前记录数量为 " + records.Count + "。");
            
            selectedRecord = records[RecordIndex];
            if (selectedRecord == null)
                throw new InvalidOperationException("索引 " + RecordIndex +
                    " 对应的视觉记录为空。");
            var recordImage =selectedRecord.Content as ICogImage;
            if (recordImage == null)
                throw new InvalidOperationException("索引 " + RecordIndex +
                    " 对应记录的 Content 不是视觉图片。");
            return recordImage;
        }

        private bool IsPathInputReady(EditorExecutionContext context)
        {
            bool constantInput = FilePathInput.GetConnectedOption().Any(option =>
                option != null && option.Owner is StringNode);
            return constantInput || GlobalDataNode.IsReadSource(FilePathInput) ||
                   context.IsInputActivated(FilePathInput);
        }

        private static string ParseFilePath(string filePath,
            out string directory, out string imageName,
            ImageFileType imageType)
        {
            string fullPath = Path.GetFullPath(filePath.Trim());
            directory = Path.GetDirectoryName(fullPath);
            if (string.IsNullOrWhiteSpace(directory))
                throw new InvalidOperationException("图片保存路径必须包含目录。");

            string extension = Path.GetExtension(fullPath);
            if (string.IsNullOrEmpty(extension))
            {
                imageName = Path.GetFileName(fullPath);
                return fullPath + GetCanonicalExtension(imageType);
            }

            string normalizedExtension = extension.TrimStart('.').ToLowerInvariant();
            bool hasImageExtension = normalizedExtension == "bmp" ||
                                     normalizedExtension == "jpg" ||
                                     normalizedExtension == "jpeg" ||
                                     normalizedExtension == "png" ||
                                     normalizedExtension == "tif" ||
                                     normalizedExtension == "tiff";
            imageName = hasImageExtension
                ? Path.GetFileNameWithoutExtension(fullPath)
                : Path.GetFileName(fullPath);

            if (imageName.Length == 0)
                throw new InvalidOperationException("图片文件名不能为空。");
            return Path.Combine(directory, imageName +
                GetCanonicalExtension(imageType));
        }

        private static string GetCanonicalExtension(ImageFileType imageType)
        {
            switch (imageType)
            {
                case ImageFileType.Bmp: return ".BMP";
                case ImageFileType.Jpg: return ".JPG";
                case ImageFileType.Png: return ".PNG";
                case ImageFileType.Tiff: return ".TIFF";
                default: throw new ArgumentOutOfRangeException(nameof(imageType));
            }
        }

        private static void InputOnDataTransfer(object sender,
            STNodeOptionEventArgs e)
        {
            var input = sender as STNodeOption;
            if (input == null) return;
            input.Data = e.Status == ConnectionStatus.Connected &&
                         e.TargetOption != null
                ? e.TargetOption.Data
                : null;
        }
    }
}
