using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Thermostat;

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

    // Common status message for all devices, can be used to provide additional information about the device's current state or any errors.
    public string StatusMessage { get; set; } = string.Empty;

    // Lights
    public int? LightBrightness { get; set; }
    public string? LightColor { get; set; }

    // Fans
    public FanSpeed? FanSpeed { get; set; }

    // Thermostat
    public ThermostatMode? ThermostatMode { get; set; }
    public int? TargetTemperature { get; set; }
    public int? AmbientTemperature { get; set; }

    // Doors
    public bool? IsLocked { get; set; }
}

