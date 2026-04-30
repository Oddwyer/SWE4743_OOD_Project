using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Thermostat;

namespace SmartHome.Infrastructure;

// Stores device-specific values needed for JSON persistence and rehydration.
public record DeviceSnapshot
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public DeviceType Type { get; init; }
    public string? Location { get; init; }
    public bool IsOn { get; init; }
    public string? DeviceState { get; init; }
    public ThermostatMode? ThermostatMode { get; init; }
    public int? TargetTemperature { get; init; }

    public LightColor? LightColor { get; init; }

    public int? LightBrightness { get; init; }

    public FanSpeed? FanSpeed { get; init; }

}