using System;
using System.Threading.Tasks;
using FrmMapper.Data;

namespace FrmServices.Services.CommunicationServices.Services;

public interface IContentNodeServer : IDisposable
{
    bool IsStarted { get; }
    Result Start();
    void Stop();

    // Typed lengths count values. byte/string lengths count registers; string returns one string.
    Result<T[]> Read<T>(string adress, short length);
    Result Write<T>(string adress, T value);
}

public interface IContentServer : IDisposable
{
    bool IsStarted { get; }
    Task<Result> StartAsync();
    void Stop();
    Task Send(string msg);

    // Raised on a background thread. TCP chunks do not define application message boundaries.
    event Action<string> Receive;
    // Exact bytes read from the socket, before text decoding (not application frames).
    event Action<byte[]> ReceiveBytes;
}
