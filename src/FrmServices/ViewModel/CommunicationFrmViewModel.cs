using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.IO.Ports;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using FrmCommon.ConfigUtils;
using FrmServices.Communication;
using FrmServices.Communication.LightSource;
using FrmServices.LogServices;
using HslCommunication.Core.Device;

namespace FrmServices.ViewModel;

public class GlobalCommunicationModel
{
    public GlobalCommunicationModel(ObservableCollection<PlcFrmVpCommunication> plcs,
        ObservableCollection<CameraFrmVpCommunication> cameras,
        ObservableCollection<LightSourceFrmVpCommunication> lightSources)
    {
        Plcs = plcs;
        Cameras = cameras;
        LightSources = lightSources;
    }

    public ObservableCollection<PlcFrmVpCommunication> Plcs { get; set; }
    public ObservableCollection<CameraFrmVpCommunication> Cameras { get; set; }
    public ObservableCollection<LightSourceFrmVpCommunication> LightSources { get; set; }
}

public enum CommunicationDeviceType
{
    Camera,
    Plc,
    LightSource
}

public sealed class DeviceConnectionConfiguration
{
    public string Name { get; set; }
    public CommunicationDeviceType DeviceType { get; set; }
    public string Protocol { get; set; }
    public string ConnectionMode { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public int Timeout { get; set; }
    public string SerialPort { get; set; }
    public int BaudRate { get; set; }
    public int DataBits { get; set; }
    public string Parity { get; set; }
    public string StopBits { get; set; }
    public int Station { get; set; }
    public bool Enabled { get; set; }
    public Dictionary<string, Dictionary<string, string>> ProtocolParameters
    {
        get;
        set;
    } = new Dictionary<string, Dictionary<string, string>>(
        StringComparer.OrdinalIgnoreCase);

    public string DeviceTypeText
    {
        get
        {
            switch (DeviceType)
            {
                case CommunicationDeviceType.Camera: return "相机";
                case CommunicationDeviceType.Plc: return "PLC";
                default: return "光源";
            }
        }
    }

    public string Endpoint
    {
        get
        {
            if (DeviceType == CommunicationDeviceType.Camera)
                return string.IsNullOrWhiteSpace(Host) ? "未配置" : Host;

            if (string.Equals(ConnectionMode, "网口", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(ConnectionMode, "TCP/IP", StringComparison.OrdinalIgnoreCase))
                return string.IsNullOrWhiteSpace(Host) ? "未配置" : Host + ":" + Port;

            return string.IsNullOrWhiteSpace(SerialPort)
                ? "未配置"
                : SerialPort + " / " + BaudRate;
        }
    }

    public string StateText => Enabled ? "已启用" : "已停用";
}

public sealed class PlcProtocolParameterDefinition
{
    internal PlcProtocolParameterDefinition(string name, string displayName,
        Type valueType, string defaultValue, string[] options)
    {
        Name = name;
        DisplayName = displayName;
        ValueType = valueType;
        DefaultValue = defaultValue ?? string.Empty;
        Options = options ?? new string[0];
    }

    public string Name { get; }
    public string DisplayName { get; }
    public Type ValueType { get; }
    public string DefaultValue { get; }
    public IReadOnlyList<string> Options { get; }
    public bool IsBoolean => ValueType == typeof(bool);
    public bool IsEnum => ValueType.IsEnum;
    public bool IsNumeric => IsNumericType(ValueType);

    private static bool IsNumericType(Type type)
    {
        Type valueType = Nullable.GetUnderlyingType(type) ?? type;
        return valueType == typeof(byte) || valueType == typeof(sbyte) ||
               valueType == typeof(short) || valueType == typeof(ushort) ||
               valueType == typeof(int) || valueType == typeof(uint) ||
               valueType == typeof(long) || valueType == typeof(ulong) ||
               valueType == typeof(float) || valueType == typeof(double) ||
               valueType == typeof(decimal);
    }
}

public partial class CommunicationFrmViewModel : ObservableObject
{
    private const int ConnectionMonitorIntervalMilliseconds = 1000;
    private static readonly string ConfigurationFilePathValue = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "communcation",
        "communication-config.json");
    private readonly object _runtimeRoot = new object();
    private Dictionary<string, PlcFrmVpCommunication> _runtimePlcs =
        new Dictionary<string, PlcFrmVpCommunication>(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, CameraFrmVpCommunication> _runtimeCameras =
        new Dictionary<string, CameraFrmVpCommunication>(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, LightSourceFrmVpCommunication> _runtimeLightSources =
        new Dictionary<string, LightSourceFrmVpCommunication>(StringComparer.OrdinalIgnoreCase);
    private CancellationTokenSource _connectionMonitorCancellation;
    private Thread _connectionMonitorThread;
    private static readonly HashSet<string> HiddenPlcPropertyNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "IpAddress", "Port", "ConnectionId", "SessionHandle", "SessionID",
            "OTConnectionId", "TOConnectionId", "PDULength", "CpuError",
            "CpuInfo", "LSCpuStatus", "LastPDUType", "OrderNumber",
            "ProductName", "ReceiveCacheLength", "ActualTimeout",
            "SendBeforeHex"
        };
    private static readonly Dictionary<string, string> PlcParameterDisplayNames =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["SiemensPLCS"] = "PLC 型号",
            ["Station"] = "站号",
            ["Slot"] = "槽号",
            ["Rack"] = "机架号",
            ["ConnectionType"] = "连接类型",
            ["LocalTSAP"] = "本地 TSAP",
            ["DestTSAP"] = "目标 TSAP",
            ["NetworkNumber"] = "网络号",
            ["PLCNumber"] = "PLC 编号",
            ["NetworkStationNumber"] = "网络站号",
            ["TargetIOStation"] = "目标 I/O 站号",
            ["UnitNumber"] = "单元号",
            ["BaseNo"] = "基板号",
            ["PlcType"] = "PLC 类型",
            ["Series"] = "PLC 系列",
            ["ConnectTimeOut"] = "连接超时(ms)",
            ["ReceiveTimeOut"] = "接收超时(ms)",
            ["SleepTime"] = "指令间隔(ms)",
            ["SocketKeepAliveTime"] = "保活时间(ms)",
            ["AddressStartWithZero"] = "地址从 0 开始",
            ["DataFormat"] = "数据格式",
            ["IsStringReverse"] = "字符串反转",
            ["EnableWriteMaskCode"] = "启用掩码写入",
            ["IsCheckMessageId"] = "校验消息 ID",
            ["StationCheckMatch"] = "校验站号",
            ["BroadcastStation"] = "广播站号",
            ["WordReadBatchLength"] = "单批读取字数",
            ["DisableFunctionCode06"] = "禁用功能码 06",
            ["ReadSplits"] = "单次读取长度",
            ["ReceiveUntilEmpty"] = "接收至缓存为空",
            ["UseAutoAmsNetID"] = "自动 AMS Net ID",
            ["AmsPort"] = "AMS 端口",
            ["UseTagCache"] = "启用标签缓存",
            ["Password"] = "密码",
            ["UserName"] = "用户名",
            ["Crc16CheckEnable"] = "启用 CRC16 校验",
            ["SumCheck"] = "启用和校验",
            ["RtsEnable"] = "启用 RTS",
            ["DataSwap"] = "交换数据字节序",
            ["IsClearCacheBeforeRead"] = "读取前清空缓存",
            ["ReceiveEmptyDataCount"] = "空数据结束次数",
            ["EnableWriteBitToWordRegister"] = "位写入字寄存器",
            ["ContextCheck"] = "校验上下文",
            ["ContextIdAutoIncrement"] = "上下文 ID 自增",
            ["ReadArrayUseSegment"] = "数组分段读取",
            ["UseHttps"] = "使用 HTTPS",
            ["Token"] = "访问令牌"
        };

    public CommunicationFrmViewModel()
    {
        DeviceConfigurations = new BindingList<DeviceConnectionConfiguration>();
        PlcProtocolNames = DiscoverPlcProtocolNames();
        LoadConfigurations();
    }

    public GlobalCommunicationModel GlobalCommunicationModel { get; set; }
    public BindingList<DeviceConnectionConfiguration> DeviceConfigurations { get; }
    public IReadOnlyList<string> PlcProtocolNames { get; }
    public string ConfigurationFilePath => ConfigurationFilePathValue;
    public string ConfigurationLoadError { get; private set; }
    public event Action ConfigurationsChanged;
    public event Action<string, CommunicationDeviceType, bool> DeviceConnectionStatusChanged;

    public bool IsPlcSerialProtocol(string protocol)
    {
        Type deviceType = ResolveDeviceType(protocol);
        return deviceType != null &&
               typeof(DeviceSerialPort).IsAssignableFrom(deviceType);
    }

    public IReadOnlyList<PlcProtocolParameterDefinition>
        GetPlcProtocolParameters(string protocol)
    {
        Type deviceType = ResolveDeviceType(protocol);
        if (deviceType == null) return new PlcProtocolParameterDefinition[0];

        var definitions = new List<PlcProtocolParameterDefinition>();
        AddConstructorParameterDefinitions(deviceType, definitions);

        object prototype = TryCreateDefaultDevice(deviceType);
        foreach (PropertyInfo property in deviceType.GetProperties(
                     BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.CanWrite || property.GetIndexParameters().Length != 0 ||
                HiddenPlcPropertyNames.Contains(property.Name) ||
                !IsSupportedParameterType(property.PropertyType))
                continue;

            object defaultValue = null;
            if (prototype != null && property.CanRead)
            {
                try { defaultValue = property.GetValue(prototype, null); }
                catch { }
            }
            definitions.Add(CreateParameterDefinition(property.Name,
                property.PropertyType, defaultValue));
        }

        return definitions
            .GroupBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(item => GetParameterOrder(item.Name))
            .ThenBy(item => item.DisplayName, StringComparer.CurrentCulture)
            .ToArray();
    }

    public PlcFrmVpCommunication ResolvePlc(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return null;
        lock (_runtimeRoot)
        {
            PlcFrmVpCommunication value;
            return _runtimePlcs.TryGetValue(key.Trim(), out value) ? value : null;
        }
    }

    public LightSourceFrmVpCommunication ResolveLightSource(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return null;
        lock (_runtimeRoot)
        {
            LightSourceFrmVpCommunication value;
            return _runtimeLightSources.TryGetValue(key.Trim(), out value) ? value : null;
        }
    }

    public CameraFrmVpCommunication ResolveCamera(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return null;
        lock (_runtimeRoot)
        {
            CameraFrmVpCommunication value;
            return _runtimeCameras.TryGetValue(key.Trim(), out value) ? value : null;
        }
    }

    public bool IsDeviceConnected(string key, CommunicationDeviceType deviceType)
    {
        if (string.IsNullOrWhiteSpace(key)) return false;
        string normalizedKey = key.Trim();
        lock (_runtimeRoot)
        {
            switch (deviceType)
            {
                case CommunicationDeviceType.Plc:
                    return _runtimePlcs.TryGetValue(normalizedKey, out var plc) &&
                           plc.ConnectSuccess;
                case CommunicationDeviceType.Camera:
                    return _runtimeCameras.TryGetValue(normalizedKey, out var camera) &&
                           camera.ConnectSuccess;
                default:
                    return _runtimeLightSources.TryGetValue(normalizedKey, out var light) &&
                           light.ConnectSuccess;
            }
        }
    }

    public void BuildRuntimeCommunications()
    {
        var plcs = new Dictionary<string, PlcFrmVpCommunication>(
            StringComparer.OrdinalIgnoreCase);
        var cameras = new Dictionary<string, CameraFrmVpCommunication>(
            StringComparer.OrdinalIgnoreCase);
        var lights = new Dictionary<string, LightSourceFrmVpCommunication>(
            StringComparer.OrdinalIgnoreCase);

        foreach (DeviceConnectionConfiguration configuration in DeviceConfigurations
                     .Where(item => item != null && item.Enabled &&
                                    !string.IsNullOrWhiteSpace(item.Name)))
        {
            try
            {
                if (configuration.DeviceType == CommunicationDeviceType.Plc)
                {
                    PlcFrmVpCommunication plc = CreatePlc(configuration);
                    string key = configuration.Name.Trim();
                    plc.ConnectStatusChanged += connected =>
                        NotifyConnectionStatus(key, CommunicationDeviceType.Plc, connected);
                    plcs[key] = plc;
                    LogConnection(configuration.Name, plc.Connect());
                    NotifyConnectionStatus(key, CommunicationDeviceType.Plc,
                        plc.ConnectSuccess);
                }
                else if (configuration.DeviceType == CommunicationDeviceType.Camera)
                {
                    string key = configuration.Name.Trim();
                    var camera = new CameraFrmVpCommunication(
                        configuration.Host, configuration.Timeout);
                    camera.ConnectStatusChanged += connected =>
                        NotifyConnectionStatus(key, CommunicationDeviceType.Camera, connected);
                    cameras[key] = camera;
                    LogConnection(configuration.Name, camera.Connect());
                    NotifyConnectionStatus(key, CommunicationDeviceType.Camera,
                        camera.ConnectSuccess);
                }
                else if (configuration.DeviceType == CommunicationDeviceType.LightSource)
                {
                    LightSourceFrmVpCommunication light = CreateLightSource(configuration);
                    string key = configuration.Name.Trim();
                    light.ConnectStatusChanged += connected =>
                        NotifyConnectionStatus(key, CommunicationDeviceType.LightSource, connected);
                    lights[key] = light;
                    LogConnection(configuration.Name, light.Connect());
                    NotifyConnectionStatus(key, CommunicationDeviceType.LightSource,
                        light.ConnectSuccess);
                }
            }
            catch (Exception ex)
            {
                NotifyConnectionStatus(configuration.Name.Trim(), configuration.DeviceType, false);
                AppLog.Error("通讯实例创建失败：" + configuration.Name +
                    Environment.NewLine + ex, nameof(CommunicationFrmViewModel));
            }
        }

        Dictionary<string, PlcFrmVpCommunication> oldPlcs;
        Dictionary<string, CameraFrmVpCommunication> oldCameras;
        Dictionary<string, LightSourceFrmVpCommunication> oldLights;
        StopConnectionMonitor();
        lock (_runtimeRoot)
        {
            oldPlcs = _runtimePlcs;
            oldCameras = _runtimeCameras;
            oldLights = _runtimeLightSources;
            _runtimePlcs = plcs;
            _runtimeCameras = cameras;
            _runtimeLightSources = lights;
            GlobalCommunicationModel = new GlobalCommunicationModel(
                new ObservableCollection<PlcFrmVpCommunication>(plcs.Values),
                new ObservableCollection<CameraFrmVpCommunication>(cameras.Values),
                new ObservableCollection<LightSourceFrmVpCommunication>(lights.Values));
        }

        DisposeCommunications(oldPlcs.Values);
        DisposeCommunications(oldCameras.Values);
        DisposeCommunications(oldLights.Values);
        StartConnectionMonitor(cameras, lights);
    }

    public void DisposeRuntimeCommunications()
    {
        StopConnectionMonitor();
        Dictionary<string, PlcFrmVpCommunication> plcs;
        Dictionary<string, CameraFrmVpCommunication> cameras;
        Dictionary<string, LightSourceFrmVpCommunication> lights;
        lock (_runtimeRoot)
        {
            plcs = _runtimePlcs;
            cameras = _runtimeCameras;
            lights = _runtimeLightSources;
            _runtimePlcs = new Dictionary<string, PlcFrmVpCommunication>(
                StringComparer.OrdinalIgnoreCase);
            _runtimeCameras = new Dictionary<string, CameraFrmVpCommunication>(
                StringComparer.OrdinalIgnoreCase);
            _runtimeLightSources = new Dictionary<string, LightSourceFrmVpCommunication>(
                StringComparer.OrdinalIgnoreCase);
            GlobalCommunicationModel = null;
        }

        DisposeCommunications(plcs.Values);
        DisposeCommunications(cameras.Values);
        DisposeCommunications(lights.Values);
    }

    private void StartConnectionMonitor(
        Dictionary<string, CameraFrmVpCommunication> cameras,
        Dictionary<string, LightSourceFrmVpCommunication> lights)
    {
        if (cameras.Count == 0 && lights.Count == 0) return;

        var cancellation = new CancellationTokenSource();
        var thread = new Thread(() => MonitorConnections(cameras, lights,
            cancellation.Token))
        {
            IsBackground = true,
            Name = "CommunicationConnectionMonitor"
        };

        lock (_runtimeRoot)
        {
            _connectionMonitorCancellation = cancellation;
            _connectionMonitorThread = thread;
        }

        thread.Start();
    }

    private void StopConnectionMonitor()
    {
        CancellationTokenSource cancellation;
        Thread thread;
        lock (_runtimeRoot)
        {
            cancellation = _connectionMonitorCancellation;
            thread = _connectionMonitorThread;
            _connectionMonitorCancellation = null;
            _connectionMonitorThread = null;
        }

        if (cancellation == null) return;
        cancellation.Cancel();
        if (thread != null && thread != Thread.CurrentThread && thread.IsAlive)
            thread.Join(1500);
        cancellation.Dispose();
    }

    private void MonitorConnections(
        Dictionary<string, CameraFrmVpCommunication> cameras,
        Dictionary<string, LightSourceFrmVpCommunication> lights,
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.WaitHandle.WaitOne(
                   ConnectionMonitorIntervalMilliseconds))
        {
            foreach (CameraFrmVpCommunication camera in cameras.Values)
            {
                if (cancellationToken.IsCancellationRequested) return;
                try
                {
                    if (camera.ConnectSuccess)
                        camera.KeepAlive();
                    else
                        camera.Connect();
                }
                catch (Exception ex)
                {
                    AppLog.Error("相机连接检测失败" + Environment.NewLine + ex,
                        nameof(CommunicationFrmViewModel));
                }
            }

            foreach (LightSourceFrmVpCommunication light in lights.Values)
            {
                if (cancellationToken.IsCancellationRequested) return;
                try
                {
                    if (light.ConnectSuccess)
                        light.KeepAlive();
                    else
                        light.Connect();
                }
                catch (Exception ex)
                {
                    AppLog.Error("光源连接检测失败" + Environment.NewLine + ex,
                        nameof(CommunicationFrmViewModel));
                }
            }
        }
    }

