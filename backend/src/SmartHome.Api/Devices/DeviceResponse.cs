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
    // All devices. Includes DeviceName, DeviceLocation, and Type from BaseDevice
    public Guid Id { get; set; }
    public bool IsDeviceOn { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Lights
    public int? LightBrightness { get; set; }
    public int? MinBrightness { get; set; }
    public int? MaxBrightness { get; set; }
    public string? LightColor { get; set; }

    // Fans
    public FanSpeed? FanSpeed { get; set; }

    // Thermostat
    public ThermostatMode? ThermostatMode { get; set; }
    public ThermostatStateType? ThermostatState { get; set; }

    public int? MinTemperature { get; set; }
    public int? MaxTemperature { get; set; }
    public int? TargetTemperature { get; set; }
    public int? AmbientTemperature { get; set; } // For bruno testing only. Frontend displayed via SimulationResponse.

    // Doors
    public bool? IsLocked { get; set; }
}

