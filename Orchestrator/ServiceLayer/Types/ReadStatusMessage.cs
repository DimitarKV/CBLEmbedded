using Modbus.Types.Interfaces;

namespace ServiceLayer.Types;

public class ReadStatusMessage : IModbusSerializable, IModbusDeserializable<ReadStatusMessage>
{
    public byte Function { get; set; } = 5;
    public byte Status { get; set; }
    public byte[] toByteArray()
    {
        return new[] { Function };
    }

    public ReadStatusMessage fromByteArray(byte[] data)
    {
        Status = data[0];
        return this;
    }
}