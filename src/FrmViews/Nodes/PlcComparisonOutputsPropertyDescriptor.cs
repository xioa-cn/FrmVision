using System.Windows.Forms;
using FrmViews.Views;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    // 作者：xioa
    // 作者邮箱：1327916255@qq.com
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
