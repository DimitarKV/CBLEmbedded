using Modbus.Types.Interfaces;

namespace ServiceLayer.Types;

public class ReadMotorStateMessage : IModbusSerializable, IModbusDeserializable<ReadMotorStateMessage>
{
    public byte Function { get; set; } = 9;
    public bool isMoving { get; set; } = false;
    public byte[] toByteArray()
    {
        return new[] { Function };
    }

    public ReadMotorStateMessage fromByteArray(byte[] data)
    {
        isMoving = data[0] != 0;
        return this;
    }
}