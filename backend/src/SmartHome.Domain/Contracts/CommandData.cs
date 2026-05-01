using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Commands;

namespace SmartHome.Domain.Contracts;


/// <summary>
/// Command request context object to encapsulate request data for Command Factory.
/// </summary>

public class CommandData
{
    public required DeviceCommandType Command { get; init; }
    public int? Brightness { get; init; }
    public LightColor? Color { get; init; }
    public FanSpeed? FanSpeed { get; init; }
    public ThermostatMode? Mode { get; init; }
    public int? TargetTemperature { get; init; }

}
