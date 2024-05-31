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

    public void Deconstruct(out IModbusConnector modbusConnector)
    {
        _modbusConnector.PurgeBuffers();
        _modbusConnector.Close();
        modbusConnector = _modbusConnector;
    }

    public async Task<bool> WriteToDisplay(WriteToDisplayMessage message)
    {
        _modbusConnector.PurgeBuffers();
        return await _modbusConnector
            .SendModbusMessageAsync(message);
    }

    public async Task<ReadColorSensorMessage> ReadColorSensorData()
    {
        _modbusConnector.PurgeBuffers();
        await _modbusConnector.SendModbusMessageAsync(new ReadColorSensorMessage());
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

    public async Task<ReadDepthSensorMessage> ReadDepthSensorMessage()
    {
        _modbusConnector.PurgeBuffers();
        await _modbusConnector.SendModbusMessageAsync(new ReadDepthSensorMessage());
        return new ReadDepthSensorMessage().fromByteArray(await _modbusConnector.ReadModbusMessageAsync());
    }
}