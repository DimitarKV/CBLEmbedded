using Modbus.Types.Interfaces;

namespace Modbus.Connectors;

/// <summary>
/// Used to operate an Adapted ModbusASCII Protocol
/// (AMAP) enabled device over a UART port. 
/// </summary>
public interface IModbusConnector
{
    /// <summary>
    /// Constructs an AMAP message with
    /// <paramref name="function"/> and <paramref name="data"/> and sends it over the port
    /// to the device asynchronously with timeouts and retries specified in the configuration
    /// of the project.
    ///
    /// If the send operation times out or is not successful the method returns
    /// false. 
    /// </summary>
    /// <param name="function">A unique operation code</param>
    /// <param name="data">To send in the packet</param>
    /// <returns>Whether the operation was successful</returns>
    Task<string> SendModbusMessageAsync(IModbusSerializable serializable);
    Task<byte[]> ReadModbusMessageAsync(IModbusSerializable serializable);
    void PurgeBuffers();
    void Read();
    bool CanRead();
    void Close();
}