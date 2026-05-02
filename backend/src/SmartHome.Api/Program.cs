using SmartHome.Domain;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Simulations;
using SmartHome.Domain.Locations;
using SmartHome.Infrastructure;
using SmartHome.Api.Middleware;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Type = "https://httpstatuses.com/400",
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Instance = context.HttpContext.Request.Path
            };

            return new BadRequestObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json" }
            };
        };
    });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SmartHome API",
        Version = "v1",
        Description = "Smart home simulator REST API for device control, history, and environment simulation."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddScoped<IDeviceService, DeviceService>();

builder.Services.AddSingleton<IDeviceTypeFactory, LightDeviceFactory>();
builder.Services.AddSingleton<IDeviceTypeFactory, FanDeviceFactory>();
builder.Services.AddSingleton<IDeviceTypeFactory, ThermostatDeviceFactory>();
builder.Services.AddSingleton<IDeviceTypeFactory, DoorLockFactory>();

builder.Services.AddScoped<ICommandFactory, CommandFactory>();
builder.Services.AddScoped<IThermostatModeStrategyFactory, ThermostatStrategyFactory>();

builder.Services.AddScoped<JsonRepository>();
builder.Services.AddScoped<IDeviceRepository>(sp => sp.GetRequiredService<JsonRepository>());
builder.Services.AddScoped<ILocationRepository>(sp => sp.GetRequiredService<JsonRepository>());

builder.Services.AddSingleton<ISimulationService, SimulationService>();

builder.Services.AddScoped<IDeviceFactory, DeviceFactory>();

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

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<GlobalErrorHandling>();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();

app.MapControllers();
app.Run();
