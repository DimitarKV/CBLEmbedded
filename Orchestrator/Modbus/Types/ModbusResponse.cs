using Modbus.Types.Interfaces;

namespace Modbus.Types;

public class ModbusResponse
{
    
    public byte? DeviceStatus { get; set; }
    public bool Success { get; set; }
}

public class ModbusResponse<T> : ModbusResponse where T : IModbusDeserializable<T>
{
    public T? Data { get; set; }
}