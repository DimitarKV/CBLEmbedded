using Modbus.Extensions;
using SimulationTransferServer.Extensions;
using SimulationTransferServer.Services;
using SimulationTransferServer.Types;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.AddExtensions();
builder.AddModbusExtensions();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var robotService = app.Services.GetService<IRobotService>();
await robotService!.WriteToDisplay(new WriteToDisplayMessage(" Welcome to our   sweet robot!"));

app.UseHttpsRedirection();
app.MapControllers();
app.Run();