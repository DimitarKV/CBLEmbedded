using Modbus.Types.Interfaces;

namespace ServiceLayer.Types;

public class SetLedStatesMessage : IModbusSerializable
{
    public byte Function { get; set; } = 10;
    public bool[] States { get; set; } = new bool[8];
    
    public byte[] toByteArray()
    {
        byte result = 0;
        for (int i = 0; i < 8; i++)
        {
            result += (byte)((byte)(States[i] ? 1 : 0) * ((byte)Math.Pow(2, i)));
        }

        return new[] { Function, result };
    }
}