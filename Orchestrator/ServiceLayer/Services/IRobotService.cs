using Modbus.Types;
using ServiceLayer.Types;

namespace ServiceLayer.Services;

public interface IRobotService
{
    Task<ModbusResponse> WriteToDisplay(WriteToDisplayMessage message);
    Task<ModbusResponse<ReadColorSensorMessage>> ReadColorSensorData();
    Task<ModbusResponse> SetServoPos(SetServoPositionsMessage message);
    Task<ModbusResponse> SetServoProgressions(SetServoProgressionsMessage message);
    Task<ModbusResponse> MoveBelt(MoveBeltMessage message);
    Task<ModbusResponse> MoveBeltSteps(MoveBeltStepsMessage message);
    Task<ModbusResponse> MoveBelt(MoveBeltContinuousMessage message);
    Task<ModbusResponse<ReadMotorStateMessage>> IsMotorMoving();
    Task<ModbusResponse<ReadDepthSensorMessage>> ReadDepthSensorMessage();
    Task<ModbusResponse> ToggleReportTimes();
    Task<ModbusResponse<ReadStatusMessage>> ReadStatusAsync();
}