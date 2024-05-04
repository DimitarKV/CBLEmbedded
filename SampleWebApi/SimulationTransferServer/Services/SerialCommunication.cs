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
        //Purge receive buffer of Arduino after opening since sp.Open() sends "dddd" after initialization, which messes up Arduino logic
        Sp.Write(":?:?aAbBcC\r\n");
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