using System;
using System.Net.NetworkInformation;
using FrmCommon.LogServices;
using FrmMapper.Data;
using FrmServices.Communication.Extensions;
using HslCommunication.Core.Device;


namespace FrmServices.Communication
{
    // 作者：xioa
    // 作者邮箱：1327916255@qq.com
    public class PlcFrmVpCommunication : IFrmVpCommunication, IDisposable
    {
        private readonly DeviceCommunication deviceCommunication;

        public PlcFrmVpCommunication(DeviceCommunication deviceCommunication)
        {
            this.deviceCommunication = deviceCommunication;
        }

        public Result<T[]> Read<T>(string url, ushort length)
        {
            if (!CommunicationAccessGuard.IsAllowed) return new Result<T[]> { IsSuccess = false, Message = CommunicationAccessGuard.FailureMessage };
            var result = deviceCommunication.ReadAnyType<T>(url, length);
            ConnectSuccess = result.IsSuccess;
            return new Result<T[]>()
            {
                IsSuccess = result.IsSuccess,
                Message = result.Message,
                Data = result.IsSuccess ? result.Content : default(T[])
            };
        }

        public Result Write<T>(string address, T data)
        {
            if (!CommunicationAccessGuard.IsAllowed) return Result.Fail(CommunicationAccessGuard.FailureMessage);
            var result = deviceCommunication.WriteAny(address, data);
            ConnectSuccess = result.IsSuccess;
            return Result.Ok(result.IsSuccess, result.Message);
        }

        private bool _connectSuccess;

        public bool ConnectSuccess
        {
            get => _connectSuccess;
            private set
            {
                if (_connectSuccess == value)
                    return;

                _connectSuccess = value;
                ConnectStatusChanged?.Invoke(value);
            }
        }

        public event Action<bool> ConnectStatusChanged;

        public Result Connect()
        {
            if (!CommunicationAccessGuard.IsAllowed) return Result.Fail(CommunicationAccessGuard.FailureMessage);
            Result result;
            switch (deviceCommunication)
            {
                case DeviceTcpNet deviceTcpNet:
                {
                    var ret = deviceTcpNet.ConnectServer();
                    result = Result.Ok(ret.IsSuccess, ret.Message);
                    break;
                }
                case DeviceUdpNet deviceUdpNet:
                {
                    var ret = deviceUdpNet.IpAddressPing();
                    result = Result.Ok(ret == IPStatus.Success);
                    break;
                }
                case DeviceSerialPort deviceSerialPort:
                {
                    var ret = deviceSerialPort.Open();
                    result = Result.Ok(ret.IsSuccess, ret.Message);
                    break;
                }
                default:
                    result = Result.Fail("未解析到plc类型");
                    break;
            }

            ConnectSuccess = result.IsSuccess;
            return result;
        }

        public void KeepAlive()
        {
        }

        public void Dispose()
        {
            deviceCommunication.Dispose();
        }
    }
}
