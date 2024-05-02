using System.IO.Ports;

namespace SimulationTransferServer.Services.Interfaces;

public interface ISerialCommunication
{
    void Initialize(string port, int baudrate);
    void Open();
    void Close();
    SerialPort GetSerialPort();
}