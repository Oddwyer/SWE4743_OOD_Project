namespace SmartHome.Api.Devices;

/// <summary>
/// DTO for displaying device(s) and related data.
/// </summary>

public class DeviceResponse
{
    // All devices
    public Guid Id { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceLocation { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
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

