using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace FrmCommon.ConfigUtils
{
    public static class JsonExtensions
    {
        public static void WriteJson<T>(this T value, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("JSON 文件路径不能为空。", nameof(filePath));

            var fullPath = Path.GetFullPath(filePath);
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write,
                       FileShare.None))
            {
                serializer.WriteObject(stream, value);
            }
        }

        public static T ReadJson<T>(this string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("JSON 文件路径不能为空。", nameof(filePath));

            var fullPath = Path.GetFullPath(filePath);
            if (!File.Exists(fullPath)) return default(T);

            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read,
                       FileShare.Read))
            {
                return (T)serializer.ReadObject(stream);
            }
        }
    }
}
