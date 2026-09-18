using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FrmCommon.Mvvm
{
    // 作者：xioa
    // 作者邮箱：1327916255@qq.com
    public abstract class ReactiveObject : ObservableObject
    {
        public Control BindingControl { get; set; }
    }
}