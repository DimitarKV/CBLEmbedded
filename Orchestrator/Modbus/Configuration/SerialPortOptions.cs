namespace Modbus.Configuration;

public class SerialPortOptions
{
    public const string SerialPort = "SerialPort";
    
    public string Port { get; set; }
    public int BaudRate { get; set; }
    public int Retries { get; set; }
    public int Timeout { get; set; }
}