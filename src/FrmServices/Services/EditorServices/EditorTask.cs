using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ST.Library.UI.NodeEditor;

namespace FrmServices.Services.EditorServices
{
    public sealed class EditorTask
    {
        private int _isExecuting;

        public bool IsExecuting => Volatile.Read(ref _isExecuting) == 1;
        public event EventHandler<EditorNodeExecutionEventArgs> NodeExecutionChanged;

        public EditorExecutionResult Execute(STNodeEditor editor)
        {
            return Execute(editor, new EditorExecutionContext());
        }

        public EditorExecutionResult Execute(STNodeEditor editor,
            EditorExecutionContext context)
        {
            if (editor == null) throw new ArgumentNullException(nameof(editor));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (Interlocked.CompareExchange(ref _isExecuting, 1, 0) != 0)
                return new EditorExecutionResult(context.ExecutionId, false, false,
                    "流程正在执行，不能重复启动。", new EditorExecutionStep[0]);

            try
            {
                return ExecuteCore(editor, context);
            }
            finally
            {
                Volatile.Write(ref _isExecuting, 0);
            }
        }

        private EditorExecutionResult ExecuteCore(STNodeEditor editor,
            EditorExecutionContext templateContext)
        {
            STNode[] startNodes = editor.Nodes.ToArray().Where(node =>
                    node is IEditorStartNode && node is IEditorExecutableNode)
                .ToArray();
            if (startNodes.Length == 0)
                return CreateResult(templateContext, false, false,
                    "流程中没有开始节点。", new List<EditorExecutionStep>());
            if (templateContext.MaxSteps < 1)
                return CreateResult(templateContext, false, false,
                    "最大执行步骤数必须大于 0。",
                    new List<EditorExecutionStep>());

            string validationError = ValidateIndependentFlows(startNodes);
            if (validationError != null)
                return CreateResult(templateContext, false, false,
                    validationError, new List<EditorExecutionStep>());

            Task[] flowTasks = startNodes.Select(startNode => Task.Run(() =>
                RunFlowLoop(startNode, templateContext))).ToArray();
            try
            {
                Task.WaitAll(flowTasks);
            }
            catch (AggregateException ex)
            {
                Exception error = ex.Flatten().InnerExceptions.FirstOrDefault();
                return CreateResult(templateContext, false, false,
                    error == null ? "并行流程执行异常。" :
                    error.GetBaseException().Message,
                    new List<EditorExecutionStep>());
            }

            return CreateResult(templateContext, false,
                templateContext.CancellationToken.IsCancellationRequested,
                templateContext.CancellationToken.IsCancellationRequested
                    ? "流程执行已取消。"
                    : "所有独立流程已停止。",
                new List<EditorExecutionStep>());
        }

        private void RunFlowLoop(STNode startNode,
            EditorExecutionContext templateContext)
        {
            while (!templateContext.CancellationToken.IsCancellationRequested)
            {
                EditorExecutionContext context = templateContext.CreateCycleContext();
                EditorExecutionResult result = ExecuteFlowCycle(startNode, context);
                templateContext.ReportFlowCycle(result);

                if (result.IsCanceled ||
                    templateContext.CancellationToken.IsCancellationRequested)
                    break;

                int delayMilliseconds = result.IsSuccess
                    ? templateContext.GetSuccessfulCycleDelayMilliseconds()
                    : templateContext.GetFailedCycleDelayMilliseconds();
                if (delayMilliseconds > 0 &&
                    templateContext.CancellationToken.WaitHandle.WaitOne(
                        delayMilliseconds))
                    break;
            }
        }

