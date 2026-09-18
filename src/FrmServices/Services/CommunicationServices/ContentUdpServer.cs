using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FrmMapper.Data;
using FrmServices.LogServices;
using FrmServices.Services.CommunicationServices.Services;

namespace FrmServices.Services.CommunicationServices;

// 作者：xioa
// 作者邮箱：1327916255@qq.com
public class ContentUdpServer : IContentServer
{
    private readonly object _syncRoot = new object();
    private readonly HashSet<IPEndPoint> _remotes = new HashSet<IPEndPoint>();
    private const int MaximumPendingMessages = 1024;
    private readonly Queue<string> _receivedMessages = new Queue<string>();
    private long _droppedMessages;
    private readonly IPAddress _address;
    private readonly Encoding _encoding;
    private UdpClient _client;
    private bool _disposed;

    public ContentUdpServer(int port = 5000, string ipAddress = "0.0.0.0", Encoding encoding = null)
    {
        if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            throw new ArgumentOutOfRangeException(nameof(port));
        if (!IPAddress.TryParse(ipAddress, out _address))
            throw new ArgumentException("A local IP address is required.", nameof(ipAddress));

        Port = port;
        _encoding = (Encoding)(encoding ?? Encoding.UTF8).Clone();
    }

    public int Port { get; }
    public event Action<string> Receive;
    public event Action<byte[]> ReceiveBytes;

    public bool IsStarted
    {
        get { lock (_syncRoot) return _client != null; }
    }

    public IPEndPoint LocalEndPoint
    {
        get { lock (_syncRoot) return _client == null ? null : (IPEndPoint)_client.Client.LocalEndPoint; }
    }

    public int RemoteCount
    {
        get { lock (_syncRoot) return _remotes.Count; }
    }

    public int PendingMessageCount
    {
        get { lock (_syncRoot) return _receivedMessages.Count; }
    }

