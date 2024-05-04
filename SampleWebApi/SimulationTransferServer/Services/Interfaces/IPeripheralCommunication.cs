namespace SimulationTransferServer.Services.Interfaces;

public interface IPeripheralCommunication
{
    void Initialize(string port, int baudRate);
    void Open();
    void WriteToDisplay(string text);
    void WriteToDisplayScrolling(string text);
    void Close();
}