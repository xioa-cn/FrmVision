using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using FrmCommon.FrmExtensions;
using FrmCommon.Mvvm;
using FrmServices.Communication;
using FrmServices.Services.EditorServices;
using FrmServices.ViewModel;
using FrmViews.Nodes;
using FrmViews.Services;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Views
{
    /// <summary>
    /// 正常运行、流程编辑器关闭：节点之间延时 0 ms，上一个节点完成后立即执行下一个。
    /// 流程编辑器打开：除开始节点外，每个节点执行前延时 1000 ms，方便观察节点状态。
    /// 一轮流程成功后：正常等待 50 ms 再开始下一轮；编辑器打开时等待 1000 ms。
    ///一轮流程失败后：等待 5000 ms 再重试。
    ///节点自身执行时间，例如相机采集、视觉处理、PLC 通讯，不算节点衔接延时。
    /// </summary>
    public partial class EditorFrm : ViewModelFrm, IViewModelFrm<EditorViewModel>
    {
        private bool _isBound;
        private bool _runtimeEventsBound;
        private readonly WorkflowRuntime _workflowRuntime;

        private readonly Func<string, LightSourceFrmVpCommunication>
            _lightSourceResolver;

        private readonly object _progressRoot = new object();

        private readonly Dictionary<Guid, EditorNodeExecutionEventArgs>
            _pendingProgress =
                new Dictionary<Guid, EditorNodeExecutionEventArgs>();

        private readonly Dictionary<Guid, STNode> _flowHighlightedNodes =
            new Dictionary<Guid, STNode>();

        private int _progressUpdateScheduled;
        private bool _isClosing;

        public event EventHandler WorkflowSaved;

        public EditorFrm() : this(null, null)
        {
        }

        public EditorFrm(WorkflowRuntime workflowRuntime,
            Func<string, LightSourceFrmVpCommunication> lightSourceResolver)
        {
            _workflowRuntime = workflowRuntime;
            _lightSourceResolver = lightSourceResolver;
            InitializeComponent();
            InitializeNodeEditor();
        }

        public object DataContext { get; set; }

        private EditorViewModel ViewModel => (EditorViewModel)DataContext;

        private void InitializeNodeEditor()
        {
            nodeEditorPannel.Editor.LoadAssembly(typeof(StartNode).Assembly.Location);
            nodeEditorPannel.Editor.NodeAdded += NodeEditorOnNodeAdded;

            // 流程控制
            nodeEditorPannel.AddSTNode(typeof(StartNode));
            nodeEditorPannel.AddSTNode(typeof(RecipeNode));
            nodeEditorPannel.AddSTNode(typeof(ManualTriggerNode));
            nodeEditorPannel.AddSTNode(typeof(WaitingRhythmNode));

            // 数据源
            nodeEditorPannel.AddSTNode(typeof(StringNode));
            nodeEditorPannel.AddSTNode(typeof(NumNode));
            nodeEditorPannel.AddSTNode(typeof(GlobalDataNode));

            // 数据处理
            nodeEditorPannel.AddSTNode(typeof(CharacterMergingNode));
            nodeEditorPannel.AddSTNode(typeof(TrimNode));
            nodeEditorPannel.AddSTNode(typeof(ReplaceNode));
            nodeEditorPannel.AddSTNode(typeof(CompensationNode));
            nodeEditorPannel.AddSTNode(typeof(CalcNumNode));

            // 逻辑判断
            nodeEditorPannel.AddSTNode(typeof(ComparisonNode));
            nodeEditorPannel.AddSTNode(typeof(BoolNode));
            nodeEditorPannel.AddSTNode(typeof(NegateNode));

            // 设备通讯
            nodeEditorPannel.AddSTNode(typeof(PlcNode));
            nodeEditorPannel.AddSTNode(typeof(PlcValueTransmitNode));
            nodeEditorPannel.AddSTNode(typeof(PlcWriteTransmitNode));
            nodeEditorPannel.AddSTNode(typeof(LightSourceNode));

            // 视觉图像
            nodeEditorPannel.AddSTNode(typeof(CameraNode));
            nodeEditorPannel.AddSTNode(typeof(VisionNode));
            nodeEditorPannel.AddSTNode(typeof(PictureNode));
            nodeEditorPannel.AddSTNode(typeof(SaveImageNode));
        }

        private void NodeEditorOnNodeAdded(object sender, STNodeEditorEventArgs e)
        {
            var lightSourceNode = e.Node as LightSourceNode;
            if (lightSourceNode != null)
                lightSourceNode.LightSourceResolver = _lightSourceResolver;
        }

        public override void FrmBinding()
        {
            if (_isBound) return;
            _isBound = true;
            if (DataContext == null) DataContext = new EditorViewModel();

            LoadEditorData();
            BindRuntimeEvents();
            base.FrmBinding();
        }

        private void BindRuntimeEvents()
        {
            if (_workflowRuntime == null || _runtimeEventsBound) return;
            _runtimeEventsBound = true;
            _workflowRuntime.NodeExecutionChanged += WorkflowRuntimeOnNodeExecutionChanged;
            _workflowRuntime.ExecutionStateChanged += WorkflowRuntimeOnExecutionStateChanged;
            UpdateEditingState();

            foreach (EditorNodeExecutionEventArgs progress in
                     _workflowRuntime.CurrentNodeProgresses)
                ApplyNodeProgress(progress);
        }

        private void LoadEditorData()
        {
            try
            {
                byte[] canvasData = ViewModel.LoadEditorData();
                if (canvasData == null)
                {
                    AddDefaultStartNode();
                    SetStatus("新流程", Color.FromArgb(100, 112, 128));
                    return;
                }

                nodeEditorPannel.Editor.LoadCanvas(canvasData);
                SetStatus("已加载", Color.FromArgb(21, 146, 78));
            }
            catch (Exception ex)
            {
                if (nodeEditorPannel.Editor.Nodes.Count == 0)
                    AddDefaultStartNode();
                SetStatus("加载失败", Color.FromArgb(220, 38, 38));
                MessageBox.Show(this,
                    "流程文件加载失败：" + ex.GetBaseException().Message +
                    Environment.NewLine + ViewModel.EditorDataFilePath,
                    "加载流程", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddDefaultStartNode()
        {
            if (nodeEditorPannel.Editor.Nodes.Count > 0) return;

            var startNode = new StartNode
            {
                Left = 80,
                Top = 80
            };
            nodeEditorPannel.Editor.Nodes.Add(startNode);
        }

        private void SaveButtonOnClick(object sender, EventArgs e)
        {
            SaveEditorData();
        }

        private void SaveEditorData()
        {
            try
            {
                ViewModel.SaveEditorData(nodeEditorPannel.Editor.GetCanvasData());
                SetStatus("已保存", Color.FromArgb(21, 146, 78));
                WorkflowSaved?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                SetStatus("保存失败", Color.FromArgb(220, 38, 38));
                MessageBox.Show(this,
                    "流程文件保存失败：" + ex.GetBaseException().Message +
                    Environment.NewLine + ViewModel.EditorDataFilePath,
                    "保存流程", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetStatus(string text, Color color)
        {
            statusLabel.Text = text;
            statusLabel.ForeColor = color;
        }

        private void WorkflowRuntimeOnNodeExecutionChanged(object sender,
            EditorNodeExecutionEventArgs e)
        {
            if (_isClosing) return;
            lock (_progressRoot)
                _pendingProgress[e.NodeGuid] = e;
            ScheduleProgressUpdate();
        }

        private void ScheduleProgressUpdate()
        {
            if (_isClosing || IsDisposed || Disposing || !IsHandleCreated) return;
            if (Interlocked.CompareExchange(ref _progressUpdateScheduled, 1, 0) != 0)
                return;

            try
            {
                BeginInvoke(new MethodInvoker(ApplyPendingProgress));
            }
            catch (ObjectDisposedException)
            {
                Interlocked.Exchange(ref _progressUpdateScheduled, 0);
            }
            catch (InvalidOperationException)
            {
                Interlocked.Exchange(ref _progressUpdateScheduled, 0);
            }
        }

        private void ApplyPendingProgress()
        {
            EditorNodeExecutionEventArgs[] progressItems;
            lock (_progressRoot)
            {
                progressItems = _pendingProgress.Values.ToArray();
                _pendingProgress.Clear();
            }

            if (!_isClosing && !IsDisposed && !Disposing)
            {
                foreach (EditorNodeExecutionEventArgs progress in progressItems)
                    ApplyNodeProgress(progress);
            }

            Interlocked.Exchange(ref _progressUpdateScheduled, 0);
            lock (_progressRoot)
            {
                if (_pendingProgress.Count == 0) return;
            }

            ScheduleProgressUpdate();
        }

        private void WorkflowRuntimeOnExecutionStateChanged(object sender, EventArgs e)
        {
            RunOnUiThread(UpdateEditingState);
        }

        private void ApplyNodeProgress(EditorNodeExecutionEventArgs progress)
        {
            STNode node = nodeEditorPannel.Editor.Nodes.ToArray()
                .FirstOrDefault(item => item.Guid == progress.NodeGuid);
            if (node != null)
            {
                STNode previousNode;
                if (_flowHighlightedNodes.TryGetValue(
                        progress.FlowStartNodeGuid, out previousNode) &&
                    !ReferenceEquals(previousNode, node))
                {
                    previousNode.RuntimeHighlightColor = Color.Empty;
                }

                _flowHighlightedNodes[progress.FlowStartNodeGuid] = node;
                node.RuntimeHighlightColor = progress.IsCompleted
                    ? progress.IsSuccess
                        ? Color.FromArgb(34, 197, 94)
                        : Color.FromArgb(239, 68, 68)
                    : Color.FromArgb(250, 204, 21);
                node.RuntimeTextColor = progress.IsCompleted
                    ? progress.IsSuccess
                        ? Color.FromArgb(34, 197, 94)
                        : Color.FromArgb(239, 68, 68)
                    : Color.FromArgb(250, 204, 21);
                node.RuntimeText = progress.IsCompleted
                    ? progress.RuntimeValueText
                    : "运行中...";
            }

            string name = string.IsNullOrWhiteSpace(progress.NodeTitle)
                ? "未命名节点"
                : progress.NodeTitle;
            if (!progress.IsCompleted)
            {
                SetStatus("运行中：" + name, Color.FromArgb(36, 99, 235));
                return;
            }

            SetStatus((progress.IsSuccess ? "已完成：" : "失败：") + name,
                progress.IsSuccess
                    ? Color.FromArgb(21, 146, 78)
                    : Color.FromArgb(220, 38, 38));
        }

        private void ClearNodeHighlight()
        {
            foreach (STNode node in _flowHighlightedNodes.Values.Distinct())
                node.RuntimeHighlightColor = Color.Empty;
            _flowHighlightedNodes.Clear();
        }

        private void UpdateEditingState()
        {
            bool isExecuting = _workflowRuntime != null && _workflowRuntime.IsExecuting;
            if (isExecuting && _workflowRuntime.CurrentNodeProgress == null)
                SetStatus("流程运行中", Color.FromArgb(36, 99, 235));
        }

        private void RunOnUiThread(Action action)
        {
            if (action == null || IsDisposed || Disposing) return;
            if (InvokeRequired)
            {
                if (!IsHandleCreated) return;
                try
                {
                    BeginInvoke(action);
                }
                catch (ObjectDisposedException)
                {
                }
                catch (InvalidOperationException)
                {
                }

                return;
            }

            action();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _isClosing = true;
            nodeEditorPannel.Editor.NodeAdded -= NodeEditorOnNodeAdded;
            if (_workflowRuntime != null && _runtimeEventsBound)
            {
                _workflowRuntime.NodeExecutionChanged -= WorkflowRuntimeOnNodeExecutionChanged;
                _workflowRuntime.ExecutionStateChanged -= WorkflowRuntimeOnExecutionStateChanged;
                _runtimeEventsBound = false;
            }

            lock (_progressRoot)
                _pendingProgress.Clear();
            ClearNodeHighlight();
            base.OnFormClosing(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                SaveEditorData();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}