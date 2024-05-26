using ServiceLayer.Types;

namespace ServiceLayer.Services;

public interface IRobotService
{
    Task<bool> WriteToDisplay(WriteToDisplayMessage message);
    Task<ReadDummySensorMessage> ReadDummySensor(ReadDummySensorMessage message);
    Task SetServoPos(SetServoPositionsMessage message);
}