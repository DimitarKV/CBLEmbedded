using SimulationTransferServer.Services.Interfaces;

namespace SimulationTransferServer.Services;

public class PeripheralCommunication(ISerialCommunication communication) : IPeripheralCommunication
{
    private ISerialCommunication _communication = communication;

    public void WriteToDisplay(string text)
    {
        _communication.GetSerialPort().Write("WDN" + text + "\r\n");
    }

    public void WriteToDisplayScrolling(string text)
    {
        _communication.GetSerialPort().Write("WDS" + text + "\r\n");
    }
}