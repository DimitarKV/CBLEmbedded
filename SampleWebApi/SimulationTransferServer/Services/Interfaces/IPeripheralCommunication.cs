namespace SimulationTransferServer.Services.Interfaces;

public interface IPeripheralCommunication
{
    void Initialize(string port, int baudRate);
    void Open();
    Task WriteToDisplay(string text);
    Task Close();
    void SetRetries(int retries);
    void SetReadTimeout(int readTimeout);
}