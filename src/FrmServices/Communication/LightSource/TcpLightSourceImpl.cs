using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using FrmMapper.Data;
using FrmServices.LogServices;

namespace FrmServices.Communication.LightSource;

public class TcpLightSourceImpl : ILightSourceService, IDisposable
{
    private const int ReceiveThreadJoinTimeout = 2000;
    private readonly object _lifecycleRoot = new object();
    private readonly object _syncRoot = new object();
    private readonly object _sendRoot = new object();
    private readonly string _host;
    private readonly int _port;
    private readonly Encoding _encoding;
    private Socket _socket;
    private Thread _receiveThread;
    private long _connectionGeneration;
    private bool _isConnected;
    private bool _disposed;

    public TcpLightSourceImpl(string host, int port, Encoding encoding = null)
    {
        if (string.IsNullOrWhiteSpace(host))
            throw new ArgumentException("主机地址不能为空。", nameof(host));
        if (port <= IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            throw new ArgumentOutOfRangeException(nameof(port), "端口号必须在 1 到 65535 之间。");

        _host = host.Trim();
        _port = port;
        _encoding = encoding ?? Encoding.UTF8;
    }

    public event Action<string> DataReceived;

    public bool IsConnected
    {
        get
        {
            lock (_syncRoot)
                return _isConnected;
        }
    }

    public Result Send(string message)
    {
        if (string.IsNullOrEmpty(message))
            return Result.Fail("发送内容不能为空");

        var socket = GetCurrentSocket();
        if (socket == null)
            return Result.Fail("TCP 未连接");

        return SendBytes(socket, _encoding.GetBytes(message), "TCP 发送失败：");
    }

    public Result Connect()
    {
        lock (_lifecycleRoot)
        {
            lock (_syncRoot)
            {
                if (_disposed)
                    return Result.Fail("TCP 连接已释放");
                if (_isConnected)
                    return Result.Ok();
            }

            if (!DisconnectCurrentConnection())
                return Result.Fail("旧的 TCP 接收线程未能退出，取消重新连接");

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                ProtocolType.Tcp)
            {
                NoDelay = true,
                ReceiveTimeout = 1000,
                SendTimeout = 1000
            };

            try
            {
                socket.Connect(_host, _port);
                long connectionGeneration = 0;
                var receiveThread = new Thread(() =>
                    ReceiveLoop(socket, connectionGeneration))
                {
                    IsBackground = true,
                    Name = "TcpLightSourceReceive"
                };

                lock (_syncRoot)
                {
                    connectionGeneration = ++_connectionGeneration;
                    _socket = socket;
                    _receiveThread = receiveThread;
                    _isConnected = true;
                }

                receiveThread.Start();
                return Result.Ok();
            }
            catch (Exception ex)
            {
                DisconnectSocket(socket);
                return Result.Fail("TCP 连接失败：" + ex.Message);
            }
        }
    }

    public Result KeepAlive()
    {
        var socket = GetCurrentSocket();
        if (socket == null)
            return Result.Fail("TCP 未连接");

        return SendBytes(socket, new byte[] { 0xff }, "TCP 心跳失败：");
    }

    private Socket GetCurrentSocket()
    {
        lock (_syncRoot)
            return _socket;
    }

    private void ReceiveLoop(Socket socket, long connectionGeneration)
    {
        var buffer = new byte[4096];
        try
        {
            while (true)
            {
                if (!IsCurrentConnection(socket, connectionGeneration))
                    return;

                try
                {
                    var received = socket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                    if (received == 0)
                    {
                        MarkDisconnected(socket);
                        return;
                    }

                    var data = _encoding.GetString(buffer, 0, received);
                    if (!string.IsNullOrEmpty(data))
                        RaiseDataReceived(data);
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    continue;
                }
                catch (Exception ex)
                {
                    if (MarkDisconnected(socket))
                        AppLog.Error("TCP 数据接收失败" + Environment.NewLine + ex,
                            nameof(TcpLightSourceImpl));
                    return;
                }
            }
        }
        finally
        {
            ClearReceiveThread(Thread.CurrentThread);
        }
    }

    private Result SendBytes(Socket socket, byte[] buffer, string errorPrefix)
    {
        try
        {
            lock (_sendRoot)
            {
                var offset = 0;
                while (offset < buffer.Length)
                {
                    var sent = socket.Send(buffer, offset, buffer.Length - offset,
                        SocketFlags.None);
                    if (sent <= 0)
                        throw new SocketException((int)SocketError.ConnectionReset);
                    offset += sent;
                }
            }

            return Result.Ok();
        }
        catch (Exception ex)
        {
            MarkDisconnected(socket);
            return Result.Fail(errorPrefix + ex.Message);
        }
    }

    private void RaiseDataReceived(string data)
    {
        try
        {
            DataReceived?.Invoke(data);
        }
        catch (Exception ex)
        {
            AppLog.Error("TCP 数据处理失败" + Environment.NewLine + ex,
                nameof(TcpLightSourceImpl));
        }
    }

    private bool MarkDisconnected(Socket socket)
    {
        lock (_syncRoot)
        {
            if (!ReferenceEquals(_socket, socket)) return false;
            _connectionGeneration++;
            _socket = null;
            _isConnected = false;
        }

        CloseSocket(socket);
        return true;
    }

    private bool DisconnectCurrentConnection()
    {
        Socket socket;
        Thread receiveThread;
        lock (_syncRoot)
        {
            socket = _socket;
            receiveThread = _receiveThread;
            _connectionGeneration++;
            _socket = null;
            _isConnected = false;
        }

        CloseSocket(socket);
        if (!WaitForReceiveThread(receiveThread))
            return false;

        ClearReceiveThread(receiveThread);
        return true;
    }

    private void DisconnectSocket(Socket socket)
    {
        Thread receiveThread = null;
        lock (_syncRoot)
        {
            if (ReferenceEquals(_socket, socket))
            {
                _connectionGeneration++;
                _socket = null;
                _isConnected = false;
                receiveThread = _receiveThread;
            }
        }

        CloseSocket(socket);
        if (WaitForReceiveThread(receiveThread))
            ClearReceiveThread(receiveThread);
    }

    private static void CloseSocket(Socket socket)
    {
        if (socket == null) return;
        try
        {
            socket.Shutdown(SocketShutdown.Both);
        }
        catch (SocketException)
        {
        }
        catch (ObjectDisposedException)
        {
        }
        finally
        {
            socket.Dispose();
        }
    }

    private bool IsCurrentConnection(Socket socket, long connectionGeneration)
    {
        lock (_syncRoot)
            return ReferenceEquals(_socket, socket) &&
                   _connectionGeneration == connectionGeneration &&
                   _isConnected;
    }

    private static bool WaitForReceiveThread(Thread receiveThread)
    {
        if (receiveThread == null || receiveThread == Thread.CurrentThread)
            return true;
        if ((receiveThread.ThreadState & ThreadState.Unstarted) != 0)
            return true;

        return receiveThread.Join(ReceiveThreadJoinTimeout);
    }

    private void ClearReceiveThread(Thread receiveThread)
    {
        if (receiveThread == null) return;
        lock (_syncRoot)
        {
            if (ReferenceEquals(_receiveThread, receiveThread))
                _receiveThread = null;
        }
    }

    public void Dispose()
    {
        lock (_lifecycleRoot)
        {
            lock (_syncRoot)
            {
                if (_disposed) return;
                _disposed = true;
            }

            if (!DisconnectCurrentConnection())
                AppLog.Warning("TCP 接收线程未能在限定时间内退出",
                    nameof(TcpLightSourceImpl));
        }
    }
}
