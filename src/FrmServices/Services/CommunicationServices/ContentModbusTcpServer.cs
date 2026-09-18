using FrmMapper.Data;
using System;
using System.Net;
using HslCommunication.Core;

namespace FrmServices.Services.CommunicationServices;

// 作者：xioa
// 作者邮箱：1327916255@qq.com
public class ContentModbusTcpServer : ContentModbusServer
{
    public ContentModbusTcpServer(int port = 502, byte station = 1, DataFormat dataFormat = DataFormat.CDAB)
        : base(station, dataFormat)
    {
        if (port <= IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            throw new ArgumentOutOfRangeException(nameof(port));
        Port = port;
    }

    public int Port { get; }

    protected override Result StartCore()
    {
        Server.ServerStart(Port);
        return Result.Ok(Server.IsStarted);
    }

    protected override void StopCore() => Server.ServerClose();
}
