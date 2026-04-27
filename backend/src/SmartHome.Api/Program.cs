using SmartHome.Domain.Factories;
using SmartHome.Domain.Services;
using SmartHome.Domain;
using SmartHome.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDeviceService, DeviceService>();
builder.Services.AddSingleton<IDeviceFactory, DeviceFactory>();
builder.Services.AddSingleton<ICommandFactory, CommandFactory>();
builder.Services.AddSingleton<IDeviceRepository, JsonDeviceRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();