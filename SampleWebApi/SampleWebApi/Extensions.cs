using SampleWebApi.Services;
using SampleWebApi.Services.Interfaces;

namespace SampleWebApi;

public static class Extensions
{
    public static void AddExtensions(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ISerialCommunication, SerialCommunication>();
    }
}