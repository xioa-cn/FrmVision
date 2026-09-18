using System;
using System.IO.Ports;
using FrmMapper.Data;
using HslCommunication.Core;

namespace FrmServices.Services.CommunicationServices;

// 作者：xioa
// 作者邮箱：1327916255@qq.com
public class ContentModbusRtuServer : ContentModbusServer
{
    public ContentModbusRtuServer(string portName = "COM4", int baudRate = 9600,
        int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One,
        byte station = 1, DataFormat dataFormat = DataFormat.CDAB, int serialReceiveAtleastTime = 20)
        : base(station, dataFormat)
    {
        if (string.IsNullOrWhiteSpace(portName)) throw new ArgumentException("Port name is required.", nameof(portName));
        if (baudRate <= 0) throw new ArgumentOutOfRangeException(nameof(baudRate));
        if (dataBits < 5 || dataBits > 8) throw new ArgumentOutOfRangeException(nameof(dataBits));
        if (!Enum.IsDefined(typeof(Parity), parity)) throw new ArgumentOutOfRangeException(nameof(parity));
        if (!Enum.IsDefined(typeof(StopBits), stopBits) || stopBits == StopBits.None)
            throw new ArgumentOutOfRangeException(nameof(stopBits));
        if (serialReceiveAtleastTime < 0) throw new ArgumentOutOfRangeException(nameof(serialReceiveAtleastTime));

        PortName = portName.Trim();
        BaudRate = baudRate;
        DataBits = dataBits;
        Parity = parity;
        StopBits = stopBits;
        Server.SerialReceiveAtleastTime = serialReceiveAtleastTime;
    }

    public string PortName { get; }
    public int BaudRate { get; }
    public int DataBits { get; }
    public Parity Parity { get; }
    public StopBits StopBits { get; }

    protected override Result StartCore()
    {
        var result = Server.StartSerialSlave(PortName, BaudRate, DataBits, Parity, StopBits);
        return Result.Ok(result.IsSuccess, result.Message);
    }

    protected override void StopCore() => Server.CloseSerialSlave();
}
