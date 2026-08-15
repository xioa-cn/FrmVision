using System;
using System.Windows.Forms;
using FrmCommon.Mvvm;

namespace FrmCommon.FrmExtensions
{
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