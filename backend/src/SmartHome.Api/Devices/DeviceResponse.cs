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
    public int? Brightness { get; set; }
    public string? Color { get; set; }

    // Fans
    public string? Speed { get; set; }

    // Thermostat
    public string? Mode { get; set; }
    public int? DesiredTemperature { get; set; }
    public int? AmbientTemperature { get; set; }

    // Doors
    public bool? IsLocked { get; set; }
}

