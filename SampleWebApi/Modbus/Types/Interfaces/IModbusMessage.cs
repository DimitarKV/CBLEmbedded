namespace Modbus.Types.Interfaces;

public interface IModbusMessage<T>
{
    byte[] toByteArray();

    T fromByteArray(byte[] data);
}