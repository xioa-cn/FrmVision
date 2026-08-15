using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FrmServices.Services.EditorServices;
using ST.Library.UI.NodeEditor;

namespace FrmViews.Nodes
{
    public enum GlobalDataMode
    {
        Read,
        Write
    }

    public sealed class GlobalDataChangedEventArgs : EventArgs
    {
        public GlobalDataChangedEventArgs(string key, object value, bool exists)
        {
            Key = key;
            Value = value;
            Exists = exists;
        }

        public string Key { get; }
        public object Value { get; }
        public bool Exists { get; }
    }

    public static class GlobalDataStore
    {
        private static readonly ConcurrentDictionary<string, object> Data =
            new ConcurrentDictionary<string, object>(
                StringComparer.OrdinalIgnoreCase);

        internal static event EventHandler<GlobalDataChangedEventArgs> Changed;

        public static void Set(string key, object value)
        {
            string normalizedKey = NormalizeKey(key);
            if (value == null)
                throw new ArgumentNullException(nameof(value),
                    "全局数据值不能为空。");
            Data[normalizedKey] = value;
            RaiseChanged(new GlobalDataChangedEventArgs(normalizedKey, value,
                true));
        }

        public static bool TryGet(string key, out object value)
        {
            string normalizedKey = NormalizeKey(key);
            return Data.TryGetValue(normalizedKey, out value);
        }

        public static bool Remove(string key)
        {
            string normalizedKey = NormalizeKey(key);
            object removed;
            bool result = Data.TryRemove(normalizedKey, out removed);
            if (result)
                RaiseChanged(new GlobalDataChangedEventArgs(normalizedKey, null,
                    false));
            return result;
        }

        public static void Clear()
        {
            Data.Clear();
            RaiseChanged(new GlobalDataChangedEventArgs(string.Empty, null,
                false));
        }

        public static IDictionary<string, object> Snapshot()
        {
            return Data.ToDictionary(item => item.Key, item => item.Value,
                StringComparer.OrdinalIgnoreCase);
        }

        private static string NormalizeKey(string key)
        {
            string normalizedKey = (key ?? string.Empty).Trim();
            if (normalizedKey.Length == 0)
                throw new ArgumentException("全局数据 Key 不能为空。",
                    nameof(key));
            return normalizedKey;
        }

        private static void RaiseChanged(GlobalDataChangedEventArgs args)
        {
            EventHandler<GlobalDataChangedEventArgs> handler = Changed;
            if (handler == null) return;
            foreach (EventHandler<GlobalDataChangedEventArgs> subscriber in
                     handler.GetInvocationList())
            {
                try { subscriber(null, args); }
                catch { }
            }
        }
    }

    public sealed class GlobalDataNodeExecutionResult
    {
        public bool IsSuccess { get; set; }
        public GlobalDataMode Mode { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
        public string Message { get; set; }
    }

    [STNode("数据源", "xioa", null, null,
        "按 Key 写入或读取进程内共享的全局数据。")]
    public sealed class GlobalDataNode : WorkflowNode, IEditorExecutableNode,
        IEditorNodeReadiness
    {
        private readonly STNodeOption _input;
        private readonly STNodeOption _output;
        private GlobalDataMode _mode = GlobalDataMode.Read;
        private string _key = string.Empty;
        private bool _subscribed;

        public GlobalDataNode()
        {
            SetNodeTypeTitle("全局数据");
            EnableExecutionLog = true;
            TitleColor = System.Drawing.Color.FromArgb(220, 93, 75, 145);
            LetGetOptions = true;

            _input = new STNodeOption("写入", typeof(object), true);
            _output = new STNodeOption("读取", typeof(object), false);
            _input.DataTransfer += InputOnDataTransfer;
            ConfigurePorts();
        }

        public STNodeOption Input => Mode == GlobalDataMode.Write
            ? _input
            : null;

        public STNodeOption Output => Mode == GlobalDataMode.Read
            ? _output
            : null;

        [STNodeProperty("模式", "读取模式只有输出，写入模式只有输入。",
            DescriptorType = typeof(GlobalDataModePropertyDescriptor))]
        public GlobalDataMode Mode
        {
            get { return _mode; }
            set
            {
                if (_mode == value && PortsMatchMode()) return;
                _mode = value;
                ConfigurePorts();
            }
        }

        [STNodeProperty("数据 Key", "全局数据字典中的唯一名称，不区分大小写。")]
        public string Key
        {
            get { return _key; }
            set
            {
                _key = (value ?? string.Empty).Trim();
                if (_mode == GlobalDataMode.Read)
                    RefreshOutput();
                else
                    TryWriteCurrentInput();
            }
        }

        public GlobalDataNodeExecutionResult Execute()
        {
            try
            {
                string normalizedKey = RequireKey();
                if (Mode == GlobalDataMode.Write)
                {
                    if (_input.Data == null)
                        throw new InvalidOperationException(
                            "全局数据写入值不能为空。");
                    GlobalDataStore.Set(normalizedKey, _input.Data);
                    return Success(_input.Data,
                        "全局数据“" + normalizedKey + "”已写入。");
                }

                object value;
                if (!GlobalDataStore.TryGet(normalizedKey, out value))
                    throw new InvalidOperationException(
                        "全局数据“" + normalizedKey + "”尚未写入。");
                _output.Data = value;
                _output.TransferData();
                return Success(value,
                    "已读取全局数据“" + normalizedKey + "”。");
            }
            catch (Exception ex)
            {
                return new GlobalDataNodeExecutionResult
                {
                    IsSuccess = false,
                    Mode = Mode,
                    Key = _key,
                    Message = ex.GetBaseException().Message
                };
            }
        }

        public EditorNodeExecutionResult Execute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            context.CancellationToken.ThrowIfCancellationRequested();
            GlobalDataNodeExecutionResult result = Execute();
            if (!result.IsSuccess)
                return EditorNodeExecutionResult.Failure(result.Message);
            return Mode == GlobalDataMode.Read
                ? EditorNodeExecutionResult.Success(result.Message, _output)
                : EditorNodeExecutionResult.Success(result.Message);
        }

