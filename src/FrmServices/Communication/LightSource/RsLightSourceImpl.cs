using System;
using System.IO.Ports;
using FrmMapper.Data;
using FrmServices.LogServices;

namespace FrmServices.Communication.LightSource;

public class RsLightSourceImpl : ILightSourceService, IDisposable
{
    private readonly SerialPort _serialPort;

    public RsLightSourceImpl(
        string portName,
        int baudRate = 9600,
        Parity parity = Parity.None,
        int dataBits = 8,
        StopBits stopBits = StopBits.One)
    {
        if (string.IsNullOrWhiteSpace(portName))
            throw new ArgumentException("串口号不能为空。", nameof(portName));
        if (baudRate <= 0)
            throw new ArgumentOutOfRangeException(nameof(baudRate), "波特率必须大于 0。");
        if (dataBits <= 0)
            throw new ArgumentOutOfRangeException(nameof(dataBits), "数据位必须大于 0。");

        _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
        {
            ReadTimeout = 1000,
            WriteTimeout = 1000
        };
        _serialPort.DataReceived += SerialPortOnDataReceived;
    }

    public event Action<string> DataReceived;

    public bool IsConnected => _serialPort.IsOpen;

    public Result Send(string message)
    {
        if (string.IsNullOrEmpty(message))
            return Result.Fail("发送内容不能为空");
        if (!IsConnected)
            return Result.Fail("串口未连接");

        try
        {
            _serialPort.Write(message);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail("串口发送失败：" + ex.Message);
        }
    }

    public Result Connect()
    {
        if (IsConnected)
            return Result.Ok();

        try
        {
            _serialPort.Open();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail("串口连接失败：" + ex.Message);
        }
    }

    public Result KeepAlive()
    {
        return IsConnected
            ? Result.Ok()
            : Result.Fail("串口未连接");
    }

    private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            var data = _serialPort.ReadExisting();
            if (!string.IsNullOrEmpty(data))
                DataReceived?.Invoke(data);
        }
        catch (Exception ex)
        {
            AppLog.Error("串口数据接收失败" + Environment.NewLine + ex,
                nameof(RsLightSourceImpl));
        }
    }

    public void Dispose()
    {
        _serialPort.DataReceived -= SerialPortOnDataReceived;
        if (_serialPort.IsOpen)
            _serialPort.Close();
        _serialPort.Dispose();
    }
}
