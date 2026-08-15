using System;
using System.Windows.Forms;

namespace FrmCommon.FrmExtensions
{
    public static class FrmUtils
    {
        public static void TryCloseFrm(this Control control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control), "控件不能为null");

            if (control is Form form)
            {
                form.Close();
                return;
            }

            // 普通控件，递归查找父窗体（直到找到Form或null）
            var parentForm = control.FindForm();

            // 边界处理：控件未挂载到任何窗体（比如动态创建未显示的控件）
            if (parentForm == null)
            {
                throw new InvalidOperationException($"控件 {control.Name} 未挂载到任何窗体，无法关闭");
            }

            // 关闭找到的父窗体（兼容MDI子窗体/普通窗体）
            if (!parentForm.IsDisposed && parentForm.Visible)
            {
                parentForm.Close();
            }
        }

        public static void OnceLoaded(this Form form, System.EventHandler action)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form), "窗体对象不能为null");
            if (action == null)
                throw new ArgumentNullException(nameof(action), "事件处理逻辑不能为null");

            // 定义临时的事件处理委托
            EventHandler tempHandler = null; // 先初始化为null，否则lambda内无法引用自身

            // 给临时委托赋值：执行目标逻辑 + 移除自身绑定
            tempHandler = (sender, args) =>
            {
                try
                {
                    // 执行你传入的核心业务逻辑
                    action.Invoke(sender, args);
                }
                catch (Exception ex)
                {
                    // 可以选择在这里处理异常，或者让它继续抛出
                    throw new InvalidOperationException("OnceLoaded事件处理过程中发生错误", ex);
                }
                finally
                {
                    // 无论核心逻辑是否抛出异常，都移除事件绑定，确保只执行一次
                    form.Load -= tempHandler;
                    // 释放委托引用，辅助GC回收
                    tempHandler = null;
                }
            };

            // 将临时委托绑定到窗体的Load事件
            form.Load += tempHandler;
        }
    }
}