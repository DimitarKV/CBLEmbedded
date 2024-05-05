namespace SimulationTransferServer.Connectors;

public interface IModbusConnector
{
    Task<bool> SendModbusMessageAsync(byte function, byte[] data);
    Task<byte[]> ReadModbusMessageAsync();
    bool CanRead();
}