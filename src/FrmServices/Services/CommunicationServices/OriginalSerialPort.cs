using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
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
public class OriginalSerialPort : IContentServer
{
    private readonly object _syncRoot = new object();
    private readonly SemaphoreSlim _startLock = new SemaphoreSlim(1, 1);
    private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);
    private readonly Encoding _encoding;
    private const int MaximumPendingMessages = 1024;
    private readonly Queue<string> _receivedMessages = new Queue<string>();
    private long _droppedMessages;
    private SerialPort _port;
    private bool _disposed;

    public OriginalSerialPort(string portName = "COM1", int baudRate = 9600,
        int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One,
        Encoding encoding = null, int receiveIdleMilliseconds = 20)
    {
        if (string.IsNullOrWhiteSpace(portName))
            throw new ArgumentException("Port name is required.", nameof(portName));
        if (baudRate <= 0) throw new ArgumentOutOfRangeException(nameof(baudRate));
        if (dataBits < 5 || dataBits > 8) throw new ArgumentOutOfRangeException(nameof(dataBits));
        if (!Enum.IsDefined(typeof(Parity), parity)) throw new ArgumentOutOfRangeException(nameof(parity));
        if (!Enum.IsDefined(typeof(StopBits), stopBits) || stopBits == StopBits.None)
            throw new ArgumentOutOfRangeException(nameof(stopBits));
        if (receiveIdleMilliseconds < 0) throw new ArgumentOutOfRangeException(nameof(receiveIdleMilliseconds));

        PortName = portName.Trim();
        BaudRate = baudRate;
        DataBits = dataBits;
        Parity = parity;
        StopBits = stopBits;
        ReceiveIdleMilliseconds = receiveIdleMilliseconds;
        _encoding = (Encoding)(encoding ?? Encoding.UTF8).Clone();
    }

    public string PortName { get; }
    public int BaudRate { get; }
    public int DataBits { get; }
    public Parity Parity { get; }
    public StopBits StopBits { get; }
    public int ReceiveIdleMilliseconds { get; }
    public event Action<string> Receive;
    public event Action<byte[]> ReceiveBytes;

    public bool IsStarted
    {
        get
        {
            lock (_syncRoot) return _port?.IsOpen == true;
        }
    }

    public int PendingMessageCount
    {
        get
        {
            lock (_syncRoot) return _receivedMessages.Count;
        }
    }

    // Consumes one decoded serial chunk. The serial byte stream does not define application frames.
    public string ReadMessage(int timeoutMilliseconds, CancellationToken cancellationToken)
    {
        CommunicationAccessGuard.EnsureAllowed();
        if (timeoutMilliseconds < 0) throw new ArgumentOutOfRangeException(nameof(timeoutMilliseconds));
        cancellationToken.ThrowIfCancellationRequested();
        var elapsed = System.Diagnostics.Stopwatch.StartNew();
        using (cancellationToken.Register(() =>
               {
                   lock (_syncRoot) Monitor.PulseAll(_syncRoot);
               }))
        {
            lock (_syncRoot)
            {
                var port = _port;
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (_disposed) throw new ObjectDisposedException(nameof(OriginalSerialPort));
                    if (port == null || !ReferenceEquals(port, _port) || !port.IsOpen)
                        throw new InvalidOperationException("串口未打开或已停止，请先启动内置通讯。");
                    if (_droppedMessages > 0)
                    {
                        long dropped = _droppedMessages;
                        _droppedMessages = 0;
                        throw new IOException("串口接收缓存已满，丢弃了 " + dropped + " 个数据块，请提高流程读取速度。");
                    }

                    if (_receivedMessages.Count > 0) return _receivedMessages.Dequeue();
                    int remaining = timeoutMilliseconds == 0
                        ? Timeout.Infinite
                        : (int)Math.Max(0, timeoutMilliseconds - elapsed.ElapsedMilliseconds);
                    if (remaining == 0) throw new TimeoutException("等待串口接收数据超时（" + timeoutMilliseconds + " ms）。");
                    Monitor.Wait(_syncRoot, remaining);
                }
            }
        }
    }

    public async Task<Result> StartAsync()
    {
        if (!CommunicationAccessGuard.IsAllowed) return Result.Fail(CommunicationAccessGuard.FailureMessage);
        await _startLock.WaitAsync().ConfigureAwait(false);
        try
        {
            SerialPort leftover = null;
            lock (_syncRoot)
            {
                if (_disposed) return Result.Fail("The serial port has been disposed.");
                if (_port?.IsOpen == true) return Result.Ok();
                leftover = _port;
                _port = null;
                ClearReceivedMessages();
            }

            ClosePort(leftover);

            var port = new SerialPort(PortName, BaudRate, Parity, DataBits, StopBits)
            {
                Handshake = Handshake.None,
                DtrEnable = true,
                RtsEnable = true,
                ReadTimeout = Timeout.Infinite,
                WriteTimeout = 5000
            };
            try
            {
                port.Open();
            }
            catch (Exception ex)
            {
                ClosePort(port);
                return Result.Fail("Failed to open the serial port: " + ex.Message);
            }

            lock (_syncRoot)
            {
                if (_disposed)
                {
                    ClosePort(port);
                    return Result.Fail("The serial port has been disposed.");
                }

                if (_port != null)
                {
                    ClosePort(port);
                    return Result.Ok();
                }

                _port = port;
            }

            _ = Task.Factory.StartNew(() => ReceiveLoop(port), CancellationToken.None,
                TaskCreationOptions.LongRunning, TaskScheduler.Default);
            return Result.Ok();
        }
        finally
        {
            _startLock.Release();
        }
    }

    public async Task Send(string msg)
    {
        CommunicationAccessGuard.EnsureAllowed();
        if (msg == null) throw new ArgumentNullException(nameof(msg));
        SerialPort port;
        lock (_syncRoot)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(OriginalSerialPort));
            port = _port;
            if (port == null || !port.IsOpen) throw new InvalidOperationException("The serial port is not open.");
        }

        var bytes = _encoding.GetBytes(msg);
        await _sendLock.WaitAsync().ConfigureAwait(false);
        try
        {
            lock (_syncRoot)
            {
                if (!ReferenceEquals(_port, port) || !port.IsOpen)
                    throw new InvalidOperationException("The serial port is not open.");
            }

            await port.BaseStream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
        }
        finally
        {
            _sendLock.Release();
        }
    }

    public void Stop()
    {
        _startLock.Wait();
        try
        {
            StopCore();
        }
        finally
        {
            _startLock.Release();
        }
    }

    public void Dispose()
    {
        _startLock.Wait();
        try
        {
            if (_disposed) return;
            _disposed = true;
            StopCore();
        }
        finally
        {
            _startLock.Release();
        }
    }

    private void StopCore()
    {
        SerialPort port;
        lock (_syncRoot)
        {
            port = _port;
            _port = null;
            ClearReceivedMessages();
        }

        ClosePort(port);
    }

    private void ClearReceivedMessages()
    {
        _receivedMessages.Clear();
        _droppedMessages = 0;
        Monitor.PulseAll(_syncRoot);
    }

    private void ReceiveLoop(SerialPort port)
    {
        var buffer = new byte[4096];
        var assembled = new MemoryStream();
        var decoder = _encoding.GetDecoder();
        var chars = new char[_encoding.GetMaxCharCount(buffer.Length)];
        try
        {
            Stream stream;
            try
            {
                stream = port.BaseStream;
            }
            catch
            {
                return;
            }

            while (true)
            {
                lock (_syncRoot)
                {
                    if (_disposed || !ReferenceEquals(_port, port)) return;
                }

                int count;
                try
                {
                    count = stream.Read(buffer, 0, buffer.Length);
                }
                catch
                {
                    lock (_syncRoot)
                    {
                        if (!ReferenceEquals(_port, port) || _disposed) return;
                    }

                    return;
                }

                if (count == 0)
                {
                    PublishAssembled(port, assembled, decoder, ref chars, true);
                    return;
                }

                assembled.Write(buffer, 0, count);
                if (ReceiveIdleMilliseconds > 0)
                    DrainUntilIdle(port, stream, buffer, assembled);
                if (!PublishAssembled(port, assembled, decoder, ref chars, false)) return;
            }
        }
        catch (Exception ex)
        {
            lock (_syncRoot)
            {
                if (ReferenceEquals(_port, port) && !_disposed)
                    AppLog.Error("Serial receive failed: " + ex, nameof(OriginalSerialPort));
            }
        }
        finally
        {
            SerialPort leftover = null;
            lock (_syncRoot)
            {
                if (ReferenceEquals(_port, port))
                {
                    leftover = _port;
                    _port = null;
                    ClearReceivedMessages();
                }
            }

            ClosePort(leftover);
        }
    }

    private void DrainUntilIdle(SerialPort port, Stream stream, byte[] buffer, MemoryStream assembled)
    {
        const int maximumAssembleBytes = 65536;
        int idle = ReceiveIdleMilliseconds;
        var last = System.Diagnostics.Stopwatch.StartNew();
        while (last.ElapsedMilliseconds < idle && assembled.Length < maximumAssembleBytes)
        {
            lock (_syncRoot)
            {
                if (_disposed || !ReferenceEquals(_port, port)) return;
            }

            int available;
            try
            {
                available = port.BytesToRead;
            }
            catch
            {
                return;
            }

            if (available > 0)
            {
                int remaining = maximumAssembleBytes - (int)assembled.Length;
                if (remaining <= 0) return;
                int toRead = Math.Min(Math.Min(available, buffer.Length), remaining);
                int count;
                try
                {
                    count = stream.Read(buffer, 0, toRead);
                }
                catch
                {
                    return;
                }

                if (count <= 0) return;
                assembled.Write(buffer, 0, count);
                last.Restart();
                continue;
            }

            Thread.Sleep(1);
        }
    }

    private bool PublishAssembled(SerialPort port, MemoryStream assembled, Decoder decoder, ref char[] chars,
        bool flush)
    {
        lock (_syncRoot)
        {
            if (_disposed || !ReferenceEquals(_port, port)) return false;
        }

        byte[] payload = assembled.Length == 0 ? Array.Empty<byte>() : assembled.ToArray();
        assembled.SetLength(0);
        if (payload.Length > 0)
        {
            OnReceiveBytes(port, payload);
            int maxChars = _encoding.GetMaxCharCount(payload.Length);
            if (chars.Length < maxChars) chars = new char[maxChars];
            int charCount = decoder.GetChars(payload, 0, payload.Length, chars, 0, flush);
            if (charCount > 0) OnReceive(port, new string(chars, 0, charCount));
            return true;
        }

        if (flush)
        {
            int flushed = decoder.GetChars(Array.Empty<byte>(), 0, 0, chars, 0, true);
            if (flushed > 0) OnReceive(port, new string(chars, 0, flushed));
        }

        return true;
    }

    private void OnReceive(SerialPort port, string message)
    {
        lock (_syncRoot)
        {
            if (!ReferenceEquals(_port, port)) return;
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

    private void OnReceiveBytes(SerialPort port, byte[] bytes)
    {
        lock (_syncRoot)
        {
            if (!ReferenceEquals(_port, port)) return;
        }

        RaiseReceiveBytes(ReceiveBytes, bytes);
    }

    private static void ClosePort(SerialPort port)
    {
        if (port == null) return;
        try
        {
            if (port.IsOpen) port.Close();
        }
        catch
        {
            /* Closing an unplugged or already-closed port is best effort. */
        }

        try
        {
            port.Dispose();
        }
        catch
        {
        }
    }

    private static void RaiseReceive(Action<string> handlers, string message)
    {
        if (handlers == null) return;
        foreach (Action<string> handler in handlers.GetInvocationList())
        {
            try
            {
                handler(message);
            }
            catch (Exception ex)
            {
                AppLog.Error("Serial receive handler failed: " + ex, nameof(OriginalSerialPort));
            }
        }
    }

    private static void RaiseReceiveBytes(Action<byte[]> handlers, byte[] bytes)
    {
        if (handlers == null) return;
        foreach (Action<byte[]> handler in handlers.GetInvocationList())
        {
            try
            {
                handler((byte[])bytes.Clone());
            }
            catch (Exception ex)
            {
                AppLog.Error("Serial byte receive handler failed: " + ex, nameof(OriginalSerialPort));
            }
        }
    }
}