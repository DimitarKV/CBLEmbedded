using System.IO.Ports;
using SampleWebApi.Services.Interfaces;

namespace SampleWebApi.Services;

public class SerialCommunication : ISerialCommunication
{
    public SerialPort Sp { get; set; }

    public SerialCommunication()
    {
    }

    public void Initialize(string port, int baudrate)
    {
        Sp = new SerialPort(port, baudrate, Parity.None, 8, StopBits.One);
    }

    public void Open()
    {
        Sp.Open();
        Console.WriteLine("Serial port open!");
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