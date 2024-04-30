using System.IO.Ports;

namespace SampleWebApi.Services.Interfaces;

public interface ISerialCommunication
{
    void Open();
    void Close();
    SerialPort GetSerialPort();
}