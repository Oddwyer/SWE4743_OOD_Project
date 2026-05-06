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
using System.Text.Json.Serialization;
using System.Text.Json;
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

// Register shared simulation runtime services.
// These are singletons because ticker/runtime state should survive across requests.
builder.Services.AddSingleton<SimulationTicker>();
builder.Services.AddSingleton<SimulationRuntime>();

// Initializes runtime simulation state from persisted devices on startup.
builder.Services.AddScoped<SimulationInitializer>();

// Register application services.
builder.Services.AddScoped<ISimulationService, SimulationService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();

// Register device factories.
builder.Services.AddScoped<IDeviceTypeFactory, LightDeviceFactory>();
builder.Services.AddScoped<IDeviceTypeFactory, FanDeviceFactory>();
builder.Services.AddScoped<IDeviceTypeFactory, ThermostatDeviceFactory>();
builder.Services.AddScoped<IDeviceTypeFactory, DoorLockFactory>();
builder.Services.AddScoped<IDeviceFactory, DeviceFactory>();

// Register command and strategy factories.
builder.Services.AddScoped<IDeviceCommandFactory, CommandFactory>();
builder.Services.AddScoped<IThermostatModeStrategyFactory, ThermostatStrategyFactory>();

// Register JSON repository once per request and expose it through repository interfaces.
builder.Services.AddScoped<JsonRepository>();
builder.Services.AddScoped<IDeviceRepository>(sp => sp.GetRequiredService<JsonRepository>());
builder.Services.AddScoped<ILocationRepository>(sp => sp.GetRequiredService<JsonRepository>());

// Configure JSON serialization to use camelCase and serialize enums as strings.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false)
        );
    });

// Allow the local Angular frontend to call the API during development.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
             "http://localhost:4200",
                "https://localhost:4200"
        )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Populate shared simulation runtime state from persisted devices.
// This runs from Program.cs because it is application startup orchestration.
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<SimulationInitializer>();
    initializer.Initialize();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<GlobalErrorHandling>();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();

app.MapControllers();
app.Run();
