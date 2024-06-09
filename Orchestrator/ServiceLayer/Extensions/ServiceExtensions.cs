using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ServiceLayer.Services;
using ServiceLayer.Services.Implementation;

namespace ServiceLayer.Extensions;

public static class ServiceExtensions
{
    public static void AddServiceExtensions(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IRobotService, RobotService>();

    }
}