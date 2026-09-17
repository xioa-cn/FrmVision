using System;
using System.Threading.Tasks;
using FrmMapper.Data;
using FrmServices.Services.CommunicationServices.Services;

namespace FrmServices.Services.CommunicationServices;

public class OriginalSerialPort : IContentServer
{
    public void Dispose()
    {
        // TODO 在此释放托管资源
    }

    public bool IsStarted { get; }
    public Task<Result> StartAsync()
    {
        throw new NotImplementedException();
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }

    public Task Send(string msg)
    {
        throw new NotImplementedException();
    }

    public event Action<string> Receive;
    public event Action<byte[]> ReceiveBytes;
}