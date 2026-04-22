namespace SmartHome.Api.DTOs;


/// <summary>
/// DTO for displaying device(s) and related data
/// </summary>
public class DeviceResponse
{
    public Guid Id { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceLocation { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsDeviceOn { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public int? Brightness { get; set; }
    public string? Color { get; set; }

    public string? Speed { get; set; }

    public string? Mode { get; set; }
    public int? DesiredTemperature { get; set; }
    public int? AmbientTemperature { get; set; }

    public bool? IsLocked { get; set; }
}

