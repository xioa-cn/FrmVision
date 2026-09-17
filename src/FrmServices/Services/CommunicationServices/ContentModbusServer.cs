using System;
using FrmMapper.Data;
using FrmServices.Communication.Extensions;
using FrmServices.Services.CommunicationServices.Services;
using HslCommunication;
using HslCommunication.Core;
using HslCommunication.ModBus;

namespace FrmServices.Services.CommunicationServices;

public abstract class ContentModbusServer : IContentNodeServer
{
    private readonly object _syncRoot = new object();
    private bool _isStarted;
    private bool _disposed;

    protected ContentModbusServer(byte station, DataFormat dataFormat)
    {
        Server = new ModbusTcpServer
        {
            EnableWrite = true,
            EnableIPv6 = false,
            Station = station,
            StationDataIsolation = false,
            UseModbusRtuOverTcp = false,
            IsStringReverse = false,
            DataFormat = dataFormat,
            ActiveTimeSpan = TimeSpan.FromHours(1)
        };
    }

    protected ModbusTcpServer Server { get; }

    public bool IsStarted
    {
        get { lock (_syncRoot) return _isStarted; }
    }

    public Result Start()
    {
        lock (_syncRoot)
        {
            if (_disposed) return Result.Fail("The Modbus server has been disposed.");
            if (_isStarted) return Result.Ok();

            try
            {
                var result = StartCore();
                _isStarted = result.IsSuccess;
                if (!_isStarted) StopCore();
                return result;
            }
            catch (Exception ex)
            {
                StopCore();
                return Result.Fail("Failed to start the Modbus server: " + ex.Message);
            }
        }
    }

    public void Stop()
    {
        lock (_syncRoot)
        {
            if (_disposed || !_isStarted) return;
            StopCore();
            _isStarted = false;
        }
    }

    // The local data area remains accessible before Start and after Stop.
    public Result<T[]> Read<T>(string adress, short length)
    {
        lock (_syncRoot)
        {
            if (_disposed) return ReadFailure<T>("The Modbus server has been disposed.");
            if (string.IsNullOrWhiteSpace(adress)) return ReadFailure<T>("Address is required.");
            if (length <= 0) return ReadFailure<T>("Length must be greater than zero.");

            try
            {
                var result = Server.ReadAnyType<T>(adress, (ushort)length);
                return new Result<T[]>
                {
                    IsSuccess = result.IsSuccess,
                    Message = result.Message,
                    Data = result.IsSuccess ? result.Content : null
                };
            }
            catch (Exception ex)
            {
                return ReadFailure<T>(ex.Message);
            }
        }
    }

    public Result Write<T>(string adress, T value)
    {
        lock (_syncRoot)
        {
            if (_disposed) return Result.Fail("The Modbus server has been disposed.");
            if (string.IsNullOrWhiteSpace(adress)) return Result.Fail("Address is required.");
            if (value is null) return Result.Fail("Value is required.");
            if (value is Array array && array.Length == 0) return Result.Fail("Value cannot be an empty array.");
            if (value is byte[] bytes && bytes.Length % 2 != 0)
                return Result.Fail("Raw register data must contain an even number of bytes.");

            try
            {
                OperateResult result;
                switch (value)
                {
                    case bool[] values: result = Server.Write(adress, values); break;
                    case byte[] values: result = Server.Write(adress, values); break;
                    case short[] values: result = Server.Write(adress, values); break;
                    case ushort[] values: result = Server.Write(adress, values); break;
                    case int[] values: result = Server.Write(adress, values); break;
                    case uint[] values: result = Server.Write(adress, values); break;
                    case long[] values: result = Server.Write(adress, values); break;
                    case ulong[] values: result = Server.Write(adress, values); break;
                    case float[] values: result = Server.Write(adress, values); break;
                    case double[] values: result = Server.Write(adress, values); break;
                    default: result = Server.WriteAny(adress, value); break;
                }

                return Result.Ok(result.IsSuccess, result.Message);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
    }

    protected abstract Result StartCore();
    protected abstract void StopCore();

    public void Dispose()
    {
        lock (_syncRoot)
        {
            if (_disposed) return;
            _disposed = true;
            _isStarted = false;
            try { StopCore(); }
            finally { Server.Dispose(); }
        }
    }

    private static Result<T[]> ReadFailure<T>(string message)
    {
        return new Result<T[]> { IsSuccess = false, Message = message };
    }
}