        private EditorExecutionResult ExecuteFlowCycle(STNode startNode,
            EditorExecutionContext context)
        {
            context.BeginExecution();
            var steps = new List<EditorExecutionStep>();
            var pending = new Queue<STNode>();
            var queued = new HashSet<STNode>();
            var waiting = new Dictionary<STNode, string>();
            bool hasFailure = false;
            Enqueue(startNode, pending, queued);

            while (pending.Count > 0)
            {
                if (context.CancellationToken.IsCancellationRequested)
                    return CreateResult(context, false, true,
                        "流程“" + GetNodeDisplayName(startNode) +
                        "”执行已取消。", steps);
                if (steps.Count >= context.MaxSteps)
                    return CreateResult(context, false, false,
                        "流程“" + GetNodeDisplayName(startNode) +
                        "”超过最大执行步骤数 " + context.MaxSteps + "。", steps);

                STNode node = pending.Dequeue();
                queued.Remove(node);
                context.PrepareNodeInputs(node);

                EditorNodeReadinessResult readinessResult;
                try
                {
                    var readiness = node as IEditorNodeReadiness;
                    readinessResult = readiness == null
                        ? EditorNodeReadinessResult.Ready()
                        : readiness.CanExecute(context);
                }
                catch (Exception ex)
                {
                    return CreateResult(context, false, false,
                        "节点“" + GetNodeDisplayName(node) +
                        "”就绪检查失败：" + ex.GetBaseException().Message,
                        steps);
                }

                if (readinessResult == null || !readinessResult.IsReady)
                {
                    waiting[node] = readinessResult == null
                        ? "节点未返回有效的就绪状态。"
                        : readinessResult.Message;
                    continue;
                }

                var executable = node as IEditorExecutableNode;
                if (executable == null)
                    return CreateResult(context, false, false,
                        "节点“" + GetNodeDisplayName(node) +
                        "”不支持流程执行。", steps);

                waiting.Remove(node);
                NodeRunResult runResult = ExecuteNode(startNode.Guid, node,
                    executable, context, !(node is IEditorStartNode));
                if (runResult.IsCanceled)
                    return CreateResult(context, false, true,
                        "流程“" + GetNodeDisplayName(startNode) +
                        "”执行已取消。", steps);

                EditorNodeExecutionResult nodeResult = runResult.ExecutionResult;
                steps.Add(new EditorExecutionStep(steps.Count + 1, node,
                    nodeResult.IsSuccess, nodeResult.Message, runResult.Elapsed,
                    nodeResult.ActiveOutputs));
                context.ConsumeNodeInputs(node);

                if (!nodeResult.IsSuccess)
                {
                    hasFailure = true;
                    if (context.StopOnFailure)
                        return CreateResult(context, false, false,
                            "流程“" + GetNodeDisplayName(startNode) +
                            "”的节点“" + GetNodeDisplayName(node) +
                            "”执行失败：" + nodeResult.Message, steps);
                    continue;
                }

                string outputError = ActivateOutputs(node,
                    nodeResult.ActiveOutputs, context, pending, queued);
                if (outputError != null)
                    return CreateResult(context, false, false,
                        outputError, steps);

                if (context.HasPendingInputs(node))
                    Enqueue(node, pending, queued);
            }

            if (waiting.Count > 0)
            {
                string detail = string.Join("；", waiting.Select(item =>
                    "“" + GetNodeDisplayName(item.Key) + "”：" + item.Value));
                return CreateResult(context, false, false,
                    "流程“" + GetNodeDisplayName(startNode) +
                    "”存在已触发但输入未就绪的节点：" + detail, steps);
            }

            return CreateResult(context, !hasFailure, false,
                hasFailure
                    ? "流程“" + GetNodeDisplayName(startNode) +
                      "”执行完成，但有节点执行失败。"
                    : "流程“" + GetNodeDisplayName(startNode) +
                      "”执行完成。",
                steps);
        }

