using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using FrmServices.Communication;
using ST.Library.UI.NodeEditor;

namespace FrmServices.Services.EditorServices
{
    public interface IEditorExecutableNode
    {
        EditorNodeExecutionResult Execute(EditorExecutionContext context);
    }

    public interface IEditorLoggableNode
    {
        bool EnableExecutionLog { get; }
    }

    public interface IEditorStartNode
    {
    }

    public interface IEditorNodeReadiness
    {
        EditorNodeReadinessResult CanExecute(EditorExecutionContext context);
    }

    public sealed class EditorExecutionContext
    {
        private readonly object _activatedInputsRoot = new object();
        private readonly Dictionary<STNodeOption, Queue<object>>
            _activatedInputs =
                new Dictionary<STNodeOption, Queue<object>>();

        public EditorExecutionContext() : this(CancellationToken.None)
        {
        }

        public EditorExecutionContext(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            ExecutionId = Guid.NewGuid();
            Items = new ConcurrentDictionary<string, object>(
                StringComparer.OrdinalIgnoreCase);
            StopOnFailure = true;
            MaxSteps = 10000;
        }

        public Guid ExecutionId { get; private set; }
        public CancellationToken CancellationToken { get; }
        public Func<string, PlcFrmVpCommunication> PlcResolver { get; set; }
        public Func<string, LightSourceFrmVpCommunication> LightSourceResolver { get; set; }
        public Func<int> NodeTransitionDelayMillisecondsProvider { get; set; }
        public Func<int> SuccessfulCycleDelayMillisecondsProvider { get; set; }
        public Func<int> FailedCycleDelayMillisecondsProvider { get; set; }
        public Action<string, string> RecipeChanged { get; set; }
        public Action<EditorExecutionResult> FlowCycleCompleted { get; set; }
        public IDictionary<string, object> Items { get; }
        public bool StopOnFailure { get; set; }
        public int MaxSteps { get; set; }

        public bool IsInputActivated(STNodeOption input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            lock (_activatedInputsRoot)
            {
                Queue<object> values;
                return _activatedInputs.TryGetValue(input, out values) &&
                       values.Count > 0;
            }
        }

        public PlcFrmVpCommunication ResolvePlc(string key)
        {
            string normalizedKey = RequireKey(key, "PLC");
            if (PlcResolver == null)
                throw new InvalidOperationException("未配置 PLC 通讯解析器。");
            PlcFrmVpCommunication plc = PlcResolver(normalizedKey);
            if (plc == null)
                throw new InvalidOperationException("未找到 PLC 通讯实例：" + normalizedKey + "。");
            return plc;
        }

        public LightSourceFrmVpCommunication ResolveLightSource(string key)
        {
            string normalizedKey = RequireKey(key, "光源");
            if (LightSourceResolver == null)
                throw new InvalidOperationException("未配置光源通讯解析器。");
            LightSourceFrmVpCommunication lightSource = LightSourceResolver(normalizedKey);
            if (lightSource == null)
                throw new InvalidOperationException("未找到光源通讯实例：" + normalizedKey + "。");
            return lightSource;
        }

        internal void BeginExecution()
        {
            ExecutionId = Guid.NewGuid();
            lock (_activatedInputsRoot)
                _activatedInputs.Clear();
        }

        internal void MarkInputActivated(STNodeOption input, object value)
        {
            if (input == null) return;
            lock (_activatedInputsRoot)
            {
                Queue<object> values;
                if (!_activatedInputs.TryGetValue(input, out values))
                {
                    values = new Queue<object>();
                    _activatedInputs.Add(input, values);
                }
                values.Enqueue(value);
            }
        }

        internal void PrepareNodeInputs(STNode node)
        {
            if (node == null) return;
            STNodeOption[] inputs = node.GetInputOptions() ??
                                    new STNodeOption[0];
            lock (_activatedInputsRoot)
            {
                foreach (STNodeOption input in inputs)
                {
                    Queue<object> values;
                    if (input != null &&
                        _activatedInputs.TryGetValue(input, out values) &&
                        values.Count > 0)
                        input.Data = values.Peek();
                }
            }
        }

        internal void ConsumeNodeInputs(STNode node)
        {
            if (node == null) return;
            STNodeOption[] inputs = node.GetInputOptions() ??
                                    new STNodeOption[0];
            lock (_activatedInputsRoot)
            {
                foreach (STNodeOption input in inputs)
                {
                    Queue<object> values;
                    if (input == null ||
                        !_activatedInputs.TryGetValue(input, out values) ||
                        values.Count == 0)
                        continue;
                    values.Dequeue();
                    if (values.Count == 0) _activatedInputs.Remove(input);
                }
            }
        }

        internal bool HasPendingInputs(STNode node)
        {
            if (node == null) return false;
            STNodeOption[] inputs = node.GetInputOptions() ??
                                    new STNodeOption[0];
            lock (_activatedInputsRoot)
            {
                foreach (STNodeOption input in inputs)
                {
                    Queue<object> values;
                    if (input != null &&
                        _activatedInputs.TryGetValue(input, out values) &&
                        values.Count > 0)
                        return true;
                }
                return false;
            }
        }

        internal int GetNodeTransitionDelayMilliseconds()
        {
            int delay = NodeTransitionDelayMillisecondsProvider == null
                ? 0
                : NodeTransitionDelayMillisecondsProvider();
            return Math.Max(0, delay);
        }

        internal int GetSuccessfulCycleDelayMilliseconds()
        {
            int delay = SuccessfulCycleDelayMillisecondsProvider == null
                ? 50
                : SuccessfulCycleDelayMillisecondsProvider();
            return Math.Max(0, delay);
        }

