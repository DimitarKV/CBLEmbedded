namespace SimulationTransferServer.Connectors;

public interface IModbusConnector
{
    Task<bool> SendModbusMessageAsync(byte function, byte[] data);
    Task<T> ReadModbusMessageAsync<T>() where T : new();
    void Read();
    bool CanRead();
}