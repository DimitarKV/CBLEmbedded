namespace SimulationTransferServer.Services;

public interface IRobotService
{
    Task<bool> WriteToDisplay(string text);
}