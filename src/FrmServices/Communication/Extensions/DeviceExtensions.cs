using HslCommunication;
using HslCommunication.Core.Device;
using HslCommunication.Profinet.Siemens;

namespace FrmServices.Communication.Extensions;

public static class DeviceExtensions
{
    public static OperateResult<T[]> ReadAnyType<T>(this DeviceCommunication deviceCommunication, string address,
        ushort length)
    {
        var type = typeof(T);
        if (type == typeof(bool))
        {
            var result = deviceCommunication.ReadBool(address, length);

            return new OperateResult<T[]>()
            {
                IsSuccess = result.IsSuccess,
                ErrorCode = result.ErrorCode,
                Message = result.Message,
                Content = result.Content as T[]
            };
        }
        else if (type == typeof(byte))
        {
            var ret = deviceCommunication.Read(address, length);
            return new OperateResult<T[]>()
            {
                IsSuccess = ret.IsSuccess,
                ErrorCode = ret.ErrorCode,
                Message = ret.Message,
                Content = ret.Content as T[]
            };
        }
        else if (type == typeof(short))
        {
            var result = deviceCommunication.ReadInt16(address, length);

            return new OperateResult<T[]>()
            {
                IsSuccess = result.IsSuccess,
                ErrorCode = result.ErrorCode,
                Message = result.Message,
                Content = result.Content as T[]
            };
        }
        else if (type == typeof(ushort))
        {
            var result = deviceCommunication.ReadUInt16(address, length);
            return new OperateResult<T[]>()
            {
                IsSuccess = result.IsSuccess,
                ErrorCode = result.ErrorCode,
                Message = result.Message,
                Content = result.Content as T[]
            };
        }
        else if (type == typeof(int))
        {
            var result = deviceCommunication.ReadInt32(address, length);
            return new OperateResult<T[]>()
            {
                IsSuccess = result.IsSuccess,
                ErrorCode = result.ErrorCode,
                Message = result.Message,
                Content = result.Content as T[]
            };
        }
        else if (type == typeof(uint))
        {
            var result = deviceCommunication.ReadUInt32(address, length);
            return new OperateResult<T[]>()
            {
                IsSuccess = result.IsSuccess,
                ErrorCode = result.ErrorCode,
                Message = result.Message,
                Content = result.Content as T[]
            };
        }
        else if (type == typeof(long))
        {
            var result = deviceCommunication.ReadInt64(address, length);
            return new OperateResult<T[]>()
            {
                IsSuccess = result.IsSuccess,
                ErrorCode = result.ErrorCode,
                Message = result.Message,
                Content = result.Content as T[]
            };
        }
        else if (type == typeof(ulong))
        {
            var result = deviceCommunication.ReadUInt64(address, length);
            return new OperateResult<T[]>()
            {
                IsSuccess = result.IsSuccess,
                ErrorCode = result.ErrorCode,
                Message = result.Message,
                Content = result.Content as T[]
            };
        }
        else if (type == typeof(float))
        {
            var result = deviceCommunication.ReadFloat(address, length);

            return new OperateResult<T[]>()
            {
                IsSuccess = result.IsSuccess,
                ErrorCode = result.ErrorCode,
                Message = result.Message,
                Content = result.Content as T[]
            };
        }
        else if (type == typeof(double))
        {
            var result = deviceCommunication.ReadDouble(address, length);
            return new OperateResult<T[]>()
            {
                IsSuccess = result.IsSuccess,
                ErrorCode = result.ErrorCode,
                Message = result.Message,
                Content = result.Content as T[]
            };
        }
        else if (type == typeof(string))
        {
            if (deviceCommunication is SiemensS7Net s7)
            {
                var result = s7.ReadString(address);
                var list = new string[]
                {
                    result.IsSuccess ? result.Content : ""
                };
                return new OperateResult<T[]>()
                {
                    IsSuccess = result.IsSuccess,
                    ErrorCode = result.ErrorCode,
                    Message = result.Message,
                    Content = list as T[]
                };
            }
            else
            {
                var result = deviceCommunication.ReadString(address, length);
                var list = new string[]
                {
                    result.IsSuccess ? result.Content : ""
                };
                return new OperateResult<T[]>()
                {
                    IsSuccess = result.IsSuccess,
                    ErrorCode = result.ErrorCode,
                    Message = result.Message,
                    Content = list as T[]
                };
            }
        }

        return new OperateResult<T[]>()
        {
            IsSuccess = false,
            Message = $"不支持的类型 {type}"
        };
    }

    public static OperateResult WriteAny<T>(this DeviceCommunication deviceCommunication, string address, T data)
    {
        var type = typeof(T);

        if (type == typeof(bool))
            return deviceCommunication.Write(address, (bool)(object)data);
        if (type == typeof(byte))
            return deviceCommunication.Write(address, (byte)(object)data);
        if (type == typeof(short))
            return deviceCommunication.Write(address, (short)(object)data);
        if (type == typeof(ushort))
            return deviceCommunication.Write(address, (ushort)(object)data);
        if (type == typeof(int))
            return deviceCommunication.Write(address, (int)(object)data);
        if (type == typeof(uint))
            return deviceCommunication.Write(address, (uint)(object)data);
        if (type == typeof(long))
            return deviceCommunication.Write(address, (long)(object)data);
        if (type == typeof(ulong))
            return deviceCommunication.Write(address, (ulong)(object)data);
        if (type == typeof(float))
            return deviceCommunication.Write(address, (float)(object)data);
        if (type == typeof(double))
            return deviceCommunication.Write(address, (double)(object)data);
        if (type == typeof(string))
            return deviceCommunication.Write(address, (string)(object)data);

        return new OperateResult
        {
            IsSuccess = false,
            Message = $"不支持的类型 {type}"
        };
    }
}