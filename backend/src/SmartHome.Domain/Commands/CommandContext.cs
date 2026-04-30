using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.Fan;

namespace SmartHome.Domain.Commands;


/// <summary>
/// Command request context object to encapsulate request data for Command Factory.
/// </summary>

public class CommandContext
{
    public required DeviceCommandType Command { get; init; }

    public int? Brightness { get; init; }
    public LightColor? Color { get; init; }
    public FanSpeed? FanSpeed { get; init; }
    public ThermostatMode? Mode { get; init; }
    public int? TargetTemperature { get; init; }

    public CommandContext(DeviceCommandType commandType, int brightness, LightColor color, FanSpeed speed, ThermostatMode mode, int targetTemp)
    {
        Command = commandType;
        Brightness = brightness;
        Color = color;
        FanSpeed = speed;
        Mode = mode;
        TargetTemperature = targetTemp;
    }

}
