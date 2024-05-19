namespace Modbus.Types.Interfaces;

public interface IModbusDeserializable<T>
{
    T fromByteArray(byte[] data);
}