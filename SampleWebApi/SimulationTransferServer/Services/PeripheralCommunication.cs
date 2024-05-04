using SimulationTransferServer.Services.Interfaces;

namespace SimulationTransferServer.Services;

public class PeripheralCommunication : IPeripheralCommunication
{
    private readonly ISerialCommunication _communication;
    private readonly ILogger<PeripheralCommunication> _logger;
    private int _retries;
    private int _readTimeout;

    public PeripheralCommunication(ISerialCommunication communication, ILogger<PeripheralCommunication> logger)
    {
        _communication = communication;
        _logger = logger;

        Initialize("COM5", 115200);
        SetRetries(3);
        _readTimeout = 2000;
    }

    public void Initialize(string port, int baudRate)
    {
        _communication.Initialize(port, baudRate);
    }

    public void Open()
    {
        _communication.Open();
        _communication.GetSerialPort().BaseStream.ReadTimeout = _readTimeout;
        _logger.LogInformation("Communication started");
    }

    public async Task WriteToDisplay(string text)
    {
        var success = await SendModbusMessage(0, text.Select(l => (byte)l).ToArray());
        if (success)
            _logger.LogInformation("Successfully sent packet!");
        else
            _logger.LogInformation("Unsuccessful packet!");
    }

    public async Task Close()
    {
        await SendModbusMessage(1, new byte[] { });
        _communication.Close();
        _logger.LogInformation("Communication closed");
    }

    public void SetRetries(int retries)
    {
        _retries = retries;
    }

    public void SetReadTimeout(int readTimeout)
    {
        _readTimeout = readTimeout;
    }

    private async Task<int> ReadAsyncWithTimeout(byte[] buffer, int offset, int length)
    {
        Task<int> result = _communication.GetSerialPort().BaseStream.ReadAsync(buffer, offset, length);

        await Task.WhenAny(result, Task.Delay(_readTimeout));

        if (!result.IsCompleted)
        {
            throw new TimeoutException();
        }

        return await result;
    }

    private async Task<string> ReadCrLfLineFromStreamAsync(Stream stream)
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

    private int CalculateLrc(byte function, byte[] input)
    {
        var calculatedLrc = 0;
        calculatedLrc = (calculatedLrc + function) & 0xFF;
        foreach (char b in input) calculatedLrc = (calculatedLrc + b) & 0xFF;

        calculatedLrc = ((calculatedLrc ^ 0xFF) + 1) & 0xFF;
        return calculatedLrc;
    }

    private string ConvertToDataChunk(byte[] data)
    {
        var chunk = "";
        foreach (var b in data) chunk += ((int)b).ToString("x2");

        return chunk;
    }

    private async Task<bool> SendModbusMessage(int function, byte[] data)
    {
        var start = ":";
        var functionHex = function.ToString("x2");
        var dataChunkHex = ConvertToDataChunk(data);
        var lrcHex = CalculateLrc((byte)function, data).ToString("x2");
        var end = "\r\n";

        for (var i = 0; i < _retries + 1; i++)
        {
            string ack = "";
            _communication.GetSerialPort().Write(start + functionHex + dataChunkHex + lrcHex + end);
            try
            {
                ack = await ReadCrLfLineFromStreamAsync(_communication.GetSerialPort().BaseStream);
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

    public void WriteToDisplayScrolling(string text)
    {
        throw new NotImplementedException();
    }
}