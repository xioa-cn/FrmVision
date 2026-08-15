using System;
using FrmMapper.Data;
using FrmServices.Communication.LightSource;

namespace FrmServices.Communication
{
    public class LightSourceFrmVpCommunication : IFrmVpCommunication, IDisposable
    {
        private readonly ILightSourceService lightSourceService;

        public LightSourceFrmVpCommunication(ILightSourceService lightSourceService)
        {
            this.lightSourceService = lightSourceService;
        }

        public Result<T[]> Read<T>(string url, ushort length)
        {
            throw new System.NotImplementedException();
        }

        public Result Write<T>(string adress, T data)
        {
            if (data is string str)
            {
                var result = lightSourceService.Send(str);
                ConnectSuccess = result.IsSuccess;
                return result;
            }

            return Result.Fail("光源仅支持字符串写入");
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
            try
            {
                var result = lightSourceService.Connect();
                ConnectSuccess = result != null && result.IsSuccess;
                return result ?? Result.Fail("光源连接未返回结果");
            }
            catch (Exception ex)
            {
                ConnectSuccess = false;
                return Result.Fail("光源连接失败：" + ex.Message);
            }
        }

        public void KeepAlive()
        {
            try
            {
                var result = lightSourceService.KeepAlive();
                ConnectSuccess = result != null && result.IsSuccess;
            }
            catch
            {
                ConnectSuccess = false;
            }
        }

        public void Dispose()
        {
            var disposable = lightSourceService as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}
