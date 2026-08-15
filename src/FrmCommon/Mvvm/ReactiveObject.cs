using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FrmCommon.Mvvm
{
    public abstract class ReactiveObject : ObservableObject
    {
        public Control BindingControl { get; set; }
    }
}