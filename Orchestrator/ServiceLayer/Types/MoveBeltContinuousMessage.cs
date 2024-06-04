using Modbus.Types.Interfaces;

namespace ServiceLayer.Types;

public class MoveBeltContinuousMessage : IModbusSerializable
{
    public byte Function { get; set; } = 6;
    public bool Running { get; set; } = false;
    public byte[] toByteArray()
    {
        return new[] { Function, Running ? (byte)1 : (byte)0 };
    }
}