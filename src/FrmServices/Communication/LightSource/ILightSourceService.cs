using System;
using FrmMapper.Data;

namespace FrmServices.Communication.LightSource;

public interface ILightSourceService
{
    bool IsConnected { get; }
    public event Action<string> DataReceived;
    Result Send(string message);

    Result Connect();

    Result KeepAlive();
}