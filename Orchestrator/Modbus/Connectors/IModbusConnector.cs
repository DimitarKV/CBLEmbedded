using Modbus.Types;
using Modbus.Types.Interfaces;

namespace Modbus.Connectors;

/// <summary>
/// Creates another layer of abstraction over the Serial protocol used by both ends - ESP-32 and RaspberryPI.
/// Works on the request-response module and integrates a checksum packet validator.
/// Has configurable timeouts and retries.
/// </summary>
public interface IModbusConnector
{
    Task<ModbusResponse> SendModbusMessageAsync(IModbusSerializable serializable);

    Task<ModbusResponse<T>> ReadModbusMessageAsync<T>(T serializable)
        where T : IModbusSerializable, IModbusDeserializable<T>, new();
    void PurgeBuffers();
    void Read();
    bool CanRead();
    void Close();
}