using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FrmServices.LogServices;

namespace FrmServices.Services.CommunicationServices;

internal sealed class TcpTextConnection : IDisposable
{
    private readonly TcpClient _client;
    private readonly NetworkStream _stream;
    private readonly Encoding _encoding;
    private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);
    private int _disposed;

    public TcpTextConnection(TcpClient client, Encoding encoding)
    {
        _client = client;
        _client.NoDelay = true;
        _stream = client.GetStream();
        _encoding = encoding;
    }

    public bool IsConnected => Volatile.Read(ref _disposed) == 0;

    public async Task SendAsync(string message)
    {
        var bytes = _encoding.GetBytes(message);
        await _sendLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!IsConnected) throw new IOException("The TCP connection is closed.");
            await _stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
        }
        catch
        {
            Dispose();
            throw;
        }
        finally
        {
            _sendLock.Release();
        }
    }

    public async Task ReceiveAsync(Action<string> receive, Action closed, Action<byte[]> receiveBytes)
    {
        var buffer = new byte[4096];
        var chars = new char[_encoding.GetMaxCharCount(buffer.Length)];
        // Each connection keeps its decoder so split multibyte characters survive packet boundaries.
        var decoder = _encoding.GetDecoder();
        try
        {
            while (IsConnected)
            {
                var count = await _stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                if (!IsConnected) break;
                if (count > 0 && receiveBytes != null)
                {
                    var received = new byte[count];
                    Buffer.BlockCopy(buffer, 0, received, 0, count);
                    receiveBytes(received);
                }
                var charCount = decoder.GetChars(buffer, 0, count, chars, 0, count == 0);
                if (charCount > 0) receive(new string(chars, 0, charCount));
                if (count == 0) break;
            }
        }
        catch (Exception ex)
        {
            if (IsConnected) AppLog.Error("TCP receive failed: " + ex, nameof(TcpTextConnection));
        }
        finally
        {
            Dispose();
            closed();
        }
    }

    internal static void RaiseReceive(Action<string> handlers, string message)
    {
        if (handlers == null) return;
        foreach (Action<string> handler in handlers.GetInvocationList())
        {
            try { handler(message); }
            catch (Exception ex)
            {
                AppLog.Error("TCP receive handler failed: " + ex, nameof(TcpTextConnection));
            }
        }
    }

    internal static void RaiseReceiveBytes(Action<byte[]> handlers, byte[] bytes)
    {
        if (handlers == null) return;
        foreach (Action<byte[]> handler in handlers.GetInvocationList())
        {
            // A subscriber must not corrupt another subscriber's raw capture.
            try { handler((byte[])bytes.Clone()); }
            catch (Exception ex)
            {
                AppLog.Error("TCP byte receive handler failed: " + ex, nameof(TcpTextConnection));
            }
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        _client.Close();
    }
}
