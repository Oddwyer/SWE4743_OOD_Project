using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Commands;

namespace SmartHome.Domain.Contracts;

/// <summary>
/// Request context passed to command factories; carries the command type and any device-specific parameters.
/// </summary>
public class CommandData
{
    /// <summary>The command to execute on the target device.</summary>
    public required DeviceCommandType Command { get; init; }
    /// <summary>Brightness value for SetBrightness commands (10–100).</summary>
    public int? Brightness { get; init; }
    /// <summary>Hex color string for SetColor commands.</summary>
    public string? Color { get; init; }
    /// <summary>Fan speed for SetFanSpeed commands.</summary>
    public FanSpeed? FanSpeed { get; init; }
    /// <summary>Thermostat mode for SetThermostatMode commands.</summary>
    public ThermostatMode? ThermostatMode { get; init; }
    /// <summary>Target temperature (°F) for SetTargetTemperature commands (60–80).</summary>
    public int? TargetTemperature { get; init; }
}