    public void SaveConfigurations()
    {
        DeviceConfigurations.ToList().WriteJson(ConfigurationFilePathValue);
        ConfigurationsChanged?.Invoke();
    }

    private void NotifyConnectionStatus(string key, CommunicationDeviceType deviceType,
        bool connected)
    {
        DeviceConnectionStatusChanged?.Invoke(key, deviceType, connected);
    }

    public DeviceConnectionConfiguration AddConfiguration(CommunicationDeviceType deviceType)
    {
        var sequence = DeviceConfigurations.Count(item => item.DeviceType == deviceType) + 1;
        var configuration = new DeviceConnectionConfiguration
        {
            DeviceType = deviceType,
            Name = GetDefaultName(deviceType) + sequence,
            Protocol = deviceType == CommunicationDeviceType.Camera ? "ICMP Ping" :
                deviceType == CommunicationDeviceType.LightSource ? "网口" :
                PlcProtocolNames.FirstOrDefault() ?? string.Empty,
            ConnectionMode = deviceType == CommunicationDeviceType.LightSource ? "网口" :
                deviceType == CommunicationDeviceType.Plc ? "网口" : "Ping",
            Host = "192.168.1.10",
            Port = deviceType == CommunicationDeviceType.LightSource ? 5000 :
                deviceType == CommunicationDeviceType.Plc ? 102 : 0,
            Timeout = 500,
            SerialPort = "COM1",
            BaudRate = 9600,
            DataBits = 8,
            Parity = "None",
            StopBits = "One",
            Station = 1,
            Enabled = true
        };
        DeviceConfigurations.Add(configuration);
        return configuration;
    }

