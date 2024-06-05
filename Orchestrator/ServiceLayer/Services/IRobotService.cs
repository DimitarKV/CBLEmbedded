using ServiceLayer.Types;

namespace ServiceLayer.Services;

public interface IRobotService
{
    Task<bool> WriteToDisplay(WriteToDisplayMessage message);
    Task<ReadColorSensorMessage> ReadColorSensorData();
    Task SetServoPos(SetServoPositionsMessage message);
    Task SetServoProgressions(SetServoProgressionsMessage message);
    Task MoveBelt(MoveBeltMessage message);
    Task MoveBeltSteps(MoveBeltStepsMessage message);
    Task MoveBelt(MoveBeltContinuousMessage message);
    Task<ReadMotorStateMessage> IsMotorMoving();
    Task<ReadDepthSensorMessage> ReadDepthSensorMessage();
    Task ToggleReportTimes();
}