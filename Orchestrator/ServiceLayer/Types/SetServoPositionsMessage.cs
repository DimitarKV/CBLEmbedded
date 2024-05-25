using Modbus.Types.Interfaces;

namespace ServiceLayer.Types;

public class SetServoPositionsMessage : IModbusSerializable, IModbusDeserializable<SetServoPositionMessage>
{
    public byte Function { get; set; } = 3;
    public List<ServoPosDto> ServoParameters { get; set; } = new();
    public byte[] toByteArray()
    {
        var packet = new byte[ServoParameters.Count * 2 + 1];
        packet[0] = Function;
        for (int i = 0; i < ServoParameters.Count; i++)
        {
            packet[2 * i + 1] = ServoParameters[i].Id;
            packet[2 * i + 2] = ServoParameters[i].Angle;
        }

        return packet;
    }

    public SetServoPositionMessage fromByteArray(byte[] data)
    {
        throw new NotImplementedException();
    }
}