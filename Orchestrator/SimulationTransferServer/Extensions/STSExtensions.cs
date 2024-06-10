using Modbus.Configuration;
using Modbus.Extensions;
using ServiceLayer.Extensions;

namespace SimulationTransferServer.Extensions;

public static class STSExtensions
{
    public static void AddSTSExtensions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<SerialPortOptions>(builder.Configuration.GetSection(SerialPortOptions.SerialPort));
        builder.AddModbusExtensions();
        builder.AddServiceExtensions();
    }
}