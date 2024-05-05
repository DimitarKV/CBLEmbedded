using SimulationTransferServer.Types;

namespace SimulationTransferServer.Services;

public interface IRobotService
{
    Task<bool> WriteToDisplay(WriteToDisplayMessage message);
    Task<ReadDummySensorMessage> ReadDummySensor(ReadDummySensorMessage message);
}