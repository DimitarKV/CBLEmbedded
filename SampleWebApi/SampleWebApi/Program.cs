using SampleWebApi;
using SampleWebApi.Services.Interfaces;

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

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
