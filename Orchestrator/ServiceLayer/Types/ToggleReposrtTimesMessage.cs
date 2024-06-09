using Modbus.Types.Interfaces;

namespace ServiceLayer.Types;

public class ToggleReportTimesMessage : IModbusSerializable
{
    public byte Function { get; set; } = 5;
    public byte[] toByteArray()
    {
        return new[]{ Function };
    }
}