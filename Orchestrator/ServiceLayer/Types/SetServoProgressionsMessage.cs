using Modbus.Types.Interfaces;

namespace ServiceLayer.Types;

public class SetServoProgressionsMessage : IModbusSerializable
{
    public byte Function { get; set; } = 6;
    public List<ServoProgressionDto> Progressions { get; set; } = new List<ServoProgressionDto>();
    public byte[] toByteArray()
    {
        byte[] packet = new byte[Progressions.Count * 2 + 1];
        packet[0] = Function;
        for (int i = 0; i < Progressions.Count; i++)
        {
            packet[2 * i + 1] = Progressions[i].ServoId;
            packet[2 * i + 2] = Progressions[i].Progression;
        }
        return packet;
    }
}