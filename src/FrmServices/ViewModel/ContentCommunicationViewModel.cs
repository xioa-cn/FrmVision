using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FrmMapper.Data;
using FrmServices.LogServices;
using FrmServices.Services.CommunicationServices;
using FrmServices.Services.CommunicationServices.Services;
using HslCommunication.Core;
using Newtonsoft.Json;

namespace FrmServices.ViewModel;

public enum ContentCommunicationType { TCPSERVICE, TCPCLIENT, MODBUSTCP, MODBUSRTU }

public sealed class ContentCommunicationConfiguration
{
    [Browsable(false)] public string Id { get; set; } = Guid.NewGuid().ToString("N");
    [Category("基本设置"), DisplayName("名称")] public string Name { get; set; } = "内部通讯";
    [Category("基本设置"), DisplayName("随软件启动"),
     Description("启动通讯后自动记住为启用；手动停止后取消。退出软件不会清除该设置。旧版配置默认启用。")]
    public bool AutoStart { get; set; } = true;
    [Browsable(false)] public ContentCommunicationType Type { get; set; }
    [Category("网络"), DisplayName("IP / 主机名"), Description("TCP 服务端填写本地 IP，0.0.0.0 监听所有网卡；客户端填写远端 IP 或主机名。")]
    public string Host { get; set; } = "127.0.0.1";
    [Category("网络"), DisplayName("端口")] public int Port { get; set; } = 5000;
    [Category("TCP"), DisplayName("连接超时 (ms)")] public int ConnectTimeout { get; set; } = 5000;
    [Category("TCP"), DisplayName("自动重连"), Description("首次连接失败或连接断开后持续重试；手动停止通讯后停止重连。")]
    public bool AutoReconnect { get; set; } = true;
    [Category("TCP"), DisplayName("重连间隔 (ms)"), Description("自动重连的重试间隔，必须大于 0。")]
    public int ReconnectInterval { get; set; } = 2000;
    [Category("TCP"), DisplayName("文本编码"), TypeConverter(typeof(CommunicationEncodingConverter)),
     Description("必须与发送端一致：UTF-8 选 utf-8；GBK/GB2312 选 gb2312；UTF-16 小端选 unicode。先停止通讯，修改保存后再启动。二进制数据请查看 HEX。")]
    public string EncodingName { get; set; } = "utf-8";
    [Category("TCP"), DisplayName("接收日志输出"),
     Description("开启时通过 AppLog 将接收文本写入主界面日志中心；关闭后不写入日志中心。通讯窗口的调试显示和数据接收不受影响。")]
    public bool ReceiveLogEnabled { get; set; } = true;
    [Category("串口"), DisplayName("串口名称")] public string SerialPort { get; set; } = "COM1";
    [Category("串口"), DisplayName("波特率")] public int BaudRate { get; set; } = 9600;
    [Category("串口"), DisplayName("数据位")] public int DataBits { get; set; } = 8;
    [Category("串口"), DisplayName("校验位")] public Parity Parity { get; set; } = Parity.None;
    [Category("串口"), DisplayName("停止位")] public StopBits StopBits { get; set; } = StopBits.One;
    [Category("Modbus"), DisplayName("站号")] public int Station { get; set; } = 1;
    [Category("Modbus"), DisplayName("数据格式"), Description("ABCD、BADC、CDAB 或 DCBA。")]
    public string DataFormat { get; set; } = "CDAB";
    [JsonIgnore] public string TypeText => GetTypeText(Type);
    [JsonIgnore] public string Endpoint => Type == ContentCommunicationType.MODBUSRTU
        ? SerialPort + " / " + BaudRate : (Type == ContentCommunicationType.MODBUSTCP ? "0.0.0.0" : Host) + ":" + Port;
    public ContentCommunicationConfiguration Clone() => (ContentCommunicationConfiguration)MemberwiseClone();
    public static string GetTypeText(ContentCommunicationType type)
    {
        switch (type)
        {
            case ContentCommunicationType.TCPSERVICE: return "TCP 服务端";
            case ContentCommunicationType.TCPCLIENT: return "TCP 客户端";
            case ContentCommunicationType.MODBUSTCP: return "Modbus TCP 从站";
            case ContentCommunicationType.MODBUSRTU: return "Modbus RTU 从站";
            default: return "未知协议";
        }
    }
}

