using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.ImageFile;
using FrmServices.LogServices;

namespace FrmVpComponents.Services
{
    public enum ImageFileType
    {
        Bmp,
        Jpg,
        Png,
        Tiff
    }

    /// <summary>
    /// VisionPro 图片和检测数据文件保存工具。
    /// </summary>
    public static class ImageTools
    {
        private static readonly object CsvWriteLock = new object();
        private static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

        public static void SaveImage(CogImage8Grey image, string path,
            string imageName)
        {
            SaveImageCore(image, path, imageName, ImageFileType.Bmp);
        }

        public static void SaveImage(CogImage24PlanarColor image, string path,
            string imageName)
        {
            SaveImageCore(image, path, imageName, ImageFileType.Bmp);
        }

        public static void SaveImage(CogImage24PlanarColor image, string path,
            ImageFileType imageType, string imageName)
        {
            SaveImageCore(image, path, imageName, imageType);
        }

        public static void SaveImage(CogImage8Grey image, string path,
            ImageFileType imageType, string imageName)
        {
            SaveImageCore(image, path, imageName, imageType);
        }

        public static bool TrySaveImage(ICogImage image, string path,
            ImageFileType imageType, string imageName, out string errorMessage)
        {
            try
            {
                if (image == null) throw new ArgumentNullException(nameof(image));

                string filePath = BuildFilePath(path, imageName,
                    GetExtension(imageType));
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                using (var imageFile = new CogImageFile())
                {
                    imageFile.Open(filePath, CogImageFileModeConstants.Write);
                    imageFile.Append(image);
                    imageFile.Close();
                }

                errorMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.GetBaseException().Message;
                LogSaveFailure("图片", path, imageName, ex);
                return false;
            }
        }

        [Obsolete("请使用 ImageFileType 枚举重载。")]
        public static void SaveImage(CogImage24PlanarColor image, string path,
            string imageType, string imageName)
        {
            ImageFileType parsedType;
            if (!TryParseImageFileType(imageType, out parsedType))
            {
                LogSaveFailure("图片", path, imageName,
                    new ArgumentException("不支持的图片格式：" + imageType,
                        nameof(imageType)));
                return;
            }

            SaveImage(image, path, parsedType, imageName);
        }

        [Obsolete("请使用 ImageFileType 枚举重载。")]
        public static void SaveImage(CogImage8Grey image, string path,
            string imageType, string imageName)
        {
            ImageFileType parsedType;
            if (!TryParseImageFileType(imageType, out parsedType))
            {
                LogSaveFailure("图片", path, imageName,
                    new ArgumentException("不支持的图片格式：" + imageType,
                        nameof(imageType)));
                return;
            }

            SaveImage(image, path, parsedType, imageName);
        }

        /// <summary>
        /// 追加保存一行 CSV 数据。文件使用 UTF-8 无 BOM 编码。
        /// </summary>
        public static void SaveCSV(string line, string path, string imageName)
        {
            try
            {
                string filePath = BuildFilePath(path, imageName, "csv");
                lock (CsvWriteLock)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    File.AppendAllText(filePath, (line ?? string.Empty) +
                        Environment.NewLine, Utf8NoBom);
                }
            }
            catch (Exception ex)
            {
                LogSaveFailure("CSV", path, imageName, ex);
            }
        }

        private static void SaveImageCore(ICogImage image, string path,
            string imageName, ImageFileType imageType)
        {
            string errorMessage;
            TrySaveImage(image, path, imageType, imageName, out errorMessage);
        }

        private static bool TryParseImageFileType(string imageType,
            out ImageFileType result)
        {
            switch ((imageType ?? string.Empty).Trim().TrimStart('.').ToLowerInvariant())
            {
                case "bmp":
                    result = ImageFileType.Bmp;
                    return true;
                case "jpg":
                case "jpeg":
                    result = ImageFileType.Jpg;
                    return true;
                case "png":
                    result = ImageFileType.Png;
                    return true;
                case "tif":
                case "tiff":
                    result = ImageFileType.Tiff;
                    return true;
                default:
                    result = ImageFileType.Bmp;
                    return false;
            }
        }

        private static string GetExtension(ImageFileType imageType)
        {
            switch (imageType)
            {
                case ImageFileType.Bmp: return "BMP";
                case ImageFileType.Jpg: return "JPG";
                case ImageFileType.Png: return "PNG";
                case ImageFileType.Tiff: return "TIFF";
                default:
                    throw new ArgumentOutOfRangeException(nameof(imageType),
                        imageType, "不支持的图片格式。");
            }
        }