    private void LoadConfigurations()
    {
        try
        {
            var configurations = ConfigurationFilePathValue
                .ReadJson<List<DeviceConnectionConfiguration>>();
            if (configurations == null) return;

            foreach (var configuration in configurations.Where(item => item != null))
            {
                if (configuration.ProtocolParameters == null)
                    configuration.ProtocolParameters =
                        new Dictionary<string, Dictionary<string, string>>(
                            StringComparer.OrdinalIgnoreCase);
                DeviceConfigurations.Add(configuration);
            }
        }
        catch (Exception ex)
        {
            ConfigurationLoadError = ex.Message;
        }
    }

    private static string GetDefaultName(CommunicationDeviceType deviceType)
    {
        switch (deviceType)
        {
            case CommunicationDeviceType.Camera: return "相机 ";
            case CommunicationDeviceType.Plc: return "PLC ";
            default: return "光源 ";
        }
    }

    private static PlcFrmVpCommunication CreatePlc(DeviceConnectionConfiguration configuration)
    {
        Type deviceType = ResolveDeviceType(configuration.Protocol);
        if (deviceType == null || !typeof(DeviceCommunication).IsAssignableFrom(deviceType))
            throw new InvalidOperationException("未找到 PLC 协议类型：" + configuration.Protocol);

        DeviceCommunication device = CreateDevice(deviceType, configuration);
        ConfigureDevice(device, configuration);
        return new PlcFrmVpCommunication(device);
    }

