using System.IO.Ports;
using Microsoft.Extensions.Options;
using Modbus.Configuration;

namespace Modbus.Connectors.Implementation;

public class PortConnector : SerialPort
{
    public PortConnector(IOptions<SerialPortOptions> options)
    {
        PortName = options.Value.Port;
        BaudRate = options.Value.BaudRate;
        Parity = Parity.None;
        DataBits = 8;
        StopBits = StopBits.One;
    }

    public void OpenAndPurge()
    {
        Open();
        Write(":?:?aAbBcC\r\n");
    }

    public async Task<int> ReadFromStreamAsync(byte[] buffer, int offset, int count)
    {
        return await BaseStream.ReadAsync(buffer, offset, count);
    }
}