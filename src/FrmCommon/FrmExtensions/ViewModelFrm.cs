using System;
using System.Windows.Forms;
using FrmCommon.Mvvm;

namespace FrmCommon.FrmExtensions
{
    public class ViewModelFrm : Form
    {
        public virtual void FrmBinding()
        {
            if (this is IViewModelFrm vmFrm && vmFrm.DataContext is ReactiveObject vm)
            {
                vm.BindingControl = this;
            }
        }

        public ViewModelFrm()
        {
            this.OnceLoaded((a, b) => FrmBinding());
        }

        protected ViewModelFrm(bool isHide, bool closeApplication = false) : this()
        {
            if (closeApplication)
            {
                this.FormClosed += IFrmViewModel_ApplicationClosd;
            }
            else
            {
                if (isHide)
                {
                    this.FormClosing += IFrmViewModel_FormClosing;
                }
            }
        }

        private void IFrmViewModel_ApplicationClosd(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void IFrmViewModel_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}