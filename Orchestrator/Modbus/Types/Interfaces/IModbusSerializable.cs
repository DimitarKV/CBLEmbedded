namespace Modbus.Types.Interfaces;

public interface IModbusSerializable
{
    byte[] toByteArray();
}