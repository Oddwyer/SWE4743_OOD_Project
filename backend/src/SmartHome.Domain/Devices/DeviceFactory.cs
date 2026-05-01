using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Devices.Thermostat.ThermostatStates;
using SmartHome.Domain.Contracts;

/// <summary>
/// Creates and restores domain device instances.
/// </summary>

namespace SmartHome.Domain.Devices;

public class DeviceFactory : IDeviceFactory
{
    private IThermostatModeStrategyFactory _thermostatModeStrategyFactory;

    public DeviceFactory(IThermostatModeStrategyFactory factory)
    {
        _thermostatModeStrategyFactory = factory;
    }

    /// <summary>
    /// Creates and restores domain device instances.
    /// </summary>
    public IDevice CreateDevice(string name, string location, DeviceType type)
    {
        Guid id = Guid.NewGuid();

        // TODO: Amber - Modify away from switch for OCP?
        switch (type)
        {
            case DeviceType.Light:
                return new LightDevice(id, name, location);

            case DeviceType.Fan:
                return new FanDevice(id, name, location);

            case DeviceType.Thermostat:
                var mode = ThermostatMode.Auto;
                var strategy = _thermostatModeStrategyFactory.Create(mode);
                return new ThermostatDevice(id, name, location, mode, strategy);

            case DeviceType.DoorLock:
                return new DoorLocks(id, name, location);

            default:
                throw new ArgumentException("Unsupported device type.");
        }
    }

    /// <summary>
    /// Creates and restores domain device instances.
    /// </summary>
    public IDevice RehydrateDevice(DeviceRehydrationData data)
    {

        return data.Type switch
        {
            DeviceType.Light => RehydrateLight(data),

            DeviceType.Fan => RehydrateFan(data),

            DeviceType.DoorLock => RehydrateDoorLock(data),

            DeviceType.Thermostat => RehydrateThermostat(data),

            _ => throw new ArgumentException("Unsupported device type.")
        };
    }

    /// <summary>
    /// Restores a light from persisted values.
    /// </summary>
    private IDevice RehydrateLight(DeviceRehydrationData data)
    {

        var light = new LightDevice(data.Id, data.Name, data.Location);

        var powerState = data.IsOn ? DevicePowerState.On : DevicePowerState.Off;
        var lightcolor = data.LightColor ?? LightColor.White;
        var lightBrightness = data.LightBrightness ?? 100;

        light.RehydrateState(powerState, lightcolor, lightBrightness);

        return light;
    }

    /// <summary>
    /// Restores a fan from persisted values.
    /// </summary>
    private IDevice RehydrateFan(DeviceRehydrationData data)
    {

        var fan = new FanDevice(data.Id, data.Name, data.Location);

        var powerState = data.IsOn ? DevicePowerState.On : DevicePowerState.Off;
        var fanSpeed = data.FanSpeed ?? FanSpeed.Medium;

        fan.RehydrateState(powerState, fanSpeed);

        return fan;
    }

    /// <summary>
    /// Restores a door lock from persisted values.
    /// </summary>
    private IDevice RehydrateDoorLock(DeviceRehydrationData data)
    {

        var doorlock = new DoorLocks(data.Id, data.Name, data.Location);

        var latchState = Enum.TryParse<DeviceLatchState>(data.DeviceState, ignoreCase: true, out var parsedState)
            ? parsedState
            : DeviceLatchState.Locked;

        doorlock.RehydrateState(latchState);

        return doorlock;
    }

    /// <summary>
    /// Restores a thermostat from persisted values.
    /// </summary>
    private IDevice RehydrateThermostat(DeviceRehydrationData data)
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