    private static LightSourceFrmVpCommunication CreateLightSource(
        DeviceConnectionConfiguration configuration)
    {
        ILightSourceService service;
        bool serial = string.Equals(configuration.ConnectionMode, "串口",
            StringComparison.OrdinalIgnoreCase);
        if (serial)
        {
            service = new RsLightSourceImpl(configuration.SerialPort, configuration.BaudRate,
                ParseParity(configuration.Parity), configuration.DataBits,
                ParseStopBits(configuration.StopBits));
        }
        else
        {
            service = new TcpLightSourceImpl(configuration.Host, configuration.Port);
        }

        return new LightSourceFrmVpCommunication(service);
    }

    private static Type ResolveDeviceType(string protocol)
    {
        if (string.IsNullOrWhiteSpace(protocol)) return null;
        Assembly assembly = typeof(DeviceCommunication).Assembly;
        string[] names =
        {
            protocol,
            "HslCommunication.Profinet." + protocol,
            "HslCommunication.ModBus." + protocol
        };
        foreach (string name in names)
        {
            Type type = assembly.GetType(name, false, true);
            if (type != null) return type;
        }

        return assembly.GetTypes().FirstOrDefault(type =>
            typeof(DeviceCommunication).IsAssignableFrom(type) &&
            string.Equals(FormatProtocolName(type), protocol,
                StringComparison.OrdinalIgnoreCase));
    }

