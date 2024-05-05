namespace Modbus.Connectors;

public interface IModbusConnector
{
    Task<bool> SendModbusMessageAsync(byte function, byte[] data);
    Task<byte[]> ReadModbusMessageAsync();
    void Read();
    bool CanRead();
}