using Modbus.Connectors;
using SimulationTransferServer.Types;

namespace SimulationTransferServer.Services.Implementation;

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
            .SendModbusMessageAsync(message.Function, message.toByteArray());
    }

    public async Task<ReadDummySensorMessage> ReadDummySensor(ReadDummySensorMessage message)
    {
        await _modbusConnector.SendModbusMessageAsync(message.Function, new byte[] { });
        return new ReadDummySensorMessage().fromByteArray(await _modbusConnector.ReadModbusMessageAsync());
    }
}