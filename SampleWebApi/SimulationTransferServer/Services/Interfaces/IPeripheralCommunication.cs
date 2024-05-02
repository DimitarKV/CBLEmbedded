namespace SimulationTransferServer.Services.Interfaces;

public interface IPeripheralCommunication
{
    void WriteToDisplay(string text);
    void WriteToDisplayScrolling(string text);
}