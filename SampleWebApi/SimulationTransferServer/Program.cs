using SimulationTransferServer;
using SimulationTransferServer.Extensions;
using SimulationTransferServer.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.AddExtensions();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var peripheralCommunication = app.Services.GetService<IPeripheralCommunication>();
peripheralCommunication!.Initialize(builder.Configuration["SerialPort:Port"]!, int.Parse(builder.Configuration["SerialPort:BaudRate"]!));
peripheralCommunication.Open();


var peripheral = app.Services.GetService<IPeripheralCommunication>();
peripheral!.WriteToDisplay(" Welcome to our   sweet robot!");

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
peripheral.Close();