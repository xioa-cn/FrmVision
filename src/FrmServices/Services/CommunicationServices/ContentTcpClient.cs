using System;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FrmMapper.Data;
using FrmServices.Services.CommunicationServices.Services;

namespace FrmServices.Services.CommunicationServices;

public class ContentTcpClient : IContentServer
{
    private readonly object _syncRoot = new object();
    private readonly SemaphoreSlim _startLock = new SemaphoreSlim(1, 1);
    private readonly Encoding _encoding;
    private const int MaximumPendingMessages = 1024;
    private readonly Queue<string> _receivedMessages = new Queue<string>();
    private long _droppedMessages;
    private TcpClient _connectingClient;
    private TcpTextConnection _connection;
    private TaskCompletionSource<bool> _session;
    private Task<Result> _firstConnect;
    private bool _disposed;

    public ContentTcpClient(string host = "127.0.0.1", int port = 5000,
        Encoding encoding = null, int connectTimeout = 5000, bool autoReconnect = true, int reconnectInterval = 2000)
    {
        if (string.IsNullOrWhiteSpace(host)) throw new ArgumentException("Host is required.", nameof(host));
        if (port <= IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            throw new ArgumentOutOfRangeException(nameof(port));
        if (connectTimeout <= 0) throw new ArgumentOutOfRangeException(nameof(connectTimeout));
        if (reconnectInterval <= 0) throw new ArgumentOutOfRangeException(nameof(reconnectInterval));

        Host = host.Trim();
        Port = port;
        ConnectTimeout = connectTimeout;
        AutoReconnect = autoReconnect;
        ReconnectInterval = reconnectInterval;
        _encoding = (Encoding)(encoding ?? Encoding.UTF8).Clone();
    }

    public string Host { get; }
    public int Port { get; }
    public int ConnectTimeout { get; }
    public bool AutoReconnect { get; }
    public int ReconnectInterval { get; }
    public bool IsRunning
    {
        get { lock (_syncRoot) return _session != null && (AutoReconnect || _connection?.IsConnected == true); }
    }
    public event Action<string> Receive;
    public event Action<byte[]> ReceiveBytes;

    public bool IsStarted
    {
        get { lock (_syncRoot) return _connection?.IsConnected == true; }
    }

    public int PendingMessageCount
    {
        get { lock (_syncRoot) return _receivedMessages.Count; }
    }

    // Reads survive automatic reconnect, but never a manual stop/start session boundary.
    public string ReadMessage(CancellationToken cancellationToken, Action<bool> connectionStateChanged = null)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using (cancellationToken.Register(() => { lock (_syncRoot) Monitor.PulseAll(_syncRoot); }))
        {
            TaskCompletionSource<bool> session;
            lock (_syncRoot) session = _session;
            bool? reportedConnected = null;
            while (true)
            {
                bool connected;
                lock (_syncRoot)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (_disposed) throw new ObjectDisposedException(nameof(ContentTcpClient));
                    if (session == null || !ReferenceEquals(session, _session) ||
                        (!AutoReconnect && _connection?.IsConnected != true))
                        throw new InvalidOperationException("TCP 客户端未连接或连接已断开，请先连接内置通讯。");
                    connected = _connection?.IsConnected == true;
                    if (connectionStateChanged != null && reportedConnected != connected)
                    {
                        reportedConnected = connected;
                        // Report outside the transport lock; recheck state/data before waiting.
                    }
                    else
                    {
                        if (!connected)
                        {
                            Monitor.Wait(_syncRoot);
                            continue;
                        }
                        if (_droppedMessages > 0)
                        {
                            long dropped = _droppedMessages;
                            _droppedMessages = 0;
                            throw new IOException("TCP 接收缓存已满，丢弃了 " + dropped + " 个数据块，请提高流程读取速度。");
                        }
                        if (_receivedMessages.Count > 0) return _receivedMessages.Dequeue();
                        Monitor.Wait(_syncRoot);
                        continue;
                    }
                }
                connectionStateChanged(connected);
            }
        }
    }

    public Task<Result> StartAsync()
    {
        lock (_syncRoot)
        {
            if (_disposed) return Task.FromResult(Result.Fail("The TCP client has been disposed."));
            if (_connection?.IsConnected == true) return Task.FromResult(Result.Ok());
            if (_session != null && (AutoReconnect || !_firstConnect.IsCompleted)) return _firstConnect;
            StopCore();
            var session = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            _session = session;
            _firstConnect = Task.Run(() => ConnectOnceAsync(session));
            if (AutoReconnect) _ = MaintainConnectionAsync(session, _firstConnect);
            return _firstConnect;
        }
    }

    private async Task MaintainConnectionAsync(TaskCompletionSource<bool> session, Task<Result> firstConnect)
    {
        await firstConnect.ConfigureAwait(false);
        while (!session.Task.IsCompleted)
        {
            if (await Task.WhenAny(Task.Delay(ReconnectInterval), session.Task).ConfigureAwait(false) == session.Task) return;
            await ConnectOnceAsync(session).ConfigureAwait(false);
        }
    }

    private async Task<Result> ConnectOnceAsync(TaskCompletionSource<bool> session)
    {
        await _startLock.WaitAsync().ConfigureAwait(false);
        TcpClient client = null;
        try
        {
            lock (_syncRoot)
            {
                if (_disposed || !ReferenceEquals(_session, session)) return Result.Fail("The TCP connection was stopped.");
                if (_connection?.IsConnected == true) return Result.Ok();

                _connection?.Dispose();
                _connection = null;
                ClearReceivedMessages();

                client = IPAddress.TryParse(Host, out var address)
                    ? new TcpClient(address.AddressFamily)
                    : new TcpClient();
                _connectingClient = client;
            }

            using (var timeout = new CancellationTokenSource())
            {
                var connect = client.ConnectAsync(Host, Port);
                var delay = Task.Delay(ConnectTimeout, timeout.Token);
                if (await Task.WhenAny(connect, delay, session.Task).ConfigureAwait(false) != connect)
                {
                    timeout.Cancel();
                    client.Close();
                    _ = ObserveConnectAsync(connect);
                    return Result.Fail("The TCP connection timed out.");
                }

                timeout.Cancel();
                await connect.ConfigureAwait(false);
            }

            lock (_syncRoot)
            {
                if (_disposed || !ReferenceEquals(_session, session) || !ReferenceEquals(_connectingClient, client))
                {
                    client.Close();
                    return Result.Fail("The TCP connection was stopped.");
                }

                var connection = new TcpTextConnection(client, _encoding);
                _connection = connection;
                _connectingClient = null;
                Monitor.PulseAll(_syncRoot);
                _ = Task.Run(() => connection.ReceiveAsync(
                    message => OnReceive(connection, message), () => OnClosed(connection),
                    bytes => OnReceiveBytes(connection, bytes)));
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            client?.Close();
            return Result.Fail("Failed to connect to the TCP server: " + ex.Message);
        }
        finally
        {
            lock (_syncRoot)
            {
                if (ReferenceEquals(_connectingClient, client)) _connectingClient = null;
            }
            _startLock.Release();
        }
    }

    public async Task Send(string msg)
    {
        if (msg == null) throw new ArgumentNullException(nameof(msg));
        TcpTextConnection connection;
        lock (_syncRoot)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ContentTcpClient));
            connection = _connection;
        }

        if (connection == null) throw new InvalidOperationException("The TCP client is not connected.");
        await connection.SendAsync(msg).ConfigureAwait(false);
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

    private void StopCore()
    {
        _session?.TrySetResult(true);
        _session = null;
        _connectingClient?.Close();
        _connectingClient = null;
        _connection?.Dispose();
        _connection = null;
        ClearReceivedMessages();
    }

    private void ClearReceivedMessages()
    {
        _receivedMessages.Clear();
        _droppedMessages = 0;
        Monitor.PulseAll(_syncRoot);
    }

    private void OnReceive(TcpTextConnection connection, string message)
    {
        lock (_syncRoot)
        {
            if (!ReferenceEquals(_connection, connection)) return;
            if (_receivedMessages.Count >= MaximumPendingMessages)
            {
                _receivedMessages.Dequeue();
                _droppedMessages++;
            }
            _receivedMessages.Enqueue(message);
            Monitor.PulseAll(_syncRoot);
        }
        TcpTextConnection.RaiseReceive(Receive, message);
    }

    private void OnClosed(TcpTextConnection connection)
    {
        lock (_syncRoot)
        {
            if (ReferenceEquals(_connection, connection))
            {
                _connection = null;
                ClearReceivedMessages();
            }
        }
    }

    private void OnReceiveBytes(TcpTextConnection connection, byte[] bytes)
    {
        lock (_syncRoot)
        {
            if (!ReferenceEquals(_connection, connection)) return;
        }
        TcpTextConnection.RaiseReceiveBytes(ReceiveBytes, bytes);
    }

    private static async Task ObserveConnectAsync(Task connect)
    {
        try { await connect.ConfigureAwait(false); }
        catch { /* Closing a pending socket completes ConnectAsync with an exception. */ }
    }
}
