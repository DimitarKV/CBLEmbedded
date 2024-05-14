using Modbus.Types.Interfaces;

namespace SimulationTransferServer.Types;

public class SetServoPositionMessage : IModbusSerializable, IModbusDeserializable<SetServoPositionMessage>
{
    public byte Function { get; set; } = 3;
    public byte ServoID { get; set; }
    public byte Angle { get; set; }
    public byte[] toByteArray()
    {
        return new byte[] { Function, ServoID, (byte)(Angle & 0xFF) };
    }

    public SetServoPositionMessage fromByteArray(byte[] data)
    {
        Function = data[0];
        ServoID = data[1];
        Angle = data[2];
        return this;
    }
}