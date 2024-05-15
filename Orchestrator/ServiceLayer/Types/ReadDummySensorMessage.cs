using Modbus.Types.Interfaces;

namespace ServiceLayer.Types;

public class ReadDummySensorMessage : IModbusSerializable, IModbusDeserializable<ReadDummySensorMessage>
{
    public byte Function { get; set; }
    public int Value { get; set; }

    public ReadDummySensorMessage(byte function = 2)
    {
        Function = function;
    }

    public byte[] toByteArray()
    {
        List<byte> data = new List<byte>();
        data.Insert(0, Function);
        int valueCopy = Value;
        while (valueCopy != 0)
        {
            data.Insert(0, (byte)(valueCopy & 0xFF));
            valueCopy >>= 8;
        }

        return data.ToArray();
    }

    public ReadDummySensorMessage fromByteArray(byte[] data)
    {
        Value = 0;
        for (int i = 0; i < data.Length; i++)
        {
            Value <<= 8;
            Value += data[i];
        }
        return this;
    }
}