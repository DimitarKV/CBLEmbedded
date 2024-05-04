using SimulationTransferServer.Services.Interfaces;

namespace SimulationTransferServer.Services;

public class PeripheralCommunication(ISerialCommunication communication) : IPeripheralCommunication
{
    private int CalculateLrc(string input)
    {
        int calculatedLrc = 0;
        foreach (char b in input)
        {
            calculatedLrc = (calculatedLrc + b) & 0xFF;
        }

        calculatedLrc = (((calculatedLrc ^ 0xFF) + 1) & 0xFF);
        return calculatedLrc;
    }

    private string StringToDataChunk(string input)
    {
        string data = "";
        foreach (char letter in input)
        {
            data += ((int)letter).ToString("x2");
        }

        return data;
    }

    public void WriteToDisplay(string text)
    {
        string start = ":";
        string function = "00";
        string dataChunk = StringToDataChunk(text);
        string lrc = CalculateLrc(function + dataChunk).ToString("x2");
        string end = "\r\n";
        communication.GetSerialPort().Write(start + function + dataChunk + lrc + end);
    }

    public void WriteToDisplayScrolling(string text)
    {
        communication.GetSerialPort().Write("WDS" + text + "\r\n");
    }
}