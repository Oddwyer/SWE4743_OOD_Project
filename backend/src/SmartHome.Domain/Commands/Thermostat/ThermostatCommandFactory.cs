using SmartHome.Domain.Devices;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Commands.Powered;

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
        if (device is not ThermostatDevice thermostat)
        {
            throw new InvalidOperationException($"{device.DeviceName} is not a thermostat and does not support thermostat commands.");
        }

        return context.Command switch
        {
            DeviceCommandType.TogglePower => CreateTogglePowerCommand(thermostat),

            DeviceCommandType.SetThermostatMode => CreateSetThermostatModeCommand(thermostat, context),

            DeviceCommandType.SetTargetTemperature => CreateSetTargetTemperatureCommand(thermostat, context),

            _ => throw new ArgumentException($"Unsupported command type.")
        };
    }

    private IDeviceCommand CreateTogglePowerCommand(ThermostatDevice thermostat)
    {
        if (thermostat is not IPoweredDevice poweredDevice)
        {
            throw new InvalidOperationException("This device does not support power control.");
        }

        return new TogglePowerCommand(thermostat, poweredDevice);
    }

    private IDeviceCommand CreateSetThermostatModeCommand(ThermostatDevice thermostat, CommandData context)
    {

        if (context.ThermostatMode is null)
        {
            throw new ArgumentException("A thermostat mode is required to alter current thermostat mode.");
        }

        var strategy = _thermostatStrategyFactory.Create(context.ThermostatMode.Value);

        return new SetThermostatModeCommand(thermostat, context.ThermostatMode.Value, strategy);
    }

    private IDeviceCommand CreateSetTargetTemperatureCommand(ThermostatDevice thermostat, CommandData context)
    {

        if (context.TargetTemperature is null)
        {
            throw new ArgumentException("A target temperature must be provided to set the desired temperature.");
        }

        return new SetTargetTemperatureCommand(thermostat, context.TargetTemperature.Value);
    }

}