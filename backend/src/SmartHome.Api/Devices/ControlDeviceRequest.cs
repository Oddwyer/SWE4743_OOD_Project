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
    /// <summary>The command to execute on the target device.</summary>
    public DeviceCommandType Command { get; set; }
    /// <summary>Brightness value for SetBrightness commands (10–100).</summary>
    public int? Brightness { get; set; }
    /// <summary>Hex color string for SetColor commands (e.g. #FF0000).</summary>
    public string? Color { get; set; }
    /// <summary>Fan speed for SetFanSpeed commands.</summary>
    public FanSpeed? FanSpeed { get; set; }
    /// <summary>Thermostat mode for SetThermostatMode commands.</summary>
    public ThermostatMode? ThermostatMode { get; set; }
    /// <summary>Target temperature in °F for SetTargetTemperature commands (60–80).</summary>
    public int? TargetTemperature { get; set; }
}
