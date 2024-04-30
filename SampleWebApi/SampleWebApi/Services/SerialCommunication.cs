using System.IO.Ports;
using SampleWebApi.Services.Interfaces;

namespace SampleWebApi.Services;

public class SerialCommunication : ISerialCommunication
{
    public SerialPort Sp { get; set; }

    public SerialCommunication()
    {
        Sp = new SerialPort("COM5", 115200, Parity.None, 8, StopBits.One);
    }

    public void Open()
    {
        Sp.Open();
    }

    public void Close()
    {
        Sp.Close();
    }

    public SerialPort GetSerialPort()
    {
        return Sp;
    }
}