    private static DeviceCommunication CreateDevice(Type type,
        DeviceConnectionConfiguration configuration)
    {
        foreach (ConstructorInfo constructor in type.GetConstructors()
                     .OrderBy(item => item.GetParameters().Length))
        {
            ParameterInfo[] parameters = constructor.GetParameters();
            object[] arguments = new object[parameters.Length];
            bool supported = true;
            for (int i = 0; i < parameters.Length; i++)
            {
                object argument;
                if (!TryCreateArgument(parameters[i], configuration, out argument))
                {
                    supported = false;
                    break;
                }
                arguments[i] = argument;
            }

            if (!supported) continue;
            try
            {
                return (DeviceCommunication)constructor.Invoke(arguments);
            }
            catch
            {
                // Try the next compatible constructor.
            }
        }

        throw new InvalidOperationException("无法创建 PLC 协议实例：" + type.FullName);
    }

    private static bool TryCreateArgument(ParameterInfo parameter,
        DeviceConnectionConfiguration configuration, out object value)
    {
        Type type = parameter.ParameterType;
        string name = (parameter.Name ?? string.Empty).ToLowerInvariant();
        string configuredValue;
        if (TryGetProtocolParameter(configuration, parameter.Name,
                out configuredValue) ||
            type.IsEnum && TryGetProtocolParameter(configuration, type.Name,
                out configuredValue))
        {
            return TryConvertParameterValue(configuredValue, type, out value);
        }
        if (type == typeof(string))
        {
            value = name.Contains("port") && !name.Contains("ip")
                ? configuration.SerialPort : configuration.Host;
            return true;
        }
        if (type == typeof(int))
        {
            value = name.Contains("port") ? configuration.Port :
                name.Contains("timeout") ? configuration.Timeout :
                name.Contains("baud") ? configuration.BaudRate :
                name.Contains("databit") ? configuration.DataBits : configuration.Station;
            return true;
        }
        if (type == typeof(byte) || type == typeof(ushort) || type == typeof(short))
        {
            value = Convert.ChangeType(configuration.Station, type);
            return true;
        }
        if (type.IsEnum)
        {
            Array values = Enum.GetValues(type);
            value = values.Length == 0 ? null : values.GetValue(0);
            return values.Length > 0;
        }
        if (type == typeof(bool)) { value = false; return true; }
        if (parameter.IsOptional) { value = parameter.DefaultValue; return true; }
        value = null;
        return type.IsClass;
    }

