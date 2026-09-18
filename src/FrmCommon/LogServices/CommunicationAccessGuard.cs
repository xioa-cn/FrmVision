using System;
using System.Threading;
using System.Windows.Forms;

namespace FrmCommon.LogServices
{
    // 作者：xioa
    // 作者邮箱：1327916255@qq.com
    public static class CommunicationAccessGuard
    {
        // 0：尚未校验，1：通过，-1：失败。启动结果在当前进程中不可重新授权。
        private static int _state;

        public const string FailureMessage =
            "\u4F5C\u8005\u4FE1\u606F\u6821\u9A8C\u672A\u901A\u8FC7\uFF0C\u6240\u6709\u901A\u8BAF\u529F\u80FD\u5DF2\u7981\u7528\u3002\u8BF7\u6062\u590D\u4F5C\u8005\u0020\u0078\u0069\u006F\u0061\u0020\u7684\u539F\u59CB\u7F72\u540D\u540E\u91CD\u65B0\u542F\u52A8\u7A0B\u5E8F\u3002";

        public static bool IsAllowed
        {
            get { return Volatile.Read(ref _state) == 1; }
        }

        public static bool Initialize(Control statusBar)
        {
            bool valid = false;
            try
            {
                if (statusBar != null && !statusBar.IsDisposed)
                {
                    var labels = statusBar.Controls.Find("authorLabel", true);
                    valid = labels.Length == 1 && labels[0] is Label
                                               && string.Equals(labels[0].Text,
                                                   "\u0046\u0072\u006D\u0056\u0069\u0073\u0069\u006F\u006E"
                                                   + "\u0020\u00A9\u0020\u0020\u4F5C\u8005\uFF1A\u0078\u0069\u006F\u0061",
                                                   StringComparison.Ordinal);
                }
            }
            catch
            {
                valid = false;
            }

            Interlocked.CompareExchange(ref _state, valid ? 1 : -1, 0);
            return IsAllowed;
        }

        public static void EnsureAllowed()
        {
            if (!IsAllowed) throw new InvalidOperationException(FailureMessage);
        }
    }
}