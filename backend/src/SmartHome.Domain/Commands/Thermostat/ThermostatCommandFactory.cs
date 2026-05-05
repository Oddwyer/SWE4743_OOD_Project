using SmartHome.Domain.Devices;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices.Thermostat;

namespace SmartHome.Domain.Commands.Thermostat;

public class ThermostatCommandFactory : IDeviceCommandFactory
{
    private readonly IThermostatModeStrategyFactory _thermostatStrategyFactory;

    public ThermostatCommandFactory() : this(new ThermostatStrategyFactory())
    {

    }

    private ThermostatCommandFactory(IThermostatModeStrategyFactory factory)
    {
        _thermostatStrategyFactory = factory;
    }

    public IDeviceCommand CreateCommand(IDevice device, CommandData context)
    {
        return context.Command switch
        {
            DeviceCommandType.SetThermostatMode => CreateSetThermostatModeCommand(device, context),

            DeviceCommandType.SetTargetTemperature => CreateSetTargetTemperatureCommand(device, context),
            _ => throw new ArgumentException($"Unsupported command type.")
        };
    }

    private IDeviceCommand CreateSetThermostatModeCommand(IDevice device, CommandData context)
    {
        if (device is not ThermostatDevice setModeThermostat)
        {
            throw new InvalidOperationException("This device does not have a thermostat mode setting.");
        }
        if (context.Mode is null)
        {
            throw new ArgumentException("A thermostat mode is required to alter current thermostat mode.");
        }
        var strategy = _thermostatStrategyFactory.Create(context.Mode.Value);
        return new SetThermostatModeCommand(setModeThermostat, context.Mode.Value, strategy);
    }

    private IDeviceCommand CreateSetTargetTemperatureCommand(IDevice device, CommandData context)
    {
        if (device is not ThermostatDevice targetTempThermostat)
        {
            throw new InvalidOperationException("This device does not have a target temperature setting.");
        }
        if (context.TargetTemperature is null)
        {
            throw new ArgumentException("A target temperature must be provided to set the desired temperature.");
        }
        return new SetTargetTemperatureCommand(targetTempThermostat, context.TargetTemperature.Value);
    }

}