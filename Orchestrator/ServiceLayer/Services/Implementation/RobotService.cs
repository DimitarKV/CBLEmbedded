using Modbus.Connectors;
using ServiceLayer.Types;

namespace ServiceLayer.Services.Implementation;

public class RobotService : IRobotService
{
    private readonly IModbusConnector _modbusConnector;

    public RobotService(IModbusConnector modbusConnector)
    {
        _modbusConnector = modbusConnector;
    }

    public async Task<bool> WriteToDisplay(WriteToDisplayMessage message)
    {
        return await _modbusConnector
            .SendModbusMessageAsync(message);
    }

    public async Task<ReadDummySensorMessage> ReadDummySensor(ReadDummySensorMessage message)
    {
        await _modbusConnector.SendModbusMessageAsync(message);
        return new ReadDummySensorMessage().fromByteArray(await _modbusConnector.ReadModbusMessageAsync());
    }

    public async Task SetServoPos(SetServoPositionMessage message)
    {
        await _modbusConnector.SendModbusMessageAsync(message);
    }
}