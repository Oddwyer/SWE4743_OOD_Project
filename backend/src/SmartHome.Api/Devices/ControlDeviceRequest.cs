using System.ComponentModel.DataAnnotations;

namespace SmartHome.Api.Devices;

/// <summary>
/// DTO for data related to device state change requests.
/// </summary>

public class ControlDeviceRequest
{
    // All devices
    public bool? IsDeviceOn { get; set; }

    // Lights
    public int? Brightness { get; set; }
    public string? Color { get; set; }

    // Fans
    public string? Speed { get; set; }

    // Thermostat
    public string? Mode { get; set; }

    [Range(60,80)]
    public int? DesiredTemperature { get; set; }

    // Doors
    public bool? IsLocked { get; set; }
}