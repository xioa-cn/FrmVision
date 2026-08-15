using System;
using System.Net.NetworkInformation;
using FrmMapper.Data;

namespace FrmServices.Communication
{
    public class CameraFrmVpCommunication : IFrmVpCommunication, IDisposable
    {
        private readonly Ping _ping = new Ping();

        private string _Ip;

        private int _timeout;

        public CameraFrmVpCommunication(string ip, int timeout = 500)
        {
            _Ip = ip;
            _timeout = timeout;
        }


        public Result<T[]> Read<T>(string url, ushort length)
        {
            return default;
        }

        public Result Write<T>(string adress, T data)
        {
            return default;
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
            return CheckConnection();
        }

        public void KeepAlive()
        {
            CheckConnection();
        }

        private Result CheckConnection()
        {
            try
            {
                var pingCam = _ping.Send(_Ip, _timeout);
                ConnectSuccess = pingCam.Status == IPStatus.Success;
                return Result.Ok(ConnectSuccess,
                    ConnectSuccess ? string.Empty : "相机 Ping 未响应");
            }
            catch (Exception ex)
            {
                ConnectSuccess = false;
                return Result.Fail("相机连接检测失败：" + ex.Message);
            }
        }

        public void Dispose()
        {
            _ping.Dispose();
        }
    }
}
