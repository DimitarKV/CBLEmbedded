using Modbus.Types.Interfaces;

namespace ServiceLayer.Types;

public class MoveBeltStepsMessage : IModbusSerializable
{
    public byte Function { get; set; } = 8;
    public int Steps { get; set; }
    public byte[] toByteArray()
    {
        return new[] { Function, (byte)(Steps & 0xFF), (byte)((Steps >> 8) & 0xFF) };
    }
}