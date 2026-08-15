using FrmCommon.ConfigUtils;
using FrmMapper.Config;

using System;
using System.IO;

namespace FrmCommon
{
    public static class GlobalConfig
    {
        private static readonly object SyncRoot = new object();
        private static readonly string ConfigDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "FrmVision");

        public static VisionConfig Instance { get; private set; } =
            new VisionConfig { ConfigCommonDir = string.Empty };

        public static string ConfigFilePath =>
            Path.Combine(ConfigDirectory, "vision-config.json");

        public static string LoadError { get; private set; } = string.Empty;

        public static void Initialize()
        {
            lock (SyncRoot)
            {
                try
                {
                    VisionConfig config = ConfigFilePath.ReadJson<VisionConfig>();
                    Instance = Normalize(config);
                    LoadError = string.Empty;
                }
                catch (Exception ex)
                {
                    Instance = new VisionConfig { ConfigCommonDir = string.Empty };
                    LoadError = ex.GetBaseException().Message;
                }
            }
        }

        public static void Save(string configCommonDir)
        {
            string normalizedPath = NormalizeDirectory(configCommonDir);
            if (!Directory.Exists(normalizedPath))
                throw new DirectoryNotFoundException("视觉文件根目录不存在：" + normalizedPath);

            var config = new VisionConfig
            {
                ConfigCommonDir = normalizedPath
            };

            lock (SyncRoot)
            {
                config.WriteJson(ConfigFilePath);
                Instance = config;
                LoadError = string.Empty;
            }
        }

        private static VisionConfig Normalize(VisionConfig config)
        {
            if (config == null || string.IsNullOrWhiteSpace(config.ConfigCommonDir))
                return new VisionConfig { ConfigCommonDir = string.Empty };

            return new VisionConfig
            {
                ConfigCommonDir = NormalizeDirectory(config.ConfigCommonDir)
            };
        }

        private static string NormalizeDirectory(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
                throw new ArgumentException("视觉文件根目录不能为空。", nameof(directory));

            return Path.GetFullPath(
                Environment.ExpandEnvironmentVariables(directory.Trim()));
        }
    }
}
