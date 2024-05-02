using System.IO.Ports;
using SimulationTransferServer.Services.Interfaces;

namespace SimulationTransferServer.Services;

public class SerialCommunication : ISerialCommunication
{
    public SerialPort Sp { get; set; }

    public void Initialize(string port, int baudrate)
    {
        Sp = new SerialPort(port, baudrate, Parity.None, 8, StopBits.One);
        Sp.Handshake = Handshake.None;
    }

    public void Open()
    {
        Sp.Open();
        Console.WriteLine("Serial port open!");
    }

    public void Close()
    {
        Sp.Close();
        Console.WriteLine("Serial port closed!");
    }

    public SerialPort GetSerialPort()
    {
        return Sp;
    }
}