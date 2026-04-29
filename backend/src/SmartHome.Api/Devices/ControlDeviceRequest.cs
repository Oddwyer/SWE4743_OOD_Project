using System.ComponentModel.DataAnnotations;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Commands;

namespace SmartHome.Api.Devices;

/// <summary>
/// DTO used to represent a command request from the client.
/// </summary>

public class ControlDeviceRequest
{
    // All devices
    [Required]
    public DeviceCommandType? Command { get; set; }
    public int? Brightness { get; set; }
    public LightColorState? Color { get; set; }
    public FanSpeed FanSpeed { get; set; }
    public ThermostatMode? Mode { get; set; }
    public int? DesiredTemperature { get; set; }

}