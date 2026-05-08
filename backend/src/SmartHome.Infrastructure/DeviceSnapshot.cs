using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Thermostat;

namespace SmartHome.Infrastructure;

/// <summary>
/// Immutable snapshot of a device's state used for JSON persistence and rehydration.
/// </summary>
public record DeviceSnapshot
{
    /// <summary>Unique device identifier.</summary>
    public Guid Id { get; init; }
    /// <summary>Human-readable device name.</summary>
    public string Name { get; init; } = string.Empty;
    /// <summary>Device category.</summary>
    public DeviceType Type { get; init; }
    /// <summary>Location the device is installed in.</summary>
    public string Location { get; init; } = string.Empty;
    /// <summary>True when the device was on at snapshot time.</summary>
    public bool IsOn { get; init; }
    /// <summary>Serialized state machine state.</summary>
    public string? DeviceState { get; init; }
    /// <summary>Thermostat mode (thermostat devices only).</summary>
    public ThermostatMode? ThermostatMode { get; init; }
    /// <summary>Target temperature in °F (thermostat devices only).</summary>
    public int? TargetTemperature { get; init; }
    /// <summary>Light color hex string (light devices only).</summary>
    public string? LightColor { get; init; }
    /// <summary>Light brightness 10–100 (light devices only).</summary>
    public int? LightBrightness { get; init; }
    /// <summary>Fan speed (fan devices only).</summary>
    public FanSpeed? FanSpeed { get; init; }
}
