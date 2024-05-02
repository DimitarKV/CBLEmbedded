using SimulationTransferServer;
using SimulationTransferServer.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

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

var sp = app.Services.GetService<ISerialCommunication>();
sp.Initialize(builder.Configuration["SerialPort:Port"], int.Parse(builder.Configuration["SerialPort:Baudrate"]));
sp.Open();
//Purge receive buffer of Arduino after opening since sp.Open() sends "dddd" after initialization, which messes up Arduino logic
sp.GetSerialPort().Write(":?:?aAbBcC\r\n");

var peripheral = app.Services.GetService<IPeripheralCommunication>();
peripheral.WriteToDisplay("Welcome to our sweet robot, feel free to test it's abilities!");

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
sp.Close();