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
        await _modbusConnector
            .SendModbusMessageAsync(message);
        return true;
    }

    public async Task<ReadColorSensorMessage> ReadColorSensorData()
    {
        _modbusConnector.PurgeBuffers();
        return new ReadColorSensorMessage().fromByteArray(await _modbusConnector.ReadModbusMessageAsync(new ReadColorSensorMessage()));
    }

    public async Task SetServoPos(SetServoPositionsMessage message)
    {
        await _modbusConnector.SendModbusMessageAsync(message);
    }

    public async Task SetServoProgressions(SetServoProgressionsMessage message)
    {
        await _modbusConnector.SendModbusMessageAsync(message);
    }

    public async Task MoveBelt(MoveBeltMessage message)
    {
        await _modbusConnector.SendModbusMessageAsync(message);
    }

    public async Task MoveBeltSteps(MoveBeltStepsMessage message)
    {
        await _modbusConnector.SendModbusMessageAsync(message);
    }

    public async Task MoveBelt(MoveBeltContinuousMessage message)
    {
        await _modbusConnector.SendModbusMessageAsync(message);
    }

    public async Task<ReadMotorStateMessage> IsMotorMoving()
    {
        return new ReadMotorStateMessage().fromByteArray(await _modbusConnector.ReadModbusMessageAsync(new ReadMotorStateMessage()));
    }

    public async Task<ReadDepthSensorMessage> ReadDepthSensorMessage()
    {
        _modbusConnector.PurgeBuffers();
        return new ReadDepthSensorMessage().fromByteArray(await _modbusConnector.ReadModbusMessageAsync(new ReadDepthSensorMessage()));
    }

    public async Task ToggleReportTimes()
    {
        await _modbusConnector.SendModbusMessageAsync(new ToggleReportTimesMessage());
    }
}