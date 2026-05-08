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

    public Guid Id { get; set; }

    /// <summary>True when the device is considered active for filtering and display.</summary>
    public bool IsDeviceOn { get; set; }

    /// <summary>Explicit power state for devices with an on/off switch; null for door locks.</summary>
    public bool? IsPoweredOn { get; set; }

    public bool? IsLocked { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public int? LightBrightness { get; set; }
    public int? MinBrightness { get; set; }
    public int? MaxBrightness { get; set; }
    public string? LightColor { get; set; }

    public FanSpeed? FanSpeed { get; set; }

    public ThermostatMode? ThermostatMode { get; set; }
    public ThermostatStateType? ThermostatState { get; set; }
    public int? MinTemperature { get; set; }
    public int? MaxTemperature { get; set; }
    public int? TargetTemperature { get; set; }
    public int? DefaultTemperature { get; set; }

    public int? AmbientTemperature { get; set; }


}

