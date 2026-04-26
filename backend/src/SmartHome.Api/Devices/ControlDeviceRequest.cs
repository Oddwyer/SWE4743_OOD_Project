using System.ComponentModel.DataAnnotations;

namespace SmartHome.Api.Devices;

/// <summary>
/// DTO for data related to device state change requests.
/// </summary>

public class ControlDeviceRequest
{
    // All devices
    [Required]
    public string Command { get; set; } = string.Empty;

}