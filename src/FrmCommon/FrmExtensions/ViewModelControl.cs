using System;
using System.Windows.Forms;
using FrmCommon.Mvvm;

namespace FrmCommon.FrmExtensions
{
    // 作者：xioa
    // 作者邮箱：1327916255@qq.com
    public class ViewModelControl : UserControl
    {
        public virtual void FrmBinding()
        {
            if (this is IViewModelFrm vmFrm && vmFrm.DataContext is ReactiveObject vm)
            {
                vm.BindingControl = this;
            }
        }

        public ViewModelControl()
        {
            Load();
        }

        private void Load()
        {
            this.FrmBinding();
        }
    }
}