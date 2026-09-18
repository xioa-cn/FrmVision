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
using FrmCommon.LogServices;
using FrmMapper.Data;
using FrmServices.LogServices;
using FrmServices.Services.CommunicationServices.Services;

namespace FrmServices.Services.CommunicationServices;

// 作者：xioa
// 作者邮箱：1327916255@qq.com
public class ContentUdpClient : IContentServer
{
    private readonly object _syncRoot = new object();
    private const int MaximumPendingMessages = 1024;
    private readonly Queue<string> _receivedMessages = new Queue<string>();
    private long _droppedMessages;
    private readonly Encoding _encoding;
    private UdpClient _client;
    private bool _disposed;

    public ContentUdpClient(string host = "127.0.0.1", int port = 5000, Encoding encoding = null)
    {
        if (string.IsNullOrWhiteSpace(host)) throw new ArgumentException("A remote host is required.", nameof(host));
        if (port <= IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            throw new ArgumentOutOfRangeException(nameof(port));
        Host = host.Trim();
        Port = port;
        _encoding = (Encoding)(encoding ?? Encoding.UTF8).Clone();
    }

    public string Host { get; }
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


    public int PendingMessageCount
    {
        get { lock (_syncRoot) return _receivedMessages.Count; }
    }

    // Consumes one decoded UDP datagram. Each datagram is one application chunk.
    public string ReadMessage(int timeoutMilliseconds, CancellationToken cancellationToken)
    {
        CommunicationAccessGuard.EnsureAllowed();
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
                    if (_disposed) throw new ObjectDisposedException(nameof(ContentUdpClient));
                    if (client == null || !ReferenceEquals(client, _client))
                        throw new InvalidOperationException("UDP 客户端未启动或已停止，请先启动内置通讯。");
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
        if (!CommunicationAccessGuard.IsAllowed) return Result.Fail(CommunicationAccessGuard.FailureMessage);
        lock (_syncRoot)
        {
            if (_disposed) return Result.Fail("The UDP client has been disposed.");
            if (_client != null) return Result.Ok();

            UdpClient client = null;
            try
            {
                var addresses = Dns.GetHostAddresses(Host);
                var address = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)
                    ?? addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetworkV6);
                if (address == null) throw new SocketException((int)SocketError.HostNotFound);
                client = new UdpClient(address.AddressFamily);
                client.Client.Bind(new IPEndPoint(address.AddressFamily == AddressFamily.InterNetwork
                    ? IPAddress.Any : IPAddress.IPv6Any, 0));
                // UDP Connect selects a peer; it does not establish or verify a remote connection.
                client.Connect(new IPEndPoint(address, Port));
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
                return Result.Fail("Failed to start the UDP client: " + ex.Message);
            }
        }
    }

    public Task<Result> StartAsync() => Task.Run(() => Start());

    // Sends one datagram to the configured peer, without waiting for an incoming packet.
    public async Task Send(string msg)
    {
        CommunicationAccessGuard.EnsureAllowed();
        if (msg == null) throw new ArgumentNullException(nameof(msg));
        UdpClient client;
        lock (_syncRoot)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ContentUdpClient));
            if (_client == null) throw new InvalidOperationException("The UDP client is not started.");
            client = _client;
        }

        var bytes = _encoding.GetBytes(msg);
        await client.SendAsync(bytes, bytes.Length).ConfigureAwait(false);
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
                    throw;
                }

                lock (_syncRoot)
                {
                    if (!ReferenceEquals(_client, client)) return;
                }

                var bytes = result.Buffer ?? Array.Empty<byte>();
                OnReceiveBytes(client, bytes);
                var decoder = _encoding.GetDecoder();
                var chars = new char[_encoding.GetMaxCharCount(bytes.Length)];
                int charCount = decoder.GetChars(bytes, 0, bytes.Length, chars, 0, true);
                OnReceive(client, new string(chars, 0, charCount));
            }
        }
        catch (Exception ex)
        {
            lock (_syncRoot)
            {
                if (!ReferenceEquals(_client, client)) return;
                StopCore();
            }
            AppLog.Error("UDP receive failed: " + ex, nameof(ContentUdpClient));
        }
    }

    private void OnReceive(UdpClient client, string message)
    {
        lock (_syncRoot)
        {
            if (!ReferenceEquals(_client, client)) return;
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

    private void OnReceiveBytes(UdpClient client, byte[] bytes)
    {
        lock (_syncRoot)
        {
            if (!ReferenceEquals(_client, client)) return;
        }
        RaiseReceiveBytes(ReceiveBytes, bytes);
    }

    private void StopCore()
    {
        var client = _client;
        _client = null;
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
                AppLog.Error("UDP receive handler failed: " + ex, nameof(ContentUdpClient));
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
                AppLog.Error("UDP byte receive handler failed: " + ex, nameof(ContentUdpClient));
            }
        }
    }
}
