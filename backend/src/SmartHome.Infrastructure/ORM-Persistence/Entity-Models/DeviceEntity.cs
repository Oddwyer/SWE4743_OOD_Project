using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Thermostat;

/// <summary>
/// EF Core entity representing a persisted smart home device.
/// </summary>
public class DeviceEntity
{
    /// <summary>Primary key (UUID).</summary>
    public Guid Id { get; set; }
    /// <summary>Human-readable device name.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Location the device is installed in.</summary>
    public string Location { get; set; } = string.Empty;
    /// <summary>Device category.</summary>
    public DeviceType Type { get; set; }
    /// <summary>True when the device was on at the time of the last save.</summary>
    public bool IsOn { get; set; }
    /// <summary>Serialized state machine state (used by thermostat and fan state patterns).</summary>
    public string? DeviceState { get; set; }

    /// <summary>Fan speed integer value (fan devices only).</summary>
    public int? FanSpeed { get; set; }

    /// <summary>True when locked (door lock devices only).</summary>
    public bool? IsLocked { get; set; }

    /// <summary>Light color hex string (light devices only).</summary>
    public string? LightColor { get; set; }
    /// <summary>Light brightness 10–100 (light devices only).</summary>
    public int? LightBrightness { get; set; }

    /// <summary>Thermostat operating mode (thermostat devices only).</summary>
    public ThermostatMode? ThermostatMode { get; set; }
    /// <summary>Target temperature in °F (thermostat devices only).</summary>
    public int? TargetTemperature { get; set; }
}
