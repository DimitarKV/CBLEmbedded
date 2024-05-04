using SimulationTransferServer.Services.Interfaces;

namespace SimulationTransferServer.Services;

public class PeripheralCommunication(ISerialCommunication communication, ILogger<PeripheralCommunication> logger) : IPeripheralCommunication
{
    private int CalculateLrc(byte function, byte[] input)
    {
        int calculatedLrc = 0;
        calculatedLrc = (calculatedLrc + function) & 0xFF;
        foreach (char b in input)
        {
            calculatedLrc = (calculatedLrc + b) & 0xFF;
        }

        calculatedLrc = (((calculatedLrc ^ 0xFF) + 1) & 0xFF);
        return calculatedLrc;
    }

    private string ConvertToDataChunk(byte[] data)
    {
        string chunk = "";
        foreach (byte b in data)
        {
            chunk += ((int)b).ToString("x2");
        }

        return chunk;
    }

    private bool SendModbusMessage(int function, byte[] data)
    {
        string start = ":";
        string functionHex = function.ToString("x2");
        string dataChunkHex = ConvertToDataChunk(data);
        string lrcHex = CalculateLrc((byte)function, data).ToString("x2");
        string end = "\r\n";
        communication.GetSerialPort().Write(start + functionHex + dataChunkHex + lrcHex + end);
        // TODO: Add ACK/NACK
        return true;
    }

    public void Initialize(string port, int baudRate)
    {
        communication.Initialize(port, baudRate);
    }

    public void Open()
    {
        communication.Open();
        
        logger.LogInformation("Communication started");
    }

    public void WriteToDisplay(string text)
    {
        SendModbusMessage(0, text.Select(l => (byte)l).ToArray());
    }

    public void WriteToDisplayScrolling(string text)
    {
        throw new NotImplementedException();
    }

    public void Close()
    {
        SendModbusMessage(1, new byte[] { });
        communication.Close();
        logger.LogInformation("Communication closed");
    }
}