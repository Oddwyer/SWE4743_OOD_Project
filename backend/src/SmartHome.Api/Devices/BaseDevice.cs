using System.ComponentModel.DataAnnotations;

namespace SmartHome.Api.Devices;

/// <summary>
/// Abstract DTO for reuse across concrete DTO classes.
/// </summary>

public abstract class BaseDevice
{
    [Required]
    public string DeviceName { get; set; } = string.Empty;

    [Required]
    public string DeviceLocation { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = string.Empty;
}