        internal int GetFailedCycleDelayMilliseconds()
        {
            int delay = FailedCycleDelayMillisecondsProvider == null
                ? 5000
                : FailedCycleDelayMillisecondsProvider();
            return Math.Max(0, delay);
        }

        internal EditorExecutionContext CreateCycleContext()
        {
            return new EditorExecutionContext(CancellationToken)
            {
                PlcResolver = PlcResolver,
                LightSourceResolver = LightSourceResolver,
                NodeTransitionDelayMillisecondsProvider =
                    NodeTransitionDelayMillisecondsProvider,
                SuccessfulCycleDelayMillisecondsProvider =
                    SuccessfulCycleDelayMillisecondsProvider,
                FailedCycleDelayMillisecondsProvider =
                    FailedCycleDelayMillisecondsProvider,
                RecipeChanged = RecipeChanged,
                FlowCycleCompleted = FlowCycleCompleted,
                StopOnFailure = StopOnFailure,
                MaxSteps = MaxSteps
            };
        }

        internal void ReportFlowCycle(EditorExecutionResult result)
        {
            if (result != null) FlowCycleCompleted?.Invoke(result);
        }

        public void NotifyRecipeChanged(string productionKey, string recipeName)
        {
            RecipeChanged?.Invoke(productionKey, recipeName);
        }

        private static string RequireKey(string key, string deviceName)
        {
            string normalizedKey = (key ?? string.Empty).Trim();
            if (normalizedKey.Length == 0)
                throw new InvalidOperationException(deviceName + " Key 不能为空。");
            return normalizedKey;
        }
    }

    public sealed class EditorFlowSignal
    {
        public EditorFlowSignal(Guid executionId)
        {
            ExecutionId = executionId;
            CreatedAt = DateTime.Now;
        }

        public Guid ExecutionId { get; }
        public DateTime CreatedAt { get; }
    }

    public sealed class EditorNodeReadinessResult
    {
        private EditorNodeReadinessResult(bool isReady, string message)
        {
            IsReady = isReady;
            Message = message ?? string.Empty;
        }

        public bool IsReady { get; }
        public string Message { get; }

        public static EditorNodeReadinessResult Ready()
        {
            return new EditorNodeReadinessResult(true, string.Empty);
        }

        public static EditorNodeReadinessResult NotReady(string message)
        {
            return new EditorNodeReadinessResult(false, message);
        }
    }

    public sealed class EditorNodeExecutionResult
    {
        private EditorNodeExecutionResult(bool isSuccess, string message, STNodeOption[] activeOutputs)
        {
            IsSuccess = isSuccess;
            Message = message ?? string.Empty;
            ActiveOutputs = activeOutputs ?? new STNodeOption[0];
        }

        public bool IsSuccess { get; }
        public string Message { get; }
        public STNodeOption[] ActiveOutputs { get; }

        public static EditorNodeExecutionResult Success(string message,
            params STNodeOption[] activeOutputs)
        {
            return new EditorNodeExecutionResult(true, message, activeOutputs);
        }

        public static EditorNodeExecutionResult Failure(string message)
        {
            return new EditorNodeExecutionResult(false, message, new STNodeOption[0]);
        }
    }

    public sealed class EditorExecutionStep
    {
        internal EditorExecutionStep(int sequence, STNode node, bool isSuccess,
            string message, TimeSpan elapsed, STNodeOption[] activeOutputs)
        {
            Sequence = sequence;
            NodeGuid = node.Guid;
            NodeTitle = node.Title;
            NodeType = node.GetType().FullName;
            IsSuccess = isSuccess;
            Message = message ?? string.Empty;
            Elapsed = elapsed;
            ActiveOutputs = activeOutputs ?? new STNodeOption[0];
            EnableExecutionLog = !(node is IEditorLoggableNode loggable) ||
                                 loggable.EnableExecutionLog;
        }

        public int Sequence { get; }
        public Guid NodeGuid { get; }
        public string NodeTitle { get; }
        public string NodeType { get; }
        public bool IsSuccess { get; }
        public string Message { get; }
        public TimeSpan Elapsed { get; }
        public STNodeOption[] ActiveOutputs { get; }
        public bool EnableExecutionLog { get; }
    }

    public sealed class EditorNodeExecutionEventArgs : EventArgs
    {
        internal EditorNodeExecutionEventArgs(Guid flowStartNodeGuid, STNode node,
            bool isCompleted, bool isSuccess, string message,
            string runtimeValueText)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            FlowStartNodeGuid = flowStartNodeGuid;
            NodeGuid = node.Guid;
            NodeTitle = node.Title;
            NodeType = node.GetType().FullName;
            IsCompleted = isCompleted;
            IsSuccess = isSuccess;
            Message = message ?? string.Empty;
            RuntimeValueText = runtimeValueText ?? string.Empty;
        }

        public Guid FlowStartNodeGuid { get; }
        public Guid NodeGuid { get; }
        public string NodeTitle { get; }
        public string NodeType { get; }
        public bool IsCompleted { get; }
        public bool IsSuccess { get; }
        public string Message { get; }
        public string RuntimeValueText { get; }
    }

    public sealed class EditorExecutionResult
    {
        internal EditorExecutionResult(Guid executionId, bool isSuccess,
            bool isCanceled, string message, EditorExecutionStep[] steps)
        {
            ExecutionId = executionId;
            IsSuccess = isSuccess;
            IsCanceled = isCanceled;
            Message = message ?? string.Empty;
            Steps = steps ?? new EditorExecutionStep[0];
        }

        public Guid ExecutionId { get; }
        public bool IsSuccess { get; }
        public bool IsCanceled { get; }
        public string Message { get; }
        public EditorExecutionStep[] Steps { get; }
    }
}