    // Consumes one decoded UDP datagram. Each datagram is one application chunk.
    public string ReadMessage(int timeoutMilliseconds, CancellationToken cancellationToken)
    {
        if (timeoutMilliseconds < 0) throw new ArgumentOutOfRangeException(nameof(timeoutMilliseconds));
        cancellationToken.ThrowIfCancellationRequested();
        var elapsed = Stopwatch.StartNew();
        using (cancellationToken.Register(() => { lock (_syncRoot) Monitor.PulseAll(_syncRoot); }))
        {
            lock (_syncRoot)
            {
                var client = _client;
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (_disposed) throw new ObjectDisposedException(nameof(ContentUdpServer));
                    if (client == null || !ReferenceEquals(client, _client))
                        throw new InvalidOperationException("UDP 服务端未启动或已停止，请先启动内置通讯。");
                    if (_droppedMessages > 0)
                    {
                        long dropped = _droppedMessages;
                        _droppedMessages = 0;
                        throw new IOException("UDP 接收缓存已满，丢弃了 " + dropped + " 个数据块，请提高流程读取速度。");
                    }
                    if (_receivedMessages.Count > 0) return _receivedMessages.Dequeue();
                    int remaining = timeoutMilliseconds == 0 ? Timeout.Infinite
                        : (int)Math.Max(0, timeoutMilliseconds - elapsed.ElapsedMilliseconds);
                    if (remaining == 0) throw new TimeoutException("等待 UDP 接收数据超时（" + timeoutMilliseconds + " ms）。");
                    Monitor.Wait(_syncRoot, remaining);
                }
            }
        }
    }

    public Result Start()
    {
        lock (_syncRoot)
        {
            if (_disposed) return Result.Fail("The UDP server has been disposed.");
            if (_client != null) return Result.Ok();

            UdpClient client = null;
            try
            {
                client = new UdpClient(new IPEndPoint(_address, Port));
                client.Client.ReceiveBufferSize = 65535;
                client.Client.SendBufferSize = 65535;
                _client = client;
                _ = Task.Run(() => ReceiveLoopAsync(client));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                client?.Close();
                _client = null;
                return Result.Fail("Failed to start the UDP server: " + ex.Message);
            }
        }
    }

    public Task<Result> StartAsync() => Task.FromResult(Start());

    // Sends the same datagram to every remote that has sent data since this listen session started.
    public async Task Send(string msg)
    {
        if (msg == null) throw new ArgumentNullException(nameof(msg));
        UdpClient client;
        IPEndPoint[] remotes;
        lock (_syncRoot)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ContentUdpServer));
            if (_client == null) throw new InvalidOperationException("The UDP server is not started.");
            client = _client;
            remotes = _remotes.ToArray();
        }

        if (remotes.Length == 0) throw new InvalidOperationException("No UDP remotes have sent data yet.");
        var bytes = _encoding.GetBytes(msg);
        await Task.WhenAll(remotes.Select(remote => client.SendAsync(bytes, bytes.Length, remote))).ConfigureAwait(false);
    }

    public void Stop()
    {
        lock (_syncRoot) StopCore();
    }

    public void Dispose()
    {
        lock (_syncRoot)
        {
            if (_disposed) return;
            _disposed = true;
            StopCore();
        }
    }

    private async Task ReceiveLoopAsync(UdpClient client)
    {
        try
        {
            while (true)
            {
                UdpReceiveResult result;
                try { result = await client.ReceiveAsync().ConfigureAwait(false); }
                catch (ObjectDisposedException) { return; }
                catch (SocketException)
                {
                    lock (_syncRoot)
                    {
                        if (!ReferenceEquals(_client, client)) return;
                    }
                    return;
                }

                lock (_syncRoot)
                {
                    if (!ReferenceEquals(_client, client)) return;
                    if (result.RemoteEndPoint != null)
                        _remotes.Add(new IPEndPoint(result.RemoteEndPoint.Address, result.RemoteEndPoint.Port));
                }

                var bytes = result.Buffer ?? Array.Empty<byte>();
                if (bytes.Length == 0) continue;
                OnReceiveBytes(bytes);
                var decoder = _encoding.GetDecoder();
                var chars = new char[_encoding.GetMaxCharCount(bytes.Length)];
                int charCount = decoder.GetChars(bytes, 0, bytes.Length, chars, 0, true);
                if (charCount > 0) OnReceive(new string(chars, 0, charCount));
            }
        }
        catch (Exception ex)
        {
            lock (_syncRoot)
            {
                if (!ReferenceEquals(_client, client)) return;
                StopCore();
            }
            AppLog.Error("UDP receive failed: " + ex, nameof(ContentUdpServer));
        }
    }

    private void OnReceive(string message)
    {
        lock (_syncRoot)
        {
            if (_client == null) return;
            if (_receivedMessages.Count >= MaximumPendingMessages)
            {
                _receivedMessages.Dequeue();
                _droppedMessages++;
            }
            _receivedMessages.Enqueue(message);
            Monitor.PulseAll(_syncRoot);
        }
        RaiseReceive(Receive, message);
    }

    private void OnReceiveBytes(byte[] bytes)
    {
        lock (_syncRoot)
        {
            if (_client == null) return;
        }
        RaiseReceiveBytes(ReceiveBytes, bytes);
    }

    private void StopCore()
    {
        var client = _client;
        _client = null;
        _remotes.Clear();
        _receivedMessages.Clear();
        _droppedMessages = 0;
        Monitor.PulseAll(_syncRoot);
        try { client?.Close(); }
        catch { }
    }

    private static void RaiseReceive(Action<string> handlers, string message)
    {
        if (handlers == null) return;
        foreach (Action<string> handler in handlers.GetInvocationList())
        {
            try { handler(message); }
            catch (Exception ex)
            {
                AppLog.Error("UDP receive handler failed: " + ex, nameof(ContentUdpServer));
            }
        }
    }

    private static void RaiseReceiveBytes(Action<byte[]> handlers, byte[] bytes)
    {
        if (handlers == null) return;
        foreach (Action<byte[]> handler in handlers.GetInvocationList())
        {
            try { handler((byte[])bytes.Clone()); }
            catch (Exception ex)
            {
                AppLog.Error("UDP byte receive handler failed: " + ex, nameof(ContentUdpServer));
            }
        }
    }
}