        private static string BuildFilePath(string path, string fileName,
            string extension)
        {
            string directory = (path ?? string.Empty).Trim();
            if (directory.Length == 0)
                throw new ArgumentException("保存目录不能为空。", nameof(path));

            string name = (fileName ?? string.Empty).Trim();
            if (name.Length == 0)
                throw new ArgumentException("文件名不能为空。", nameof(fileName));
            if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                throw new ArgumentException("文件名包含无效字符。", nameof(fileName));

            string normalizedExtension = (extension ?? string.Empty)
                .Trim().TrimStart('.');
            if (normalizedExtension.Length == 0 ||
                normalizedExtension.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                throw new ArgumentException("文件扩展名无效。", nameof(extension));

            return Path.Combine(Path.GetFullPath(directory),
                name + "." + normalizedExtension);
        }

        private static void LogSaveFailure(string kind, string path,
            string imageName, Exception exception)
        {
            AppLog.Error(kind + "保存失败：" +
                (path ?? string.Empty) + Path.DirectorySeparatorChar +
                (imageName ?? string.Empty) + Environment.NewLine + exception,
                nameof(ImageTools));
        }
        
        public static bool TrySaveRecordWithGraphics(ICogRecord imageRecord,
            string savePath, ImageFileType imageType, out string errorMessage)
        {
            try
            {
                if (imageRecord == null)
                    throw new ArgumentNullException(nameof(imageRecord));
                if (!(imageRecord.Content is ICogImage))
                    throw new InvalidOperationException(
                        "视觉记录的 Content 不是视觉图片。");

                string fullPath = Path.GetFullPath(
                    (savePath ?? string.Empty).Trim());
                string directory = Path.GetDirectoryName(fullPath);
                if (string.IsNullOrWhiteSpace(directory))
                    throw new ArgumentException("保存路径必须包含目录。",
                        nameof(savePath));
                Directory.CreateDirectory(directory);

                ExecuteOnStaThread(() =>
                {
                    using (var display = new CogRecordDisplay())
                    {
                        display.CreateControl();
                        display.Record = imageRecord;
                        using (Image renderedImage = display.CreateContentBitmap(
                                   CogDisplayContentBitmapConstants.Image))
                        {
                            if (renderedImage == null)
                                throw new InvalidOperationException(
                                    "VisionPro 未能生成带图形的图片。");
                            renderedImage.Save(fullPath,
                                GetImageFormat(imageType));
                        }
                        display.Record = null;
                    }
                });

                errorMessage = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.GetBaseException().Message;
                LogSaveFailure("带视觉图形的图片", string.Empty, savePath, ex);
                return false;
            }
        }

        public static void SaveRecordWithGraphics(ICogRecord lastRunRecord,
            string imageRecordKey, string savePath, ImageFileType imageType)
        {
            try
            {
                if (lastRunRecord == null)
                    throw new ArgumentNullException(nameof(lastRunRecord));
                if (lastRunRecord.SubRecords == null)
                    throw new InvalidOperationException("视觉运行记录没有子记录。");
                if (string.IsNullOrWhiteSpace(imageRecordKey))
                    throw new ArgumentException("图像记录 Key 不能为空。",
                        nameof(imageRecordKey));

                ICogRecord imageRecord =
                    lastRunRecord.SubRecords[imageRecordKey.Trim()];
                string errorMessage;
                if (!TrySaveRecordWithGraphics(imageRecord, savePath,
                        imageType, out errorMessage))
                    throw new InvalidOperationException(errorMessage);
            }
            catch (Exception ex)
            {
                LogSaveFailure("带视觉图形的图片", string.Empty, savePath, ex);
            }
        }

        private static ImageFormat GetImageFormat(ImageFileType imageType)
        {
            switch (imageType)
            {
                case ImageFileType.Bmp: return ImageFormat.Bmp;
                case ImageFileType.Jpg: return ImageFormat.Jpeg;
                case ImageFileType.Png: return ImageFormat.Png;
                case ImageFileType.Tiff: return ImageFormat.Tiff;
                default:
                    throw new ArgumentOutOfRangeException(nameof(imageType),
                        imageType, "不支持的图片格式。");
            }
        }

        private static void ExecuteOnStaThread(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                action();
                return;
            }

            Exception executionException = null;
            var thread = new Thread(() =>
            {
                try { action(); }
                catch (Exception ex) { executionException = ex; }
            });
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            if (executionException != null)
                throw new InvalidOperationException(
                    "保存带视觉图形的图片失败。", executionException);
        }
    }
}
