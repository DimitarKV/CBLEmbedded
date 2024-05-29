using System.Runtime.CompilerServices;
using Modbus.Types.Interfaces;

namespace ServiceLayer.Types;

public class MoveBeltMessage : IModbusSerializable, IModbusDeserializable<MoveBeltMessage>
{
    public byte Function { get; set; } = 2;
    public int Distance { get; set; }

    public byte[] toByteArray()
    {
        return new[] { Function, (byte)(Distance & 0xFF), (byte)((Distance >> 8) & 0xFF) };
    }

    public MoveBeltMessage fromByteArray(byte[] data)
    {
        throw new NotImplementedException();
    }
}