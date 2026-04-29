using System.ComponentModel.DataAnnotations;
using SmartHome.Domain.Devices.Light;

namespace SmartHome.Api.Devices;

/// <summary>
/// DTO used to represent a command request from the client.
/// </summary>

public class ControlDeviceRequest
{
    // All devices
    [Required]
    public string Command { get; set; } = string.Empty;
    public int? Brightness { get; set; }
    public LightColorState? Color { get; set; }
    public string? FanSpeed { get; set; }
    public int? DesiredTemperature { get; set; }

}