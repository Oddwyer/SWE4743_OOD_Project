using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Devices.Thermostat.ThermostatStates;

/// <summary>
/// Creates and rehydrates device instances based on type or persisted data.
/// Centralizes device creation and hides concrete implementations (Factory Pattern).
/// </summary>

namespace SmartHome.Domain.Devices;

// TODO Kataali: This factory belongs to the Domain layer because it creates domain objects.
// I added this so API endpoints could be tested but we still need thermostat. You're welcome to flush it out!

public class DeviceFactory : IDeviceFactory
{
    private IThermostatModeStrategyFactory _thermostatModeStrategyFactory;

    public DeviceFactory(IThermostatModeStrategyFactory factory)
    {
        _thermostatModeStrategyFactory = factory;
    }

    /// <summary>
    /// Creates new specific device based on type entered.
    /// </summary>
    public IDevice CreateDevice(string name, string location, DeviceType type)
    {
        Guid id = Guid.NewGuid();

        switch (type)
        {
            case DeviceType.Light:
                return new LightDevice(id, name, location);

            case DeviceType.Fan:
                return new FanDevice(id, name, location);

            // TODO - Kataali (or I can do it): Add Thermostat creation and rehydration once thermostat constructor/state 
            // fields are finalized.

            /*case DeviceType.Thermostat:
                return new Thermostat(name, location);*/

            case DeviceType.DoorLock:
                return new DoorLocks(id, name, location);

            default:
                throw new ArgumentException("Unsupported device type.");
        }
    }

    /// <summary>
    /// Rehydrates saved data into device objects.
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


    private IDevice RehydrateLight(DeviceRehydrationData data)
    {

        var light = new LightDevice(data.Id, data.Name ?? "", data.Location ?? "");

        var powerState = data.IsOn ? DevicePowerState.On : DevicePowerState.Off;
        var lightcolor = data.LightColor ?? LightColor.White;
        var lightBrightness = data.LightBrightness ?? 100;

        light.RehydrateState(powerState, lightcolor, lightBrightness);

        return light;
    }

    private IDevice RehydrateFan(DeviceRehydrationData data)
    {

        var fan = new FanDevice(data.Id, data.Name ?? "", data.Location ?? "");

        var powerState = data.IsOn ? DevicePowerState.On : DevicePowerState.Off;
        var fanSpeed = data.FanSpeed ?? FanSpeed.Medium;

        fan.RehydrateState(powerState, fanSpeed);

        return fan;
    }


    private IDevice RehydrateDoorLock(DeviceRehydrationData data)
    {

        var doorlock = new DoorLocks(data.Id, data.Name ?? "", data.Location ?? "");

        var latchState = data.IsOn ? DeviceLatchState.Locked : DeviceLatchState.Unlocked;

        doorlock.RehydrateState(latchState);

        return doorlock;
    }



    private IDevice RehydrateThermostat(DeviceRehydrationData data)
    {

        var strategy = _thermostatModeStrategyFactory.Create(data.ThermostatMode ?? ThermostatMode.Auto);

        var thermostat = new ThermostatDevice(data.Id, data.Name ?? "", data.Location ?? "", strategy);

        var powerState = data.IsOn ? DevicePowerState.On : DevicePowerState.Off;

        var stateType = powerState == DevicePowerState.Off ?
        ThermostatStateType.Off
        : Enum.TryParse<ThermostatStateType>(data.DeviceState, ignoreCase: true, out var parsedState)
            ? parsedState
            : ThermostatStateType.Idle;

        var targetTemp = data.TargetTemperature ?? ThermostatDevice.MinTemperature;

        thermostat.RehydrateState(powerState, strategy, targetTemp, stateType);

        return thermostat;
    }



}

