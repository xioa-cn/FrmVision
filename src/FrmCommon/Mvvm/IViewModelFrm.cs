using System.ComponentModel;

namespace FrmCommon.Mvvm
{
    public interface IViewModelFrm
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        object DataContext { get; set; }
    }

    public interface IViewModelFrm<T> : IViewModelFrm
    {
      

        void FrmBinding();
    }
}