using SmartHome.Domain;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Simulations;
using SmartHome.Domain.Locations;
using SmartHome.Infrastructure.ORM_Persistence;
using SmartHome.Infrastructure;
using SmartHome.Api.Middleware;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register MVC controllers and configure API behavior.
// Includes:
// - RFC/problem+json validation responses
// - camelCase JSON serialization
// - enum serialization as readable strings instead of integers
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Type = "about:blank",
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Instance = context.HttpContext.Request.Path
            };

            return new BadRequestObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json" }
            };
        };
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(
                JsonNamingPolicy.CamelCase,
                allowIntegerValues: false)
        );
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
builder.Services.AddSingleton<SimulationInitializer>();

// Register application services.
builder.Services.AddSingleton<ISimulationService, SimulationService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();

// Register device factories.
builder.Services.AddSingleton<IDeviceTypeFactory, LightDeviceFactory>();
builder.Services.AddSingleton<IDeviceTypeFactory, FanDeviceFactory>();
builder.Services.AddSingleton<IDeviceTypeFactory, ThermostatDeviceFactory>();
builder.Services.AddSingleton<IDeviceTypeFactory, DoorLockFactory>();

// Extra credit SQLite/ORM implementation in progress but is not currently wired as the active repository.
//builder.Services.AddScoped<IDeviceRepository, SqliteRepository>();
//builder.Services.AddScoped<ILocationRepository, SqliteRepository>();

// Register command and strategy factories.
builder.Services.AddSingleton<IDeviceFactory, DeviceFactory>();
builder.Services.AddSingleton<IThermostatModeStrategyFactory, ThermostatStrategyFactory>();
builder.Services.AddScoped<IDeviceCommandFactory, CommandFactory>();

// Register JSON repository and expose it through repository interfaces.
builder.Services.AddSingleton<JsonRepository>();
builder.Services.AddSingleton<IDeviceRepository>(sp => sp.GetRequiredService<JsonRepository>());
builder.Services.AddSingleton<ILocationRepository>(sp => sp.GetRequiredService<JsonRepository>());

// Add DbContext with SQLite provider
builder.Services.AddDbContext<SmartHomeDbContext>(options =>
{
    options.UseSqlite("Data Source=SmartHome.db");
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

// database creation, migration, and seeding
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SmartHomeDbContext>();
    if (app.Environment.IsEnvironment("Testing"))
        dbContext.Database.EnsureCreated();
    else
        dbContext.Database.Migrate();
    SmartHomeSeedData.Seed(dbContext);
}
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

//app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();

app.MapControllers();
app.Run();

// Exposes the auto-generated Program class to the integration test project.
public partial class Program { }