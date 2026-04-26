using System.ComponentModel.DataAnnotations;

namespace SmartHome.Api.Devices;

/// <summary>
/// DTO for registering new devices into the smart home registry. 
/// Must provide device name, location, and type.
/// </summary>

public class RegisterDeviceRequest
{

    [Required]
    public string DeviceName { get; set; } = string.Empty;

    [Required]
    public string DeviceLocation { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = string.Empty;
}