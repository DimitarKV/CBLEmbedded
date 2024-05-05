using SimulationTransferServer.Connectors;

namespace SimulationTransferServer.Services.Implementation;

public class RobotService : IRobotService
{
    private readonly IModbusConnector _modbusConnector;

    public RobotService(IModbusConnector modbusConnector)
    {
        _modbusConnector = modbusConnector;
    }

    public async Task<bool> WriteToDisplay(string text)
    {
        return await _modbusConnector
            .SendModbusMessageAsync(0, text.Select(c => (byte)c)
            .ToArray());
    }
}