using SmartHome.Domain;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Simulations;
using SmartHome.Domain.Locations;
using SmartHome.Infrastructure;
using FluentValidation.AspNetCore;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDeviceService, DeviceService>();
builder.Services.AddSingleton<ISimulationService, SimulationService>();
builder.Services.AddSingleton<IDeviceFactory, DeviceFactory>();
builder.Services.AddSingleton<ICommandFactory, CommandFactory>();
builder.Services.AddSingleton<IDeviceRepository, JsonRepository>();
builder.Services.AddSingleton<ILocationRepository, JsonRepository>();

builder.Services.AddCors(options =>
{
    // TODO - Amber: Tighten CORS when frontend local host is defined; JWT implementation?
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowFrontend");
app.MapControllers();
app.Run();