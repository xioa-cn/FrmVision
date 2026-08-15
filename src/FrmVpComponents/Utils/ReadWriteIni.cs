using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using FrmServices.LogServices;

namespace FrmVpComponents.Utils
{
    public static class ReadWriteIni
    {
        private const int InitialBufferSize = 256;
        private const int MaximumBufferSize = 1024 * 1024;
        private static readonly object FileLock = new object();
        private static readonly Encoding UnicodeEncoding = new UnicodeEncoding(false, true);
        private static readonly UTF8Encoding StrictUtf8Encoding = new UTF8Encoding(false, true);
        private static readonly string sPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Vision.ini");

        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileStringW",
            CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        private static extern uint GetPrivateProfileString(
            string section,
            string key,
            string defaultValue,
            StringBuilder result,
            uint size,
            string filePath);

        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileStringW",
            CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool WritePrivateProfileString(
            string section,
            string key,
            string value,
            string filePath);

        /// <summary>
        /// 向指定 INI 文件写入节点键值。
        /// </summary>
        public static bool Write(string Section, string Key, string Value, string FileName)
        {
            return WriteCore(Section, Key, Value, FileName);
        }

        /// <summary>
        /// 向程序目录下的 Vision.ini 写入节点键值。
        /// </summary>
        public static bool Write(string Section, string Key, string Value)
        {
            return WriteCore(Section, Key, Value, sPath);
        }

        /// <summary>
        /// 从程序目录下的 Vision.ini 读取节点键值。
        /// </summary>
        public static string ReadValue(string Section, string Key)
        {
            return ReadCore(Section, Key, sPath);
        }

        /// <summary>
        /// 从指定 INI 文件读取节点键值。
        /// </summary>
        public static string ReadValue(string Section, string Key, string FileName)
        {
            return ReadCore(Section, Key, FileName);
        }

        private static bool WriteCore(string section, string key, string value, string fileName)
        {
            if (section == null || key == null || value == null || string.IsNullOrWhiteSpace(fileName))
                return false;

            try
            {
                var fullPath = NormalizePath(fileName);
                lock (FileLock)
                {
                    EnsureUnicodeFile(fullPath);
                    if (WritePrivateProfileString(section, key, value, fullPath)) return true;
                    throw CreateWin32Exception("写入 INI 文件失败", fullPath);
                }
            }
            catch (Exception ex)
            {
                LogError("INI 写入失败", fileName, ex);
                return false;
            }
        }

        private static string ReadCore(string section, string key, string fileName)
        {
            if (section == null || key == null || string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            try
            {
                var fullPath = NormalizePath(fileName);
                lock (FileLock)
                {
                    if (!File.Exists(fullPath)) return string.Empty;
                    EnsureUnicodeFile(fullPath);
                    return ReadProfileValue(section, key, fullPath);
                }
            }
            catch (Exception ex)
            {
                LogError("INI 读取失败", fileName, ex);
                return string.Empty;
            }
        }

        private static string ReadProfileValue(string section, string key, string filePath)
        {
            var bufferSize = InitialBufferSize;
            while (true)
            {
                var buffer = new StringBuilder(bufferSize);
                var length = GetPrivateProfileString(
                    section, key, string.Empty, buffer, (uint)buffer.Capacity, filePath);

                if (length < buffer.Capacity - 1 || bufferSize >= MaximumBufferSize)
                    return buffer.ToString(0, (int)length);

                bufferSize = Math.Min(bufferSize * 2, MaximumBufferSize);
            }
        }

        private static string NormalizePath(string fileName)
        {
            var expandedPath = Environment.ExpandEnvironmentVariables(fileName.Trim());
            return Path.GetFullPath(expandedPath);
        }

        private static void EnsureUnicodeFile(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, string.Empty, UnicodeEncoding);
                return;
            }

            var bytes = File.ReadAllBytes(filePath);
            if (HasUtf16LittleEndianBom(bytes)) return;

            var content = DecodeExistingFile(bytes);
            File.WriteAllText(filePath, content, UnicodeEncoding);
        }

        private static string DecodeExistingFile(byte[] bytes)
        {
            if (bytes.Length == 0) return string.Empty;

            if (HasUtf8Bom(bytes))
                return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);

            if (HasUtf16BigEndianBom(bytes))
                return Encoding.BigEndianUnicode.GetString(bytes, 2, bytes.Length - 2);

            try
            {
                return StrictUtf8Encoding.GetString(bytes);
            }
            catch (DecoderFallbackException)
            {
                return Encoding.Default.GetString(bytes);
            }
        }

        private static bool HasUtf8Bom(byte[] bytes)
        {
            return bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
        }

        private static bool HasUtf16LittleEndianBom(byte[] bytes)
        {
            return bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE;
        }

        private static bool HasUtf16BigEndianBom(byte[] bytes)
        {
            return bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF;
        }

        private static Exception CreateWin32Exception(string message, string filePath)
        {
            var errorCode = Marshal.GetLastWin32Error();
            if (errorCode == 0) return new IOException($"{message}：{filePath}");
            return new Win32Exception(errorCode, $"{message}：{filePath}");
        }

        private static void LogError(string operation, string fileName, Exception exception)
        {
            AppLog.Error(
                $"{operation}：{fileName ?? string.Empty}{Environment.NewLine}{exception}",
                nameof(ReadWriteIni));
        }
    }
}
