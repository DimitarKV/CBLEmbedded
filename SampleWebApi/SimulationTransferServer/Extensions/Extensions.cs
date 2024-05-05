using SimulationTransferServer.Configuration;
using SimulationTransferServer.Connectors;
using SimulationTransferServer.Connectors.Implementation;
using SimulationTransferServer.Services;
using SimulationTransferServer.Services.Implementation;

namespace SimulationTransferServer.Extensions;

public static class Extensions
{
    public static void AddExtensions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<SerialPortOptions>(builder.Configuration.GetSection(SerialPortOptions.SerialPort));
        builder.Services.AddSingleton<IRobotService, RobotService>();
        builder.Services.AddSingleton<IModbusConnector, ModbusConnector>();

    }
}