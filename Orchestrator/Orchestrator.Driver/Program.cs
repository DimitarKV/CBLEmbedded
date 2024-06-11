using Modbus.Configuration;
using Modbus.Connectors;
using Modbus.Connectors.Implementation;
using NetCoreAudio;
using Orchestrator.Driver;
using ServiceLayer.Services;
using ServiceLayer.Services.Implementation;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IModbusConnector , ModbusConnector>();
builder.Services.AddSingleton<IRobotService, RobotService>();
builder.Services.AddTransient<Player>();
builder.Services.AddSingleton<IRobotSoundService, RobotSoundService>();
builder.Services.Configure<SerialPortOptions>(builder.Configuration.GetSection(SerialPortOptions.SerialPort));

Console.WriteLine(builder.Configuration.GetSection("RobotVariables:Motor:Speed").Value);

var host = builder.Build();
host.Run();