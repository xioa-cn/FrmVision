using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FrmCommon;

namespace FrmServices.Services
{
    public sealed class ParameterCatalogService
    {
        public Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetCatalogAsync()
        {
            return Task.Run<IReadOnlyDictionary<string, IReadOnlyList<string>>>(GetCatalog);
        }

        private static IReadOnlyDictionary<string, IReadOnlyList<string>> GetCatalog()
        {
            var config = GlobalConfig.Instance;
            if (config == null || string.IsNullOrWhiteSpace(config.ConfigCommonDir))
                throw new InvalidOperationException("未配置 ConfigCommonDir。");

            var catalogRoot = Path.GetFullPath(Path.Combine(config.ConfigCommonDir, "产品型录"));
            if (!Directory.Exists(catalogRoot))
                throw new DirectoryNotFoundException("产品型录不存在：" + catalogRoot);

            var catalog = new Dictionary<string, IReadOnlyList<string>>(
                StringComparer.OrdinalIgnoreCase);
            foreach (var productDirectory in Directory
                         .EnumerateDirectories(catalogRoot, "*", SearchOption.TopDirectoryOnly)
                         .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase))
            {
                var productName = Path.GetFileName(productDirectory);
                if (string.IsNullOrWhiteSpace(productName)) continue;

                catalog[productName] = Directory
                    .EnumerateDirectories(productDirectory, "*", SearchOption.TopDirectoryOnly)
                    .Select(Path.GetFileName)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }

            return catalog;
        }
    }
}
