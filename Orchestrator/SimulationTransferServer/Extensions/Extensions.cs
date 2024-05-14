using Modbus.Configuration;
using ServiceLayer.Services;
using ServiceLayer.Services.Implementation;

namespace SimulationTransferServer.Extensions;

public static class Extensions
{
    public static void AddExtensions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<SerialPortOptions>(builder.Configuration.GetSection(SerialPortOptions.SerialPort));
        builder.Services.AddSingleton<IRobotService, RobotService>();

    }
}