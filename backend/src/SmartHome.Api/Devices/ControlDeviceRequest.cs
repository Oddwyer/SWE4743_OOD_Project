using System.ComponentModel.DataAnnotations;

namespace SmartHome.Api.Devices;

/// <summary>
/// DTO used to represent a command request from the client.
/// </summary>

public class ControlDeviceRequest
{
    // All devices
    [Required]
    public string Command { get; set; } = string.Empty;

}