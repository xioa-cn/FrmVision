using System.Windows.Forms;
using FrmViews.Views;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public sealed class PlcComparisonOutputsPropertyDescriptor : STNodePropertyDescriptor
    {
        protected override void OnMouseClick(MouseEventArgs e)
        {
            var node = Node as PlcNode;
            if (node == null) return;

            using (var editor = new PlcComparisonOutputEditorFrm(
                       node.ReadValueType, node.ComparisonOutputs))
            {
                if (editor.ShowDialog(Control.FindForm()) != DialogResult.OK) return;
                SetValue(editor.ComparisonOutputs);
            }
        }
    }
}
