using System;
using System.Windows.Forms;

namespace ApplicationStartup
{
    // 作者：xioa
    // 作者邮箱：1327916255@qq.com
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SplashFrm());
        }
    }
}