// The main window owns this instance; closing the configuration dialog keeps transports alive.
public sealed class ContentCommunicationViewModel : IDisposable
{
    private readonly object _sync = new object();
    private readonly SemaphoreSlim _operations = new SemaphoreSlim(1, 1);
    private readonly Dictionary<string, IDisposable> _runtimes = new Dictionary<string, IDisposable>();
    private readonly Dictionary<string, string> _startupErrors = new Dictionary<string, string>();
    private readonly HashSet<string> _starting = new HashSet<string>();
    private bool _disposed;
    public BindingList<ContentCommunicationConfiguration> Configurations { get; } = new BindingList<ContentCommunicationConfiguration>();
    public string ConfigurationFilePath { get; }
    public string ConfigurationLoadError { get; private set; }
    // Raised on transport threads; views must marshal updates.
    public event Action<string, string> MessageReceived;
    public event Action<string, byte[]> BytesReceived;

    public ContentCommunicationViewModel(string configurationFilePath = null)
    {
        ConfigurationFilePath = Path.GetFullPath(configurationFilePath ?? Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "communcation", "content-communication-config.json"));
        try
        {
            if (!File.Exists(ConfigurationFilePath)) return;
            var items = JsonConvert.DeserializeObject<List<ContentCommunicationConfiguration>>(File.ReadAllText(ConfigurationFilePath));
            if (items == null) throw new InvalidDataException("配置内容不能为空。");
            ValidateAll(items);
            foreach (var item in items) Configurations.Add(item);
        }
        catch (Exception ex) { ConfigurationLoadError = "内部通讯配置加载失败：" + ex.Message; }
    }

    public ContentCommunicationConfiguration CreateConfiguration(ContentCommunicationType type)
    {
        var prefix = ContentCommunicationConfiguration.GetTypeText(type);
        int index = 1;
        while (Configurations.Any(c => c.Name == prefix + " " + index)) index++;
        return new ContentCommunicationConfiguration { Name = prefix + " " + index, Type = type,
            Host = type == ContentCommunicationType.TCPSERVICE ? "0.0.0.0" : "127.0.0.1",
            Port = type == ContentCommunicationType.MODBUSTCP ? 502 : 5000 };
    }

    public Result SaveConfiguration(ContentCommunicationConfiguration draft)
    {
        lock (_sync)
        {
            try
            {
                EnsureAvailable();
                if (draft == null) return Result.Fail("请选择或新增通讯配置。");
                if (draft.Id != null && _runtimes.ContainsKey(draft.Id)) return Result.Fail("请先停止通讯，再修改配置。");
                var copy = draft.Clone();
                copy.Name = copy.Name?.Trim(); copy.Host = copy.Host?.Trim(); copy.SerialPort = copy.SerialPort?.Trim();
                var items = Configurations.Select(c => c.Clone()).ToList();
                int index = items.FindIndex(c => c.Id == copy.Id);
                if (index < 0) items.Add(copy); else items[index] = copy;
                ValidateAll(items);
                Persist(items);
                if (index < 0) Configurations.Add(copy); else Configurations[index] = copy;
                _startupErrors.Remove(copy.Id);
                return Result.Ok();
            }
            catch (Exception ex) { return Result.Fail(ex.Message); }
        }
    }

    public Result DeleteConfiguration(string id)
    {
        lock (_sync)
        {
            try
            {
                EnsureAvailable();
                if (_runtimes.ContainsKey(id)) return Result.Fail("请先停止通讯，再删除配置。");
                var item = Configurations.FirstOrDefault(c => c.Id == id);
                if (item == null) return Result.Fail("配置不存在。");
                Persist(Configurations.Where(c => c.Id != id).ToList());
                Configurations.Remove(item);
                _startupErrors.Remove(id);
                return Result.Ok();
            }
            catch (Exception ex) { return Result.Fail(ex.Message); }
        }
    }

    public static string ValidateConfiguration(ContentCommunicationConfiguration c)
    {
        if (c == null || string.IsNullOrWhiteSpace(c.Id)) return "配置及 ID 不能为空。";
        if (string.IsNullOrWhiteSpace(c.Name)) return "名称不能为空。";
        if (!Enum.IsDefined(typeof(ContentCommunicationType), c.Type)) return "不支持该通讯类型。";
        bool tcp = c.Type == ContentCommunicationType.TCPSERVICE || c.Type == ContentCommunicationType.TCPCLIENT;
        if (c.Type != ContentCommunicationType.MODBUSRTU && (c.Port < 1 || c.Port > 65535)) return "端口范围必须为 1–65535。";
        if (tcp)
        {
            if (string.IsNullOrWhiteSpace(c.Host)) return "IP / 主机名不能为空。";
            if (c.Type == ContentCommunicationType.TCPSERVICE && !IPAddress.TryParse(c.Host.Trim(), out _)) return "TCP 服务端必须填写有效的本地 IP。";
            if (c.Type == ContentCommunicationType.TCPCLIENT && c.ConnectTimeout <= 0) return "连接超时必须大于 0。";
            if (c.Type == ContentCommunicationType.TCPCLIENT && c.ReconnectInterval <= 0) return "重连间隔必须大于 0。";
            try { Encoding.GetEncoding(c.EncodingName ?? ""); }
            catch (ArgumentException) { return "文本编码无效。"; }
        }
        else
        {
            if (c.Station < 1 || c.Station > 247) return "Modbus 站号范围必须为 1–247。";
            if (!Enum.TryParse(c.DataFormat, out DataFormat format) || !Enum.IsDefined(typeof(DataFormat), format)) return "数据格式必须为 ABCD、BADC、CDAB 或 DCBA。";
        }
        if (c.Type == ContentCommunicationType.MODBUSRTU)
        {
            if (string.IsNullOrWhiteSpace(c.SerialPort)) return "串口名称不能为空。";
            if (c.BaudRate <= 0) return "波特率必须大于 0。";
            if (c.DataBits < 5 || c.DataBits > 8) return "数据位范围必须为 5–8。";
            if (!Enum.IsDefined(typeof(Parity), c.Parity)) return "校验位无效。";
            if (!Enum.IsDefined(typeof(StopBits), c.StopBits) || c.StopBits == StopBits.None) return "停止位无效。";
        }
        return null;
    }

    private static void ValidateAll(IList<ContentCommunicationConfiguration> items)
    {
        var ids = new HashSet<string>();
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var c in items)
        {
            var error = ValidateConfiguration(c);
            if (error != null) throw new InvalidDataException(error);
            if (!ids.Add(c.Id)) throw new InvalidDataException("存在重复的配置 ID。");
            if (!names.Add(c.Name.Trim())) throw new InvalidDataException("通讯名称不能重复：" + c.Name);
        }
    }

    private void Persist(IList<ContentCommunicationConfiguration> items)
    {
        if (ConfigurationLoadError != null) throw new InvalidOperationException(ConfigurationLoadError + " 请先修复或备份并移走配置文件，然后重新启动软件。");
        Directory.CreateDirectory(Path.GetDirectoryName(ConfigurationFilePath));
        string temporary = ConfigurationFilePath + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.WriteAllText(temporary, JsonConvert.SerializeObject(items, Formatting.Indented), new UTF8Encoding(false));
            if (File.Exists(ConfigurationFilePath)) File.Replace(temporary, ConfigurationFilePath, null);
            else File.Move(temporary, ConfigurationFilePath);
        }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
    }

    public Task<Result> StartAsync(string id) => StartCoreAsync(id, false);

    // Called by application startup, independently of whether the settings dialog is opened.
    public async Task<Result> RestoreStartedCommunicationsAsync()
    {
        string[] ids;
        lock (_sync)
        {
            if (_disposed) return Result.Fail("通讯已关闭。");
            if (ConfigurationLoadError != null) return Result.Fail(ConfigurationLoadError);
            ids = Configurations.Where(c => c.AutoStart)
                .OrderBy(c => c.Type == ContentCommunicationType.TCPCLIENT ? 1 : 0)
                .Select(c => c.Id).ToArray();
        }
        var errors = new List<string>();
        foreach (string id in ids)
        {
            lock (_sync) { if (_disposed) return Result.Fail("通讯已关闭。"); }
            var result = await StartCoreAsync(id, true).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                lock (_sync)
                {
                    string name = Configurations.FirstOrDefault(c => c.Id == id)?.Name ?? id;
                    errors.Add(name + "：" + result.Message);
                }
            }
        }
        return errors.Count == 0 ? Result.Ok() : Result.Fail(string.Join(Environment.NewLine, errors));
    }

    private async Task<Result> StartCoreAsync(string id, bool restoring)
    {
        await _operations.WaitAsync().ConfigureAwait(false);
        IDisposable runtime = null;
        try
        {
            lock (_sync)
            {
                EnsureAvailable();
                var c = Configurations.FirstOrDefault(item => item.Id == id);
                if (c == null) return restoring ? Result.Ok() : Result.Fail("请先保存配置。");
                // A manual stop or edit while restoration was queued takes precedence.
                if (restoring && !c.AutoStart) return Result.Ok();
                if (_runtimes.TryGetValue(id, out runtime))
                {
                    if (IsStarted(runtime) || runtime is ContentTcpClient activeClient && activeClient.IsRunning)
                    {
                        if (!restoring) SetAutoStart(id, true);
                        return Result.Ok();
                    }
                }
                else
                {
                    var error = ValidateConfiguration(c);
                    if (error != null) return Result.Fail(error);
                    runtime = CreateRuntime(c);
                    _runtimes.Add(id, runtime);
                }
                _startupErrors.Remove(id);
                _starting.Add(id);
            }
            var tcp = runtime as IContentServer;
            var result = tcp != null ? await tcp.StartAsync().ConfigureAwait(false)
                : await Task.Run(() => ((IContentNodeServer)runtime).Start()).ConfigureAwait(false);
            lock (_sync)
            {
                if (_disposed) return Result.Fail("通讯已关闭。");
                // Keep the client alive when the peer is unavailable at startup.
                if (!result.IsSuccess && runtime is ContentTcpClient reconnecting && reconnecting.IsRunning)
                    result = Result.Ok();
                if (!result.IsSuccess)
                {
                    RemoveRuntime(id);
                    _startupErrors[id] = string.IsNullOrWhiteSpace(result.Message) ? "通讯服务未能启动。" : result.Message;
                    return Result.Fail(_startupErrors[id]);
                }
                if (!restoring) SetAutoStart(id, true);
            }
            return result;
        }
        catch (Exception ex)
        {
            lock (_sync)
            {
                if (runtime != null) RemoveRuntime(id);
                if (!_disposed && id != null) _startupErrors[id] = ex.Message;
            }
            return Result.Fail(ex.Message);
        }
        finally
        {
            lock (_sync) { if (id != null) _starting.Remove(id); }
            _operations.Release();
        }
    }

    private IDisposable CreateRuntime(ContentCommunicationConfiguration c)
    {
        IContentServer tcp;
        switch (c.Type)
        {
            case ContentCommunicationType.TCPSERVICE: tcp = new ContentTcpServer(c.Port, c.Host, Encoding.GetEncoding(c.EncodingName)); break;
            case ContentCommunicationType.TCPCLIENT: tcp = new ContentTcpClient(c.Host, c.Port, Encoding.GetEncoding(c.EncodingName), c.ConnectTimeout, c.AutoReconnect, c.ReconnectInterval); break;
            case ContentCommunicationType.MODBUSTCP:
                return new ContentModbusTcpServer(c.Port, (byte)c.Station, (DataFormat)Enum.Parse(typeof(DataFormat), c.DataFormat));
            case ContentCommunicationType.MODBUSRTU:
                return new ContentModbusRtuServer(c.SerialPort, c.BaudRate, c.DataBits, c.Parity, c.StopBits,
                    (byte)c.Station, (DataFormat)Enum.Parse(typeof(DataFormat), c.DataFormat));
            default: throw new InvalidOperationException("不支持该通讯类型。");
        }
        tcp.Receive += text =>
        {
            // Logging belongs to the runtime, not the settings dialog's lifetime.
            if (c.ReceiveLogEnabled)
                AppLog.Info("[" + c.Name + "] " + c.TypeText + " 接收文本 (" + c.EncodingName + ")：" + text,
                    nameof(ContentCommunicationViewModel));
            MessageReceived?.Invoke(c.Id, text);
        };
        tcp.ReceiveBytes += bytes => BytesReceived?.Invoke(c.Id, bytes);
        return tcp;
    }

    public async Task<Result> StopAsync(string id)
    {
        await _operations.WaitAsync().ConfigureAwait(false);
        try
        {
            return await Task.Run(() =>
            {
                lock (_sync)
                {
                    EnsureAvailable();
                    RemoveRuntime(id);
                    SetAutoStart(id, false);
                    _startupErrors.Remove(id);
                }
                return Result.Ok();
            }).ConfigureAwait(false);
        }
        catch (Exception ex) { return Result.Fail(ex.Message); }
        finally { _operations.Release(); }
    }

    public async Task<Result> SendAsync(string id, string text)
    {
        try
        {
            IContentServer tcp;
            lock (_sync) { EnsureAvailable(); tcp = _runtimes.TryGetValue(id, out var runtime) ? runtime as IContentServer : null; }
            if (tcp == null) return Result.Fail("请先启动 TCP 通讯。");
            if (string.IsNullOrEmpty(text)) return Result.Fail("发送内容不能为空。");
            await tcp.Send(text).ConfigureAwait(false);
            return Result.Ok();
        }
        catch (Exception ex) { return Result.Fail(ex.Message); }
    }

    public bool HasRuntime(string id) { lock (_sync) return id != null && _runtimes.ContainsKey(id); }

    public ContentTcpServer ResolveTcpServer(string key)
    {
        lock (_sync)
        {
            EnsureAvailable();
            string normalized = (key ?? string.Empty).Trim();
            if (normalized.Length == 0) throw new InvalidOperationException("TCP 服务端名称不能为空。");
            var configuration = Configurations.FirstOrDefault(c => string.Equals(c.Id, normalized, StringComparison.OrdinalIgnoreCase))
                ?? Configurations.FirstOrDefault(c => string.Equals(c.Name, normalized, StringComparison.OrdinalIgnoreCase));
            if (configuration == null) throw new InvalidOperationException("未找到内置通讯配置：" + normalized + "。");
            if (configuration.Type != ContentCommunicationType.TCPSERVICE)
                throw new InvalidOperationException("通讯“" + configuration.Name + "”不是 TCP 服务端。");
            if (!_runtimes.TryGetValue(configuration.Id, out var runtime) || !(runtime is ContentTcpServer server) || !server.IsStarted)
                throw new InvalidOperationException("TCP 服务端“" + configuration.Name + "”未启动，请先启动内置通讯。");
            return server;
        }
    }

    public ContentTcpClient ResolveTcpClient(string key)
    {
        lock (_sync)
        {
            EnsureAvailable();
            string normalized = (key ?? string.Empty).Trim();
            if (normalized.Length == 0) throw new InvalidOperationException("TCP 客户端名称不能为空。");
            var configuration = Configurations.FirstOrDefault(c => string.Equals(c.Id, normalized, StringComparison.OrdinalIgnoreCase))
                ?? Configurations.FirstOrDefault(c => string.Equals(c.Name, normalized, StringComparison.OrdinalIgnoreCase));
            if (configuration == null) throw new InvalidOperationException("未找到内置通讯配置：" + normalized + "。");
            if (configuration.Type != ContentCommunicationType.TCPCLIENT)
                throw new InvalidOperationException("通讯“" + configuration.Name + "”不是 TCP 客户端。");
            if (!_runtimes.TryGetValue(configuration.Id, out var runtime) || !(runtime is ContentTcpClient client) || !client.IsRunning)
                throw new InvalidOperationException("TCP 客户端“" + configuration.Name + "”未连接，请先连接内置通讯。");
            return client;
        }
    }

    public ContentModbusTcpServer ResolveModbusTcpServer(string key)
    {
        lock (_sync)
        {
            EnsureAvailable();
            string normalized = (key ?? string.Empty).Trim();
            if (normalized.Length == 0) throw new InvalidOperationException("Modbus TCP 服务端名称不能为空。");
            var configuration = Configurations.FirstOrDefault(c => string.Equals(c.Id, normalized, StringComparison.OrdinalIgnoreCase))
                ?? Configurations.FirstOrDefault(c => string.Equals(c.Name, normalized, StringComparison.OrdinalIgnoreCase));
            if (configuration == null) throw new InvalidOperationException("未找到内置通讯配置：" + normalized + "。");
            if (configuration.Type != ContentCommunicationType.MODBUSTCP)
                throw new InvalidOperationException("通讯“" + configuration.Name + "”不是 Modbus TCP 从站。");
            if (!_runtimes.TryGetValue(configuration.Id, out var runtime) || !(runtime is ContentModbusTcpServer server) || !server.IsStarted)
                throw new InvalidOperationException("Modbus TCP 服务端“" + configuration.Name + "”未启动，请先启动内置通讯。");
            return server;
        }
    }

    public ContentModbusRtuServer ResolveModbusRtuServer(string key)
    {
        lock (_sync)
        {
            EnsureAvailable();
            string normalized = (key ?? string.Empty).Trim();
            if (normalized.Length == 0) throw new InvalidOperationException("Modbus RTU 服务端名称不能为空。");
            var configuration = Configurations.FirstOrDefault(c => string.Equals(c.Id, normalized, StringComparison.OrdinalIgnoreCase))
                ?? Configurations.FirstOrDefault(c => string.Equals(c.Name, normalized, StringComparison.OrdinalIgnoreCase));
            if (configuration == null) throw new InvalidOperationException("未找到内置通讯配置：" + normalized + "。");
            if (configuration.Type != ContentCommunicationType.MODBUSRTU)
                throw new InvalidOperationException("通讯“" + configuration.Name + "”不是 Modbus RTU 从站。");
            if (!_runtimes.TryGetValue(configuration.Id, out var runtime) || !(runtime is ContentModbusRtuServer server) || !server.IsStarted)
                throw new InvalidOperationException("Modbus RTU 服务端“" + configuration.Name + "”未启动，请先启动内置通讯。");
            return server;
        }
    }

    private void SetAutoStart(string id, bool enabled)
    {
        var configuration = Configurations.FirstOrDefault(c => c.Id == id);
        if (configuration == null) throw new InvalidOperationException("配置不存在。");
        if (configuration.AutoStart == enabled) return;
        var items = Configurations.Select(c => c.Clone()).ToList();
        items.First(c => c.Id == id).AutoStart = enabled;
        Persist(items);
        // Keep the bound item stable; no cross-thread BindingList notification is raised.
        configuration.AutoStart = enabled;
    }
    public string GetStatus(string id)
    {
        lock (_sync)
        {
            if (id != null && _starting.Contains(id)) return "启动中";
            if (id != null && _startupErrors.TryGetValue(id, out var error)) return "启动失败：" + error;
            if (id == null || !_runtimes.TryGetValue(id, out var runtime)) return "已停止";
            if (runtime is ContentTcpClient client && client.IsRunning && !client.IsStarted) return "重连中";
            if (!IsStarted(runtime)) return "已断开";
            if (runtime is ContentTcpServer server) return "监听中 / 客户端 " + server.ClientCount;
            return runtime is ContentTcpClient ? "已连接" : "运行中";
        }
    }
    private static bool IsStarted(IDisposable runtime) => runtime is IContentServer tcp ? tcp.IsStarted : ((IContentNodeServer)runtime).IsStarted;
    private void RemoveRuntime(string id)
    {
        if (!_runtimes.TryGetValue(id, out var runtime)) return;
        runtime.Dispose();
        _runtimes.Remove(id);
    }
    private void EnsureAvailable() { if (_disposed) throw new ObjectDisposedException(nameof(ContentCommunicationViewModel)); }
    public void Dispose()
    {
        lock (_sync)
        {
            if (_disposed) return;
            _disposed = true;
            foreach (var runtime in _runtimes.Values) runtime.Dispose();
            _runtimes.Clear();
            MessageReceived = null;
            BytesReceived = null;
        }
    }
}

public sealed class CommunicationEncodingConverter : StringConverter
{
    public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => false;
    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) =>
        new StandardValuesCollection(new[] { "utf-8", "gb2312", "gb18030", "ascii", "unicode", "big5" });
}
