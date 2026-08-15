using ST.Library.UI.NodeEditor;
using FrmServices.Services.EditorServices;

namespace FrmViews.Nodes
{
    public abstract class WorkflowNode : STNode, IEditorLoggableNode
    {
        private string _nodeName = string.Empty;
        private string _nodeTypeTitle = string.Empty;

        [STNodeProperty("节点名称", "可选名称；留空时不在画布上显示。")]
        public string NodeName
        {
            get => _nodeName;
            set
            {
                _nodeName = value == null ? string.Empty : value.Trim();
                Title = string.IsNullOrEmpty(_nodeName) ? _nodeTypeTitle : _nodeName;
            }
        }

        [STNodeProperty("生成执行日志", "是否在每次流程执行时记录该节点的日志。")]
        public bool EnableExecutionLog { get; set; } = false;

        protected void SetNodeTypeTitle(string title)
        {
            _nodeTypeTitle = title ?? string.Empty;
            Title = string.IsNullOrEmpty(_nodeName) ? _nodeTypeTitle : _nodeName;
        }
    }
}
