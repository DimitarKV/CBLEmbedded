using SimulationTransferServer.Services;
using SimulationTransferServer.Services.Interfaces;

namespace SimulationTransferServer.Extensions;

public static class Extensions
{
    public static void AddExtensions(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ISerialCommunication, SerialCommunication>();
        builder.Services.AddSingleton<IPeripheralCommunication, PeripheralCommunication>();
    }
}