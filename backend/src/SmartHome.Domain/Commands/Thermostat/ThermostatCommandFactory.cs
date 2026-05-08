using SmartHome.Domain.Devices;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Commands.Powered;

namespace SmartHome.Domain.Commands.Thermostat;

/// <summary>
/// Creates command objects for thermostat devices (toggle power, set mode, set target temperature).
/// </summary>
public class ThermostatCommandFactory : IDeviceCommandFactory
{
    private readonly IThermostatModeStrategyFactory _thermostatStrategyFactory;

    /// <summary>Initializes the factory with the default strategy factory.</summary>
    public ThermostatCommandFactory() : this(new ThermostatStrategyFactory()) { }

    /// <summary>Initializes the factory with an explicit strategy factory (used in tests).</summary>
    private ThermostatCommandFactory(IThermostatModeStrategyFactory factory)
    {
        _thermostatStrategyFactory = factory;
    }

    /// <summary>Creates the appropriate thermostat command from the request context.</summary>
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

    /// <summary>Creates a TogglePower command for a thermostat device.</summary>
    private IDeviceCommand CreateTogglePowerCommand(ThermostatDevice thermostat)
    {
        if (thermostat is not IPoweredDevice poweredDevice)
        {
            throw new InvalidOperationException("This device does not support power control.");
        }

        return new TogglePowerCommand(thermostat, poweredDevice);
    }

    /// <summary>Creates a SetThermostatMode command, resolving the appropriate mode strategy.</summary>
    private IDeviceCommand CreateSetThermostatModeCommand(ThermostatDevice thermostat, CommandData context)
    {

        if (context.ThermostatMode is null)
        {
            throw new ArgumentException("A thermostat mode is required to alter current thermostat mode.");
        }

        var strategy = _thermostatStrategyFactory.Create(context.ThermostatMode.Value);

        return new SetThermostatModeCommand(thermostat, context.ThermostatMode.Value, strategy);
    }

    /// <summary>Creates a SetTargetTemperature command, validating the temperature value is present.</summary>
    private IDeviceCommand CreateSetTargetTemperatureCommand(ThermostatDevice thermostat, CommandData context)
    {

        if (context.TargetTemperature is null)
        {
            throw new ArgumentException("A target temperature must be provided to set the desired temperature.");
        }

        return new SetTargetTemperatureCommand(thermostat, context.TargetTemperature.Value);
    }

}