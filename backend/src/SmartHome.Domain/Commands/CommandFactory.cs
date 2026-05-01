using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Commands.Fan;
using SmartHome.Domain.Commands.Light;
using SmartHome.Domain.Commands.Lock;
using SmartHome.Domain.Commands.Power;
using SmartHome.Domain.Commands.Thermostat;
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
        // TODO - Amber: Modify away from switch for OCP? Creation branching is centralized inside factories 
        // so controllers/services remain closed to device-specific branching. Future improvement could be
        // provider registration per device type.
        switch (context.Command)
        {
            case DeviceCommandType.TogglePower:
                if (device is not IPoweredDevice poweredDevice)
                {
                    throw new InvalidOperationException("This device does not support power control.");
                }
                return new TogglePowerCommand(device, poweredDevice);

            case DeviceCommandType.SetBrightness:
                if (device is not LightDevice lightDevice)
                {
                    throw new InvalidOperationException("This device does not have a brightness setting.");
                }
                if (context.Brightness is null)
                {
                    throw new ArgumentException("Brightness is required for setting brightness.");
                }
                return new SetLightBrightnessCommand(lightDevice, context.Brightness.Value);

            case DeviceCommandType.SetColor:
                if (device is not LightDevice colorLightDevice)
                {
                    throw new InvalidOperationException("This device does not have a color setting.");
                }
                if (context.Color is null)
                {
                    throw new ArgumentException("Color is required for changing light color.");
                }
                return new SetLightColorCommand(colorLightDevice, context.Color.Value);

            case DeviceCommandType.SetFanSpeed:
                if (device is not FanDevice fanDevice)
                {
                    throw new InvalidOperationException("This device does not have a speed setting.");
                }
                if (context.FanSpeed is null)
                {
                    throw new ArgumentException("Fan speed is required to alter fan speed.");
                }
                return new SetFanSpeedCommand(fanDevice, context.FanSpeed.Value);

            case DeviceCommandType.SetThermostatMode:
                if (device is not ThermostatDevice setModeThermostat)
                {
                    throw new InvalidOperationException("This device does not have a thermostat mode setting.");
                }
                if (context.Mode is null)
                {
                    throw new ArgumentException("A thermostat mode is required to alter current thermostat mode.");
                }
                var strategy = _thermostatStrategyFactory.Create(context.Mode.Value);
                return new SetThermostateModeCommand(setModeThermostat, context.Mode.Value, strategy);

            case DeviceCommandType.SetDesiredTemperature:
                if (device is not ThermostatDevice targetTempThermostat)
                {
                    throw new InvalidOperationException("This device does not have a target temperature setting.");
                }
                if (context.TargetTemperature is null)
                {
                    throw new ArgumentException("A target temperature must be provided to alter thermostat mode.");
                }
                return new SetTargetTemperatureCommand(targetTempThermostat, context.TargetTemperature.Value);

            case DeviceCommandType.ToggleLock:

                if (device is not DoorLocks doorLock)
                {
                    throw new InvalidOperationException("This device does not have a lock setting.");
                }
                return new ToggleLockCommand(doorLock);

            default:
                throw new ArgumentException($"Unsupported command type.");



        }
    }
}