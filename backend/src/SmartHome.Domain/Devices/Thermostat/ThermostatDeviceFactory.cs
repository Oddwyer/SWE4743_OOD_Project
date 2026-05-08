using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Thermostat.ThermostatStates;
using SmartHome.Domain.Contracts;

namespace SmartHome.Domain.Devices.Thermostat;

/// <summary>
/// Creates and restores thermostat device instances.
/// </summary>
public class ThermostatDeviceFactory : IDeviceTypeFactory
{
    public DeviceType DeviceType => DeviceType.Thermostat;
    private IThermostatModeStrategyFactory _thermostatModeStrategyFactory;

    public ThermostatDeviceFactory(IThermostatModeStrategyFactory factory)
    {
        _thermostatModeStrategyFactory = factory;
    }

    /// <summary>
    /// Creates thermostat device instances.
    /// </summary>
    public IDevice CreateDevice(string name, string location)
    {
        Guid id = Guid.NewGuid();
        var mode = ThermostatMode.Auto;
        var strategy = _thermostatModeStrategyFactory.Create(mode);
        return new ThermostatDevice(id, name, location, mode, strategy);

    }


    /// <summary>
    /// Restores a thermostat from persisted values.
    /// </summary>
    public IDevice RehydrateDevice(DeviceRehydrationData data)
    {

        var mode = data.ThermostatMode ?? ThermostatMode.Auto;

        var strategy = _thermostatModeStrategyFactory.Create(data.ThermostatMode ?? ThermostatMode.Auto);

        var thermostat = new ThermostatDevice(data.Id, data.Name, data.Location, mode, strategy);

        var powerState = data.IsOn ? DevicePowerState.On : DevicePowerState.Off;

        var stateType = powerState == DevicePowerState.Off ?
        ThermostatStateType.Off
        : Enum.TryParse<ThermostatStateType>(data.DeviceState, ignoreCase: true, out var parsedState)
            ? parsedState
            : ThermostatStateType.Idle;

        var targetTemp = data.TargetTemperature ?? 72;

        thermostat.RehydrateState(powerState, strategy, targetTemp, stateType, mode);

        return thermostat;

    }

}