        private NodeRunResult ExecuteNode(Guid flowStartNodeGuid, STNode node,
            IEditorExecutableNode executable, EditorExecutionContext context,
            bool applyTransitionDelay)
        {
            if (applyTransitionDelay)
            {
                int transitionDelay = context.GetNodeTransitionDelayMilliseconds();
                if (transitionDelay > 0 &&
                    context.CancellationToken.WaitHandle.WaitOne(transitionDelay))
                    return NodeRunResult.Canceled();
            }

            if (context.CancellationToken.IsCancellationRequested)
                return NodeRunResult.Canceled();

            var stopwatch = Stopwatch.StartNew();
            OnNodeExecutionChanged(flowStartNodeGuid, node, false, true,
                string.Empty, string.Empty);
            EditorNodeExecutionResult nodeResult;
            try
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                nodeResult = executable.Execute(context);
            }
            catch (OperationCanceledException)
            {
                stopwatch.Stop();
                OnNodeExecutionChanged(flowStartNodeGuid, node, true, false,
                    "流程执行已取消。", "已取消");
                return NodeRunResult.Canceled();
            }
            catch (Exception ex)
            {
                nodeResult = EditorNodeExecutionResult.Failure(
                    ex.GetBaseException().Message);
            }
            stopwatch.Stop();

            if (nodeResult == null)
                nodeResult = EditorNodeExecutionResult.Failure(
                    "节点未返回执行结果。");
            string runtimeValueText = nodeResult.IsSuccess
                ? FormatRuntimeOutputs(nodeResult.ActiveOutputs)
                : "错误：" + nodeResult.Message;
            OnNodeExecutionChanged(flowStartNodeGuid, node, true,
                nodeResult.IsSuccess, nodeResult.Message, runtimeValueText);
            return NodeRunResult.Completed(nodeResult, stopwatch.Elapsed);
        }

        private static string FormatRuntimeOutputs(
            IEnumerable<STNodeOption> outputs)
        {
            string[] values = (outputs ?? Enumerable.Empty<STNodeOption>())
                .Where(option => option != null &&
                                 !(option.Data is EditorFlowSignal))
                .Select(option =>
                {
                    string name = string.IsNullOrWhiteSpace(option.Text)
                        ? "输出"
                        : option.Text.Trim();
                    return name + "：" + FormatRuntimeValue(option.Data);
                })
                .ToArray();
            return LimitRuntimeText(string.Join("；", values));
        }

        private static string FormatRuntimeValue(object value)
        {
            if (value == null) return "null";
            var array = value as Array;
            if (array != null)
            {
                const int maximumItems = 8;
                string[] items = new string[Math.Min(array.Length, maximumItems)];
                for (int index = 0; index < items.Length; index++)
                    items[index] = Convert.ToString(array.GetValue(index),
                        CultureInfo.InvariantCulture) ?? string.Empty;
                return "[" + string.Join(", ", items) +
                       (array.Length > maximumItems ? ", ..." : string.Empty) +
                       "]";
            }

            var formattable = value as IFormattable;
            return formattable == null
                ? Convert.ToString(value, CultureInfo.InvariantCulture) ??
                  string.Empty
                : formattable.ToString(null, CultureInfo.InvariantCulture);
        }

        private static string LimitRuntimeText(string text)
        {
            const int maximumLength = 120;
            if (string.IsNullOrEmpty(text) || text.Length <= maximumLength)
                return text ?? string.Empty;
            return text.Substring(0, maximumLength - 3) + "...";
        }

        private static string ValidateIndependentFlows(STNode[] startNodes)
        {
            var owners = new Dictionary<STNode, STNode>();
            foreach (STNode startNode in startNodes)
            {
                var pending = new Queue<STNode>();
                var visited = new HashSet<STNode>();
                pending.Enqueue(startNode);

                while (pending.Count > 0)
                {
                    STNode node = pending.Dequeue();
                    if (!visited.Add(node)) continue;

                    STNode existingOwner;
                    if (owners.TryGetValue(node, out existingOwner) &&
                        !ReferenceEquals(existingOwner, startNode))
                    {
                        return "多流程不允许交叉：节点“" +
                               GetNodeDisplayName(node) + "”同时连接到开始流程“" +
                               GetNodeDisplayName(existingOwner) + "”和“" +
                               GetNodeDisplayName(startNode) + "”。";
                    }
                    owners[node] = startNode;

                    foreach (STNode nextNode in GetConnectedNodes(node))
                        pending.Enqueue(nextNode);
                }
            }
            return null;
        }

