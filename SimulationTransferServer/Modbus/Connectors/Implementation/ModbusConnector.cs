using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Modbus.Configuration;

namespace Modbus.Connectors.Implementation;


public class ModbusConnector : PortConnector, IModbusConnector
{
    private int _retries;
    private int _timeout;
    private readonly ILogger<ModbusConnector> _logger;
    
    public ModbusConnector(IOptions<SerialPortOptions> options, ILogger<ModbusConnector> logger) : base(options)
    {
        _logger = logger;
        _retries = options.Value.Retries;
        _timeout = options.Value.Timeout;
        OpenAndPurge();
    }
    
    public async Task<bool> SendModbusMessageAsync(byte function, byte[] data)
    {
        byte[] funcDataChunk = new byte[data.Length + 1];
        funcDataChunk[0] = function;
        Array.Copy(data, 0, funcDataChunk, 1, data.Length);
        var start = ":";
        var funcDataChunkHex = ConvertToDataChunk(funcDataChunk);
        var lrcHex = CalculateLrc(funcDataChunk).ToString("x2");
        var end = "\r\n";
        var message = start + funcDataChunkHex + lrcHex + end;
        return await TrySendAsync(message);
    }

    public async Task<byte[]> ReadModbusMessageAsync()
    {
        string modbusChunk = await ReadCrLfLineFromStreamAsync();
        string dataChunk = modbusChunk.Substring(3, modbusChunk.Length - 5);
        byte[] buffer = new byte[dataChunk.Length/2];
        for (int i = 0; i < dataChunk.Length; i+=2)
        {
            buffer[i / 2] = byte.Parse(dataChunk.Substring(i, 2), NumberStyles.HexNumber);
        }
        return buffer;
    }

    public void Read()
    {
        while (true)
        {
            if (BytesToRead != 0)
            {
                Console.Write((char)ReadByte());
            }
            
        }
    }

    public bool CanRead()
    {
        throw new NotImplementedException();
    }

    private string ConvertToDataChunk(byte[] data)
    {
        var chunk = "";
        foreach (var b in data) chunk += ((int)b).ToString("x2");

        return chunk;
    }
    
    private int CalculateLrc(byte[] data)
    {
        var calculatedLrc = 0;
        foreach (char b in data) calculatedLrc = (calculatedLrc + b) & 0xFF;

        calculatedLrc = ((calculatedLrc ^ 0xFF) + 1) & 0xFF;
        return calculatedLrc;
    }

    private async Task<bool> TrySendAsync(string data)
    {
        for (var i = 0; i < _retries + 1; i++)
        {
            string ack = "";
            Write(data);
            try
            {
                ack = await ReadCrLfLineFromStreamAsync();
            }
            catch (TimeoutException e)
            {
                _logger.LogInformation("ACK packet receive timed out, retrying!");
                continue;
            }
            if (ack == "ACK") return true;
        }
        return false;
    }

    private async Task<int> ReadAsyncWithTimeout(byte[] buffer, int offset, int length)
    {
        Task<int> result = ReadFromStreamAsync(buffer, offset, length);

        await Task.WhenAny(result, Task.Delay(_timeout));

        if (!result.IsCompleted)
        {
            throw new TimeoutException();
        }

        return await result;
    }

    private async Task<string> ReadCrLfLineFromStreamAsync()
    {
        var data = "";

        while (true)
        {
            var c = new byte[1];

            await ReadAsyncWithTimeout(c, 0, 1);

            data += (char)c[0];
            if (data.Length > 1 && data[^2] == '\r' && data[^1] == '\n') return data.Substring(0, data.Length - 2);
        }
    }
}