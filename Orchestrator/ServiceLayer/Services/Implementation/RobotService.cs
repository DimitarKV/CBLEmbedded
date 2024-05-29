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

    public async Task<ReadColorSensorMessage> ReadColorSensorData(ReadColorSensorMessage message)
    {
        _modbusConnector.PurgeBuffer();
        await _modbusConnector.SendModbusMessageAsync(message);
        return new ReadColorSensorMessage().fromByteArray(await _modbusConnector.ReadModbusMessageAsync());
    }

    public async Task SetServoPos(SetServoPositionsMessage message)
    {
        await _modbusConnector.SendModbusMessageAsync(message);
    }

    public async Task MoveBelt(MoveBeltMessage message)
    {
        await _modbusConnector.SendModbusMessageAsync(message);
    }
}