        private static IEnumerable<STNode> GetConnectedNodes(STNode node)
        {
            if (node == null || node.Owner == null)
                return Enumerable.Empty<STNode>();
            return node.Owner.GetConnectionInfo(node,
                    node.GetOutputOptions() ?? new STNodeOption[0])
                .Select(connection => connection.Input)
                .Where(input => input != null && input.Owner != null)
                .Select(input => input.Owner);
        }

        private static string ActivateOutputs(STNode node,
            IEnumerable<STNodeOption> outputs, EditorExecutionContext context,
            Queue<STNode> pending, HashSet<STNode> queued)
        {
            STNodeOption[] activeOutputs = (outputs ?? Enumerable.Empty<STNodeOption>())
                .Where(option => option != null).Distinct().ToArray();
            foreach (STNodeOption output in activeOutputs)
            {
                if (output.Owner != node || output.IsInput)
                    return "节点“" + GetNodeDisplayName(node) +
                           "”返回了不属于自身的输出端口。";
            }

            if (node.Owner == null)
                return "节点“" + GetNodeDisplayName(node) + "”不属于当前流程画布。";

            foreach (ConnectionInfo connection in
                     node.Owner.GetConnectionInfo(node, activeOutputs))
            {
                STNodeOption input = connection.Input;
                if (input == null || input.Owner == null) continue;
                context.MarkInputActivated(input, connection.Output.Data);
                STNode nextNode = input.Owner;
                if (!(nextNode is IEditorExecutableNode))
                    return "输出“" + connection.Output.Text + "”连接的节点“" +
                           GetNodeDisplayName(nextNode) +
                           "”不支持流程执行。";
                Enqueue(nextNode, pending, queued);
            }
            return null;
        }

        private static void Enqueue(STNode node, Queue<STNode> pending,
            HashSet<STNode> queued)
        {
            if (node == null || !queued.Add(node)) return;
            pending.Enqueue(node);
        }

        private static string GetNodeDisplayName(STNode node)
        {
            if (node == null) return "未知节点";
            return string.IsNullOrWhiteSpace(node.Title)
                ? node.GetType().Name
                : node.Title;
        }

        private void OnNodeExecutionChanged(Guid flowStartNodeGuid, STNode node,
            bool isCompleted, bool isSuccess, string message,
            string runtimeValueText)
        {
            EventHandler<EditorNodeExecutionEventArgs> handler = NodeExecutionChanged;
            if (handler == null) return;
            var args = new EditorNodeExecutionEventArgs(flowStartNodeGuid, node,
                isCompleted, isSuccess, message, runtimeValueText);
            foreach (EventHandler<EditorNodeExecutionEventArgs> subscriber in
                     handler.GetInvocationList())
            {
                try { subscriber(this, args); }
                catch { }
            }
        }

        private static EditorExecutionResult CreateResult(
            EditorExecutionContext context, bool isSuccess, bool isCanceled,
            string message, List<EditorExecutionStep> steps)
        {
            return new EditorExecutionResult(context.ExecutionId, isSuccess,
                isCanceled, message, steps.ToArray());
        }

        private sealed class NodeRunResult
        {
            private NodeRunResult(bool isCanceled,
                EditorNodeExecutionResult executionResult, TimeSpan elapsed)
            {
                IsCanceled = isCanceled;
                ExecutionResult = executionResult;
                Elapsed = elapsed;
            }

            public bool IsCanceled { get; }
            public EditorNodeExecutionResult ExecutionResult { get; }
            public TimeSpan Elapsed { get; }

            public static NodeRunResult Canceled()
            {
                return new NodeRunResult(true, null, TimeSpan.Zero);
            }

            public static NodeRunResult Completed(
                EditorNodeExecutionResult executionResult, TimeSpan elapsed)
            {
                return new NodeRunResult(false, executionResult, elapsed);
            }
        }
    }
}
