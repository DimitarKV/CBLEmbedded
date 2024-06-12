using Modbus.Connectors;
using Modbus.Types;
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

    public async Task<ModbusResponse> WriteToDisplay(WriteToDisplayMessage message)
    {
        _modbusConnector.PurgeBuffers();
        return await _modbusConnector
            .SendModbusMessageAsync(message);;
    }

    public async Task<ModbusResponse<ReadColorSensorMessage>> ReadColorSensorData()
    {
        _modbusConnector.PurgeBuffers();
        return await _modbusConnector.ReadModbusMessageAsync(new ReadColorSensorMessage());
    }

    public async Task<ModbusResponse> SetServoPos(SetServoPositionsMessage message)
    {
        return await _modbusConnector.SendModbusMessageAsync(message);
    }

    public async Task<ModbusResponse> SetServoProgressions(SetServoProgressionsMessage message)
    {
        return await _modbusConnector.SendModbusMessageAsync(message);
    }

    public async Task<ModbusResponse> MoveBelt(MoveBeltMessage message)
    {
        return await _modbusConnector.SendModbusMessageAsync(message);
    }

    public async Task<ModbusResponse> MoveBeltSteps(MoveBeltStepsMessage message)
    {
        return await _modbusConnector.SendModbusMessageAsync(message);
    }

    public async Task<ModbusResponse> MoveBelt(MoveBeltContinuousMessage message)
    {
        return await _modbusConnector.SendModbusMessageAsync(message);
    }

    public async Task<ModbusResponse<ReadMotorStateMessage>> IsMotorMoving()
    {
        return await _modbusConnector.ReadModbusMessageAsync(new ReadMotorStateMessage());
    }

    public async Task<ModbusResponse<ReadDepthSensorMessage>> ReadDepthSensorMessage()
    {
        _modbusConnector.PurgeBuffers();
        return await _modbusConnector.ReadModbusMessageAsync(new ReadDepthSensorMessage());
    }

    public async Task<ModbusResponse> ToggleReportTimes()
    {
        return await _modbusConnector.SendModbusMessageAsync(new ToggleReportTimesMessage());
    }

    public async Task<ModbusResponse<ReadStatusMessage>> ReadStatusAsync()
    {
        return await _modbusConnector.ReadModbusMessageAsync(new ReadStatusMessage());
    }

    public async Task<ModbusResponse> SetLedStatesAsync(SetLedStatesMessage message)
    {
        return await _modbusConnector.SendModbusMessageAsync(message);
    }
}