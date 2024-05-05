using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Modbus.Connectors;
using Modbus.Connectors.Implementation;

namespace Modbus.Extensions;

public static class AddModbus
{
    public static void AddModbusExtensions(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IModbusConnector , ModbusConnector>();
    }
}