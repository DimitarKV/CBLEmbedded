using Modbus.Types.Interfaces;

namespace ServiceLayer.Types;

public class ReadColorSensorMessage : IModbusSerializable, IModbusDeserializable<ReadColorSensorMessage>
{
    public byte Function { get; set; } = 1;
    public int Red { get; set; }
    public int Green { get; set; }
    public int Blue { get; set; }
    public int Clear { get; set; }
    public int ColorTemp { get; set; }
    public int Lux { get; set; }

    public byte[] toByteArray()
    {
        return new[] { Function };
    }

    public ReadColorSensorMessage fromByteArray(byte[] data)
    {
        Red = (data[1] << 8) + data[0];
        Green = (data[3] << 8) + data[2];
        Blue = (data[5] << 8) + data[4];
        Clear = (data[7] << 8) + data[6];
        ColorTemp = (data[9] << 8) + data[8];
        Lux = (data[11] << 8) + data[10];
        return this;
    }
}