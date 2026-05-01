using SmartHome.Domain.Devices;
using SmartHome.Domain.Commands.Fan;
using SmartHome.Domain.Commands.Light;
using SmartHome.Domain.Commands.Lock;
using SmartHome.Domain.Commands.Power;
using SmartHome.Domain.Commands.Thermostat;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Contracts;


namespace SmartHome.Domain.Commands;

/// <summary>
/// Creates command objects from API requests for device operations.
/// Encapsulates actions and provides descriptions for audit logging (Command Pattern).
/// </summary>

public class CommandFactory : ICommandFactory
{
    private readonly IThermostatModeStrategyFactory _thermostatStrategyFactory;

    public CommandFactory(IThermostatModeStrategyFactory factory)
    {
        _thermostatStrategyFactory = factory;
    }
    public IDeviceCommand CreateCommand(IDevice device, CommandData context)
    {
        // TODO - Amber: Modify away from switch for OCP.
        switch (context.Command)
        {
            case DeviceCommandType.TogglePower:
                return new TogglePowerCommand(device);

            case DeviceCommandType.SetBrightness:
                if (context.Brightness is null)
                {
                    throw new ArgumentException("Brightness is required for setting brightness.");
                }
                return new SetLightBrightnessCommand(device, context.Brightness.Value);

            case DeviceCommandType.SetColor:
                if (context.Color is null)
                {
                    throw new ArgumentException("Color is required for changing light color.");
                }
                return new SetLightColorCommand(device, context.Color.Value);

            case DeviceCommandType.SetFanSpeed:
                if (context.FanSpeed is null)
                {
                    throw new ArgumentException("Fan speed is required to alter fan speed.");
                }
                return new SetFanSpeedCommand(device, context.FanSpeed.Value);

            case DeviceCommandType.SetThermostatMode:
                if (context.Mode is null)
                {
                    throw new ArgumentException("A thermostat mode is required to alter current thermostat mode.");
                }
                var strategy = _thermostatStrategyFactory.Create(context.Mode.Value);
                return new SetThermostateModeCommand(device, context.Mode.Value, strategy);

            case DeviceCommandType.SetDesiredTemperature:
                if (context.TargetTemperature is null)
                {
                    throw new ArgumentException("A target temperature must be provided to alter thermostat mode.");
                }
                return new SetTargetTemperatureCommand(device, context.TargetTemperature.Value);

            case DeviceCommandType.ToggleLock:
                return new ToggleLockCommand(device);

            default:
                throw new ArgumentException($"Unsupported command type.");



        }
    }
}