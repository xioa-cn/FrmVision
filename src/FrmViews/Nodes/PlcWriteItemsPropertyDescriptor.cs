using System;
using System.Text;
using System.Windows.Forms;
using FrmViews.Views;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public sealed class PlcWriteItemsPropertyDescriptor : STNodePropertyDescriptor
    {
        protected override string GetStringFromValue()
        {
            var node = Node as PlcNode;
            if (node == null) return "未配置";

            try
            {
                var items = PlcWriteItemSerializer.Parse(node.WriteItems);
                int fixedCount = items.FindAll(item =>
                    item.ValueSource == PlcWriteValueSource.FixedValue).Count;
                return items.Count == 0
                    ? "未配置"
                    : string.Format("{0} 项（{1} 端口 / {2} 固定值）",
                        items.Count, items.Count, fixedCount);
            }
            catch
            {
                return "配置错误";
            }
        }

        protected override byte[] GetBytesFromValue()
        {
            var node = Node as PlcNode;
            return Encoding.UTF8.GetBytes(node == null
                ? string.Empty
                : node.WriteItems ?? string.Empty);
        }

        protected override object GetValueFromBytes(byte[] byData)
        {
            return byData == null ? string.Empty : Encoding.UTF8.GetString(byData);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            var node = Node as PlcNode;
            if (node == null) return;

            using (var editor = new PlcWriteItemEditorFrm(node.WriteItems))
            {
                if (editor.ShowDialog(Control.FindForm()) != DialogResult.OK) return;
                SetValue(editor.WriteItems);
            }
        }
    }
}