        public EditorNodeReadinessResult CanExecute(EditorExecutionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(Key))
                return EditorNodeReadinessResult.NotReady(
                    "等待配置全局数据 Key。");
            if (Mode == GlobalDataMode.Read)
            {
                return RefreshOutput()
                    ? EditorNodeReadinessResult.Ready()
                    : EditorNodeReadinessResult.NotReady(
                        "等待全局数据“" + Key + "”写入。");
            }
            return context.IsInputActivated(_input) && _input.Data != null
                ? EditorNodeReadinessResult.Ready()
                : EditorNodeReadinessResult.NotReady("等待全局数据写入值。");
        }

        public static bool IsReadSource(STNodeOption input)
        {
            if (input == null) return false;
            return input.GetConnectedOption().Any(option =>
            {
                var node = option == null ? null : option.Owner as GlobalDataNode;
                return node != null && node.Mode == GlobalDataMode.Read &&
                       ReferenceEquals(option, node._output) &&
                       node.RefreshOutput();
            });
        }

        protected override void OnOwnerChanged()
        {
            base.OnOwnerChanged();
            if (Owner != null && !_subscribed)
            {
                GlobalDataStore.Changed += GlobalDataStoreOnChanged;
                _subscribed = true;
                RefreshOutput();
            }
            else if (Owner == null && _subscribed)
            {
                GlobalDataStore.Changed -= GlobalDataStoreOnChanged;
                _subscribed = false;
            }
        }

        private void ConfigurePorts()
        {
            while (InputOptions.Count > 0)
                InputOptions.RemoveAt(InputOptions.Count - 1);
            while (OutputOptions.Count > 0)
                OutputOptions.RemoveAt(OutputOptions.Count - 1);

            if (_mode == GlobalDataMode.Write)
            {
                _output.Data = null;
                InputOptions.Add(_input);
                TryWriteCurrentInput();
            }
            else
            {
                OutputOptions.Add(_output);
                RefreshOutput();
            }

            BuildSize(true, true, false);
            if (Owner != null) Owner.Invalidate();
        }

        private bool PortsMatchMode()
        {
            return _mode == GlobalDataMode.Write
                ? InputOptions.Contains(_input) && OutputOptions.Count == 0
                : OutputOptions.Contains(_output) && InputOptions.Count == 0;
        }

        private bool RefreshOutput()
        {
            if (_mode != GlobalDataMode.Read ||
                string.IsNullOrWhiteSpace(_key))
            {
                _output.Data = null;
                return false;
            }

            object value;
            if (!GlobalDataStore.TryGet(_key, out value))
            {
                _output.Data = null;
                _output.TransferData();
                return false;
            }
            _output.Data = value;
            _output.TransferData();
            return true;
        }

        private string RequireKey()
        {
            string normalizedKey = (_key ?? string.Empty).Trim();
            if (normalizedKey.Length == 0)
                throw new InvalidOperationException("全局数据 Key 不能为空。");
            return normalizedKey;
        }

        private bool TryWriteCurrentInput()
        {
            if (_mode != GlobalDataMode.Write || _input.Data == null ||
                string.IsNullOrWhiteSpace(_key))
                return false;

            GlobalDataStore.Set(_key, _input.Data);
            return true;
        }

        private GlobalDataNodeExecutionResult Success(object value,
            string message)
        {
            return new GlobalDataNodeExecutionResult
            {
                IsSuccess = true,
                Mode = Mode,
                Key = _key,
                Value = value,
                Message = message
            };
        }

        private void GlobalDataStoreOnChanged(object sender,
            GlobalDataChangedEventArgs e)
        {
            if (_mode != GlobalDataMode.Read) return;
            if (string.IsNullOrEmpty(e.Key) ||
                string.Equals(e.Key, _key, StringComparison.OrdinalIgnoreCase))
                RefreshOutput();
        }

        private void InputOnDataTransfer(object sender, STNodeOptionEventArgs e)
        {
            _input.Data = e.Status == ConnectionStatus.Connected &&
                          e.TargetOption != null
                ? e.TargetOption.Data
                : null;
            TryWriteCurrentInput();
        }
    }

    public sealed class GlobalDataModePropertyDescriptor :
        STNodePropertyDescriptor
    {
        protected override object GetValueFromString(string text)
        {
            switch ((text ?? string.Empty).Trim())
            {
                case "读取": return GlobalDataMode.Read;
                case "写入": return GlobalDataMode.Write;
                default:
                    return Enum.Parse(typeof(GlobalDataMode), text, true);
            }
        }

        protected override string GetStringFromValue()
        {
            return GetText((GlobalDataMode)GetValue(null));
        }

        protected override string GetSelectItemText(object value)
        {
            return GetText((GlobalDataMode)value);
        }

        private static string GetText(GlobalDataMode mode)
        {
            return mode == GlobalDataMode.Read ? "读取" : "写入";
        }
    }
}
