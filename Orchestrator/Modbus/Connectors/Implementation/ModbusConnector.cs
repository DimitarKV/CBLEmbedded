using System.Diagnostics;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Modbus.Configuration;
using Modbus.Types;
using Modbus.Types.Interfaces;

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

    private async Task<byte[]?> TrySendAndReturnDataAsync(IModbusSerializable serializable)
    {
        byte[] funcDataChunk = serializable.toByteArray();
        var start = ":";
        var funcDataChunkHex = ConvertToDataChunk(funcDataChunk);
        var lrcHex = CalculateLrc(funcDataChunk).ToString("x2");
        var end = "\r\n";
        var message = start + funcDataChunkHex + lrcHex + end;
        var data = await TrySendAsync(message);
        return data;
    }
    
    public async Task<ModbusResponse> SendModbusMessageAsync(IModbusSerializable serializable)
    {
        var data = await TrySendAndReturnDataAsync(serializable);
        if (data is not null && data.Length >= 1)
        {
            try
            {
                return new ModbusResponse()
                    { DeviceStatus = data[0], Success = true };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new ModbusResponse()
                    { DeviceStatus = null, Success = false };
            }
        }

        return new ModbusResponse() { DeviceStatus = null, Success = false };
    }

    public async Task<ModbusResponse<T>> ReadModbusMessageAsync<T>(T serializable) where T : IModbusSerializable, IModbusDeserializable<T>, new()
    {
        var data = await TrySendAndReturnDataAsync(serializable);
        if (data is null || data.Length == 0)
        {
            return new ModbusResponse<T>() { Data = default, DeviceStatus = null, Success = false };
        }

        byte[] dataChunk = new byte[data.Length - 1];
        Array.Copy(data, 1, dataChunk, 0, dataChunk.Length);
        
        try
        {
            return new ModbusResponse<T>()
                { Data = new T().fromByteArray(dataChunk), DeviceStatus = data[0], Success = true };
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return new ModbusResponse<T>() { Data = default, DeviceStatus = null, Success = false };
        }
    }

    public void PurgeBuffers()
    {
        DiscardInBuffer();
        DiscardOutBuffer();
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
    
    private byte[] FromDataChunk(string data)
    {
        byte[] buffer = new byte[data.Length/2];
        for (int i = 0; i < data.Length; i+=2)
        {
            buffer[i / 2] = byte.Parse(data.Substring(i, 2), NumberStyles.HexNumber);
        }

        return buffer;
    }
    
    private int CalculateLrc(byte[] data)
    {
        var calculatedLrc = 0;
        foreach (char b in data) calculatedLrc = (calculatedLrc + b) & 0xFF;

        calculatedLrc = ((calculatedLrc ^ 0xFF) + 1) & 0xFF;
        return calculatedLrc;
    }

    private async Task<byte[]?> TrySendAsync(string data)
    {
        for (var i = 0; i < _retries + 1; i++)
        {
            string? ack = "";
            PurgeBuffers();
            var watch = Stopwatch.StartNew();
            Write(data);
            // await Task.Delay(100);
            try
            {
                ack = await ReadCrLfLineFromStreamAsync();
                // ack = ReadLine();
            }
            catch (TimeoutException e)
            {
                _logger.LogError("ACK packet receive timed out, retrying!");
                continue;
            }
            finally
            {
                watch.Stop();
                //_logger.LogInformation("Operation took: {0}ms", watch.Elapsed.Milliseconds);
            }
            //_logger.LogInformation("Received: {0}", ack);
            if (ack is not null && ack.Length >= 3 && ack.Substring(ack.Length - 3, 3) == "ACK")
            {
                var responseData = ack.Substring(0, ack.Length - 3);
                if (responseData.Length == 0)
                    return null;

                if (responseData.Length % 2 == 0)
                {
                    var response = FromDataChunk(responseData);
                    byte[] responseDataChunk = new byte[response.Length - 1];
                    Array.Copy(response, responseDataChunk, responseDataChunk.Length);
                    var lrc = CalculateLrc(responseDataChunk);
                    if (lrc == response[^1])
                    {
                        return responseDataChunk;
                    }
                }
            }
            _logger.LogError("ACK packet receive unsuccessful, retrying!");
        }

        return null;
    }

    private async Task<byte> ReadByteWithTimeoutAsync()
    {
        Task<byte> result = ReadByteFromStreamAsync();

        await Task.WhenAny(result, Task.Delay(_timeout));

        if (!result.IsCompleted)
        {
            throw new TimeoutException();
        }

        return result.Result;
    }

    private async Task<string?> ReadCrLfLineFromStreamAsync()
    {
        var data = "";
        // byte[] data = new byte[1024];
        // int index = 0;
        
        while (true)
        {
            data += (char)(await ReadByteWithTimeoutAsync());

            // index += await ReadToBufferAsync(data, index);
            if (data.Length > 1 && data[^2] == '\r' && data[^1] == '\n')
            {
                // return System.Text.Encoding.Default.GetString(data).Substring(0, index - 2);
                return data.Substring(1, data.Length - 3);
            }

        }
    }
}