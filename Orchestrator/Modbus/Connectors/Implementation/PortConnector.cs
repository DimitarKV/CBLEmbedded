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

    public async Task<byte> ReadByteFromStreamAsync()
    {
        Task waitBytesOnBuffer = new Task(() =>
        {
            while (BytesToRead <= 0)
            {
            }
        });
        waitBytesOnBuffer.Start();
        await waitBytesOnBuffer;
        
        byte[] buffer = new byte[1];
        await BaseStream.ReadAsync(buffer, 0, 1);
        return buffer[0];
    }
}