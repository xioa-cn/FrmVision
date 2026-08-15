using FrmCommon;

namespace FrmServices.Utils
{
    public static class ConfigDirUtils
    {
        /// <summary>
        /// 视觉工具路径获取
        /// </summary>
        /// <param name="productionName"></param>
        /// <param name="parameterName"></param>
        /// <param name="blockToolName"></param>
        /// <returns></returns>
        public static string GetBlockToolUtilsDir(string productionName, string parameterName, string blockToolName)
        {
            return GetUtilsDir(productionName, parameterName, blockToolName, "视觉工具");
        }

        /// <summary>
        /// 相机工具路径获取
        /// </summary>
        /// <param name="productionName"></param>
        /// <param name="parameterName"></param>
        /// <param name="blockToolName"></param>
        /// <returns></returns>
        public static string GetCogAcqFifoUtilsDir(string productionName, string parameterName, string blockToolName)
        {
            return GetUtilsDir(productionName, parameterName, blockToolName, "相机工具");
        }


        private static string GetUtilsDir(string productionName, string parameterName, string blockToolName, string dirKey)
        {
            return System.IO.Path.Combine(GlobalConfig.Instance.ConfigCommonDir,
                "产品型录",
                productionName,
                parameterName,
                dirKey,
                blockToolName
                );
        }
    }
}
