using System.IO.Ports;

namespace SampleWebApi.Services.Interfaces;

public interface ISerialCommunication
{
    void Initialize(string port, int baudrate);
    void Open();
    void Close();
    SerialPort GetSerialPort();
}