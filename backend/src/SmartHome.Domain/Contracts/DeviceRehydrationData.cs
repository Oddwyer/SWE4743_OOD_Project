using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Contracts;

/// <summary>
/// Snapshot data used to reconstruct a device from persistent storage without exposing concrete types.
/// </summary>
public class DeviceRehydrationData
{
    /// <summary>Unique device identifier.</summary>
    public Guid Id { get; init; }
    /// <summary>Human-readable device name.</summary>
    public string Name { get; init; } = string.Empty;
    /// <summary>Location the device is installed in.</summary>
    public string Location { get; init; } = string.Empty;
    /// <summary>Device category.</summary>
    public DeviceType Type { get; init; }
    /// <summary>Whether the device was on when last persisted.</summary>
    public bool IsOn { get; init; }
    /// <summary>Serialized state machine state (used by thermostat and fan).</summary>
    public string? DeviceState { get; init; }
    /// <summary>Last thermostat mode (thermostat devices only).</summary>
    public ThermostatMode? ThermostatMode { get; init; }
    /// <summary>Last target temperature in °F (thermostat devices only).</summary>
    public int? TargetTemperature { get; init; }
    /// <summary>Last light color hex string (light devices only).</summary>
    public string? LightColor { get; init; }
    /// <summary>Last brightness level 10–100 (light devices only).</summary>
    public int? LightBrightness { get; init; }
    /// <summary>Last fan speed (fan devices only).</summary>
    public FanSpeed? FanSpeed { get; init; }
}
