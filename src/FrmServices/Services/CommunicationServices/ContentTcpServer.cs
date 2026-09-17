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

public class ContentTcpServer : IContentServer
{
    private readonly object _syncRoot = new object();
    private readonly HashSet<TcpTextConnection> _clients = new HashSet<TcpTextConnection>();
    private const int MaximumPendingMessages = 1024;
    private readonly Queue<string> _receivedMessages = new Queue<string>();
    private long _droppedMessages;
    private readonly IPAddress _address;
    private readonly Encoding _encoding;
    private TcpListener _listener;
    private bool _disposed;

    public ContentTcpServer(int port = 5000, string ipAddress = "0.0.0.0", Encoding encoding = null)
    {
        if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            throw new ArgumentOutOfRangeException(nameof(port));
        if (!IPAddress.TryParse(ipAddress, out _address))
            throw new ArgumentException("A local IP address is required.", nameof(ipAddress));

        Port = port;
        _encoding = (Encoding)(encoding ?? Encoding.UTF8).Clone();
    }

    // Port zero asks the OS for a free port; LocalEndPoint exposes the selected port.
    public int Port { get; }
    public event Action<string> Receive;
    public event Action<byte[]> ReceiveBytes;

    public bool IsStarted
    {
        get { lock (_syncRoot) return _listener != null; }
    }

    public IPEndPoint LocalEndPoint
    {
        get { lock (_syncRoot) return (IPEndPoint)_listener?.LocalEndpoint; }
    }

    public int ClientCount
    {
        get { lock (_syncRoot) return _clients.Count(client => client.IsConnected); }
    }

    public int PendingMessageCount
    {
        get { lock (_syncRoot) return _receivedMessages.Count; }
    }

    // Consumes one decoded TCP chunk. TCP itself does not define application message boundaries.
    // Called by workflow workers, never by the WinForms UI thread.
    public string ReadMessage(int timeoutMilliseconds, CancellationToken cancellationToken)
    {
        if (timeoutMilliseconds < 0) throw new ArgumentOutOfRangeException(nameof(timeoutMilliseconds));
        cancellationToken.ThrowIfCancellationRequested();
        var elapsed = Stopwatch.StartNew();
        using (cancellationToken.Register(() => { lock (_syncRoot) Monitor.PulseAll(_syncRoot); }))
        {
            lock (_syncRoot)
            {
                var listener = _listener;
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (_disposed) throw new ObjectDisposedException(nameof(ContentTcpServer));
                    if (listener == null || !ReferenceEquals(listener, _listener))
                        throw new InvalidOperationException("TCP 服务端未启动或已停止，请先启动内置通讯。");
                    if (_droppedMessages > 0)
                    {
                        long dropped = _droppedMessages;
                        _droppedMessages = 0;
                        throw new IOException("TCP 接收缓存已满，丢弃了 " + dropped + " 个数据块，请提高流程读取速度。");
                    }
                    if (_receivedMessages.Count > 0) return _receivedMessages.Dequeue();
                    int remaining = timeoutMilliseconds == 0 ? Timeout.Infinite
                        : (int)Math.Max(0, timeoutMilliseconds - elapsed.ElapsedMilliseconds);
                    if (remaining == 0) throw new TimeoutException("等待 TCP 接收数据超时（" + timeoutMilliseconds + " ms）。");
                    Monitor.Wait(_syncRoot, remaining);
                }
            }
        }
    }

    public Result Start()
    {
        lock (_syncRoot)
        {
            if (_disposed) return Result.Fail("The TCP server has been disposed.");
            if (_listener != null) return Result.Ok();

            var listener = new TcpListener(_address, Port);
            try
            {
                listener.Start();
                _listener = listener;
                _ = Task.Run(() => AcceptAsync(listener));
                return Result.Ok();
            }
            catch (Exception ex)
            {
                listener.Stop();
                return Result.Fail("Failed to start the TCP server: " + ex.Message);
            }
        }
    }

    public Task<Result> StartAsync() => Task.FromResult(Start());

    // Sends the same text to every client that is connected at the time of this call.
    public async Task Send(string msg)
    {
        if (msg == null) throw new ArgumentNullException(nameof(msg));
        TcpTextConnection[] clients;
        lock (_syncRoot)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ContentTcpServer));
            if (_listener == null) throw new InvalidOperationException("The TCP server is not started.");
            clients = _clients.Where(client => client.IsConnected).ToArray();
        }

        if (clients.Length == 0) throw new InvalidOperationException("No TCP clients are connected.");
        await Task.WhenAll(clients.Select(client => client.SendAsync(msg))).ConfigureAwait(false);
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

    private async Task AcceptAsync(TcpListener listener)
    {
        try
        {
            while (true)
            {
                var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                lock (_syncRoot)
                {
                    if (!ReferenceEquals(_listener, listener))
                    {
                        client.Close();
                        return;
                    }

                    TcpTextConnection connection;
                    try { connection = new TcpTextConnection(client, _encoding); }
                    catch
                    {
                        client.Close();
                        throw;
                    }
                    _clients.Add(connection);
                    _ = Task.Run(() => connection.ReceiveAsync(
                        message => OnReceive(connection, message), () => OnClosed(connection),
                        bytes => OnReceiveBytes(connection, bytes)));
                }
            }
        }
        catch (Exception ex)
        {
            lock (_syncRoot)
            {
                if (!ReferenceEquals(_listener, listener)) return;
                StopCore();
            }
            AppLog.Error("TCP accept failed: " + ex, nameof(ContentTcpServer));
        }
    }

    private void OnReceive(TcpTextConnection connection, string message)
    {
        lock (_syncRoot)
        {
            if (!_clients.Contains(connection)) return;
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
        lock (_syncRoot) _clients.Remove(connection);
    }

    private void OnReceiveBytes(TcpTextConnection connection, byte[] bytes)
    {
        lock (_syncRoot)
        {
            if (!_clients.Contains(connection)) return;
        }
        TcpTextConnection.RaiseReceiveBytes(ReceiveBytes, bytes);
    }

    private void StopCore()
    {
        var listener = _listener;
        _listener = null;
        listener?.Stop();
        foreach (var client in _clients) client.Dispose();
        _clients.Clear();
        _receivedMessages.Clear();
        _droppedMessages = 0;
        Monitor.PulseAll(_syncRoot);
    }
}
