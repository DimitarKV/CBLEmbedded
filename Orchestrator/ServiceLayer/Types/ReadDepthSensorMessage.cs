using Modbus.Connectors;
using Modbus.Types.Interfaces;

namespace ServiceLayer.Types;

public class ReadDepthSensorMessage : IModbusSerializable, IModbusDeserializable<ReadDepthSensorMessage>
{
    public byte Function { get; set; } = 4;
    public byte Range { get; set; }
    public byte[] toByteArray()
    {
        return new[] { Function, Range };
    }

    public ReadDepthSensorMessage fromByteArray(byte[] data)
    {
        if (data.Length < 1)
            throw new ArgumentException("Data chunk empty");
        Range = data[0];
        return this;
    }
}