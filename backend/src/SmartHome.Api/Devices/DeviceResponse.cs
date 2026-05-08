using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.Thermostat.ThermostatStates;

namespace SmartHome.Api.Devices;

/// <summary>
/// DTO used to represent a device returned by the API.
/// </summary>
public class DeviceResponse : BaseDevice
{
    /// <summary>Unique device identifier (UUID).</summary>
    public Guid Id { get; set; }
    /// <summary>True when the device is considered active for filtering and display.</summary>
    public bool IsDeviceOn { get; set; }
    /// <summary>Explicit power state for devices with an on/off switch; null for door locks.</summary>
    public bool? IsPoweredOn { get; set; }
    /// <summary>Timestamp of device registration.</summary>
    public DateTime CreatedAt { get; set; }
    /// <summary>Timestamp of the most recent state change.</summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>Current brightness level (light devices, 10–100).</summary>
    public int? LightBrightness { get; set; }
    /// <summary>Minimum allowed brightness (light devices).</summary>
    public int? MinBrightness { get; set; }
    /// <summary>Maximum allowed brightness (light devices).</summary>
    public int? MaxBrightness { get; set; }
    /// <summary>Current color hex string (light devices).</summary>
    public string? LightColor { get; set; }

    /// <summary>Current fan speed (fan devices).</summary>
    public FanSpeed? FanSpeed { get; set; }

    /// <summary>Active operating mode (thermostat devices).</summary>
    public ThermostatMode? ThermostatMode { get; set; }
    /// <summary>Runtime heating/cooling state (thermostat devices).</summary>
    public ThermostatStateType? ThermostatState { get; set; }
    /// <summary>Minimum allowed target temperature in °F (thermostat devices).</summary>
    public int? MinTemperature { get; set; }
    /// <summary>Maximum allowed target temperature in °F (thermostat devices).</summary>
    public int? MaxTemperature { get; set; }
    /// <summary>User-set target temperature in °F (thermostat devices).</summary>
    public int? TargetTemperature { get; set; }
    /// <summary>Default target temperature in °F (thermostat devices).</summary>
    public int? DefaultTemperature { get; set; }
    /// <summary>Current ambient temperature in the device's location in °F (thermostat devices).</summary>
    public int? AmbientTemperature { get; set; }

    /// <summary>Lock state (door lock devices).</summary>
    public bool? IsLocked { get; set; }
}

