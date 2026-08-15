using System;
using FrmMapper.Data;

namespace FrmServices.Communication
{
    public interface IFrmVpCommunication
    {
        Result<T[]> Read<T>(string url, ushort length);
        Result Write<T>(string adress, T data);
        bool ConnectSuccess { get; }

        event Action<bool> ConnectStatusChanged;

        Result Connect();
        void KeepAlive();
    }
}
