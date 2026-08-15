using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrmServices.Services.EditorServices;
using FrmServices.ViewModel;
using FrmViews.Nodes;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Services
{
    public sealed class WorkflowRuntime : IDisposable
    {
        private readonly object _stateRoot = new object();
        private readonly STNodeEditor _editor;
        private readonly EditorTask _task;
        private readonly EditorViewModel _viewModel;
        private bool _isExecuting;
        private bool _isLoaded;
        private bool _disposed;
        private readonly Dictionary<Guid, EditorNodeExecutionEventArgs>
            _currentNodeProgresses =
                new Dictionary<Guid, EditorNodeExecutionEventArgs>();

        public WorkflowRuntime(EditorViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            _editor = new STNodeEditor();
            _editor.LoadAssembly(typeof(StartNode).Assembly.Location);
            _task = new EditorTask();
            _task.NodeExecutionChanged += TaskOnNodeExecutionChanged;
        }

        public event EventHandler<EditorNodeExecutionEventArgs> NodeExecutionChanged;
        public event EventHandler ExecutionStateChanged;

        public bool IsExecuting
        {
            get
            {
                lock (_stateRoot)
                    return _isExecuting;
            }
        }

        public bool IsLoaded
        {
            get
            {
                lock (_stateRoot)
                    return _isLoaded;
            }
        }

        public EditorNodeExecutionEventArgs CurrentNodeProgress
        {
            get
            {
                lock (_stateRoot)
                    return _currentNodeProgresses.Values.LastOrDefault();
            }
        }

        public EditorNodeExecutionEventArgs[] CurrentNodeProgresses
        {
            get
            {
                lock (_stateRoot)
                    return _currentNodeProgresses.Values.ToArray();
            }
        }

        public bool Load()
        {
            lock (_stateRoot)
            {
                ThrowIfDisposed();
                if (_isExecuting)
                    throw new InvalidOperationException("流程正在执行，不能重新加载画布。");

                byte[] canvasData = _viewModel.LoadEditorData();
                _editor.Nodes.Clear();
                _currentNodeProgresses.Clear();
                _isLoaded = false;
                if (canvasData == null) return false;

                _editor.LoadCanvas(canvasData);
                _isLoaded = true;
                return true;
            }
        }

        public Task<EditorExecutionResult> ExecuteAsync(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            lock (_stateRoot)
            {
                ThrowIfDisposed();
                if (!_isLoaded)
                    throw new InvalidOperationException("流程尚未加载。");
                if (_isExecuting)
                    throw new InvalidOperationException("流程正在执行，不能重复启动。");
                _isExecuting = true;
                _currentNodeProgresses.Clear();
            }

            OnExecutionStateChanged();
            return ExecuteCoreAsync(context);
        }

        private async Task<EditorExecutionResult> ExecuteCoreAsync(
            EditorExecutionContext context)
        {
            try
            {
                return await Task.Run(() => _task.Execute(_editor, context));
            }
            finally
            {
                lock (_stateRoot)
                    _isExecuting = false;
                OnExecutionStateChanged();
            }
        }

        private void TaskOnNodeExecutionChanged(object sender,
            EditorNodeExecutionEventArgs e)
        {
            lock (_stateRoot)
                _currentNodeProgresses[e.FlowStartNodeGuid] = e;

            EventHandler<EditorNodeExecutionEventArgs> handler = NodeExecutionChanged;
            if (handler == null) return;
            foreach (EventHandler<EditorNodeExecutionEventArgs> subscriber in
                     handler.GetInvocationList())
            {
                try { subscriber(this, e); }
                catch { }
            }
        }

        private void OnExecutionStateChanged()
        {
            EventHandler handler = ExecutionStateChanged;
            if (handler == null) return;
            foreach (EventHandler subscriber in handler.GetInvocationList())
            {
                try { subscriber(this, EventArgs.Empty); }
                catch { }
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(WorkflowRuntime));
        }

        public void Dispose()
        {
            lock (_stateRoot)
            {
                if (_disposed) return;
                if (_isExecuting)
                    throw new InvalidOperationException("流程仍在执行，不能释放运行时。");
                _disposed = true;
            }

            _task.NodeExecutionChanged -= TaskOnNodeExecutionChanged;
            _editor.Dispose();
        }
    }
}