    private static void ConfigureDevice(DeviceCommunication device,
        DeviceConnectionConfiguration configuration)
    {
        SetProperty(device, "IpAddress", configuration.Host);
        SetProperty(device, "Port", configuration.Port);
        SetProperty(device, "ConnectTimeOut", configuration.Timeout);
        SetProperty(device, "ReceiveTimeOut", configuration.Timeout);
        SetProperty(device, "Station", (byte)Math.Max(0, configuration.Station));

        Dictionary<string, string> parameters = GetProtocolParameterValues(
            configuration, false);
        if (parameters != null)
        {
            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                PropertyInfo property = device.GetType().GetProperty(parameter.Key,
                    BindingFlags.Instance | BindingFlags.Public |
                    BindingFlags.IgnoreCase);
                if (property == null || !property.CanWrite ||
                    HiddenPlcPropertyNames.Contains(property.Name))
                    continue;

                object converted;
                if (!TryConvertParameterValue(parameter.Value,
                        property.PropertyType, out converted))
                    throw new InvalidOperationException("PLC 参数“" +
                        GetParameterDisplayName(property.Name) + "”的值无效：" +
                        parameter.Value);
                property.SetValue(device, converted, null);
            }
        }

        DeviceSerialPort serial = device as DeviceSerialPort;
        if (serial != null)
        {
            serial.SerialPortInni(configuration.SerialPort, configuration.BaudRate,
                configuration.DataBits, ParseStopBits(configuration.StopBits),
                ParseParity(configuration.Parity));
        }
    }

    private static void SetProperty(object target, string name, object value)
    {
        PropertyInfo property = target.GetType().GetProperty(name,
            BindingFlags.Instance | BindingFlags.Public);
        if (property == null || !property.CanWrite) return;
        try { property.SetValue(target, Convert.ChangeType(value, property.PropertyType), null); }
        catch { }
    }

    private static Dictionary<string, string> GetProtocolParameterValues(
        DeviceConnectionConfiguration configuration, bool create)
    {
        if (configuration == null || string.IsNullOrWhiteSpace(
                configuration.Protocol)) return null;
        if (configuration.ProtocolParameters == null)
        {
            if (!create) return null;
            configuration.ProtocolParameters =
                new Dictionary<string, Dictionary<string, string>>(
                    StringComparer.OrdinalIgnoreCase);
        }

        Dictionary<string, string> values;
        if (configuration.ProtocolParameters.TryGetValue(
                configuration.Protocol, out values)) return values;
        if (!create) return null;
        values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        configuration.ProtocolParameters[configuration.Protocol] = values;
        return values;
    }

    private static bool TryGetProtocolParameter(
        DeviceConnectionConfiguration configuration, string name,
        out string value)
    {
        value = null;
        Dictionary<string, string> values = GetProtocolParameterValues(
            configuration, false);
        return values != null && !string.IsNullOrWhiteSpace(name) &&
               values.TryGetValue(name, out value);
    }

    private static bool TryConvertParameterValue(string text, Type targetType,
        out object value)
    {
        Type type = Nullable.GetUnderlyingType(targetType) ?? targetType;
        try
        {
            if (type == typeof(string)) value = text ?? string.Empty;
            else if (type == typeof(bool)) value = bool.Parse(text);
            else if (type.IsEnum) value = Enum.Parse(type, text, true);
            else value = Convert.ChangeType(text, type,
                CultureInfo.InvariantCulture);
            return true;
        }
        catch
        {
            value = null;
            return false;
        }
    }

    private static void AddConstructorParameterDefinitions(Type deviceType,
        ICollection<PlcProtocolParameterDefinition> definitions)
    {
        ConstructorInfo constructor = deviceType.GetConstructors()
            .OrderBy(item => item.GetParameters().Length)
            .FirstOrDefault();
        if (constructor == null) return;
        foreach (ParameterInfo parameter in constructor.GetParameters())
        {
            if (!parameter.ParameterType.IsEnum) continue;
            definitions.Add(CreateParameterDefinition(parameter.ParameterType.Name,
                parameter.ParameterType, parameter.HasDefaultValue
                    ? parameter.DefaultValue : Enum.GetValues(
                        parameter.ParameterType).GetValue(0)));
        }
    }

    private static DeviceCommunication TryCreateDefaultDevice(Type deviceType)
    {
        var configuration = new DeviceConnectionConfiguration
        {
            Protocol = FormatProtocolName(deviceType),
            Host = "127.0.0.1",
            Port = 102,
            Timeout = 500,
            SerialPort = "COM1",
            BaudRate = 9600,
            DataBits = 8,
            Parity = "None",
            StopBits = "One",
            Station = 1
        };
        try { return CreateDevice(deviceType, configuration); }
        catch { return null; }
    }

    private static PlcProtocolParameterDefinition CreateParameterDefinition(
        string name, Type type, object defaultValue)
    {
        string[] options = type.IsEnum ? Enum.GetNames(type) : new string[0];
        string text = defaultValue == null
            ? type == typeof(bool) ? bool.FalseString : string.Empty
            : Convert.ToString(defaultValue, CultureInfo.InvariantCulture) ??
              string.Empty;
        return new PlcProtocolParameterDefinition(name,
            GetParameterDisplayName(name), type, text, options);
    }

    private static bool IsSupportedParameterType(Type type)
    {
        Type valueType = Nullable.GetUnderlyingType(type) ?? type;
        return valueType == typeof(string) || valueType == typeof(bool) ||
               valueType.IsEnum || valueType == typeof(byte) ||
               valueType == typeof(sbyte) || valueType == typeof(short) ||
               valueType == typeof(ushort) || valueType == typeof(int) ||
               valueType == typeof(uint) || valueType == typeof(long) ||
               valueType == typeof(ulong) || valueType == typeof(float) ||
               valueType == typeof(double) || valueType == typeof(decimal);
    }

    private static string GetParameterDisplayName(string name)
    {
        string displayName;
        return PlcParameterDisplayNames.TryGetValue(name, out displayName)
            ? displayName
            : name;
    }

    private static int GetParameterOrder(string name)
    {
        switch (name)
        {
            case "SiemensPLCS": return 0;
            case "Station": return 1;
            case "Rack": return 2;
            case "Slot": return 3;
            case "ConnectTimeOut": return 900;
            case "ReceiveTimeOut": return 901;
            case "SleepTime": return 902;
            case "SocketKeepAliveTime": return 903;
            default: return 100;
        }
    }

    private static Parity ParseParity(string value)
    {
        Parity parity;
        return Enum.TryParse(value, true, out parity) ? parity : Parity.None;
    }

    private static StopBits ParseStopBits(string value)
    {
        StopBits stopBits;
        return Enum.TryParse(value, true, out stopBits) ? stopBits : StopBits.One;
    }

    private static void LogConnection(string name, FrmMapper.Data.Result result)
    {
        if (result == null || !result.IsSuccess)
            AppLog.Error("通讯连接失败：" + name + "，" + (result == null ? "未知错误" : result.Message),
                nameof(CommunicationFrmViewModel));
        else
            AppLog.Info("通讯连接成功：" + name, nameof(CommunicationFrmViewModel));
    }

    private static void DisposeCommunications<T>(IEnumerable<T> values)
    {
        foreach (T value in values)
        {
            IDisposable disposable = value as IDisposable;
            if (disposable == null) continue;
            try { disposable.Dispose(); }
            catch (Exception ex) { AppLog.Error("释放通讯实例失败" + Environment.NewLine + ex); }
        }
    }

    private static IReadOnlyList<string> DiscoverPlcProtocolNames()
    {
        IEnumerable<Type> types;
        try
        {
            types = typeof(DeviceCommunication).Assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            types = ex.Types.Where(type => type != null);
        }

        return types
            .Where(type => type.IsPublic && !type.IsAbstract &&
                           typeof(DeviceCommunication).IsAssignableFrom(type) &&
                           IsPlcNamespace(type.Namespace) &&
                           type.Name.IndexOf("Server", StringComparison.OrdinalIgnoreCase) < 0)
            .Select(type => FormatProtocolName(type))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static bool IsPlcNamespace(string value)
    {
        return value != null &&
               (value.StartsWith("HslCommunication.Profinet", StringComparison.Ordinal) ||
                value.StartsWith("HslCommunication.ModBus", StringComparison.Ordinal));
    }

    private static string FormatProtocolName(Type type)
    {
        return type.FullName
            .Replace("HslCommunication.Profinet.", string.Empty)
            .Replace("HslCommunication.ModBus.", "ModBus.");
    }
}
