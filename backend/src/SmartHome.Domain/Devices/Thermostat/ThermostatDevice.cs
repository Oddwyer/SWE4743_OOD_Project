using SmartHome.Domain.Devices.Thermostat.ThermostatStates;

namespace SmartHome.Domain.Devices.Thermostat;

// basic template of what all devices will need in the home simulator
public abstract class ThermostatDevice : Device, IPoweredDevice
{   //core fields and properties accounted for first
    private IThermostatState _currentState;
    private DevicePowerState _powerState;
    private readonly IThermostatModeStrategy _modeStrategy;
    public override bool IsDeviceOn => _powerState == DevicePowerState.On;

    public DevicePowerState PowerState => _powerState;
    public ThermostatDevice(Guid id, string deviceName, string deviceLocation, IThermostatModeStrategy strategy) : base(id, deviceName, deviceLocation, DeviceType.Thermostat)
    {
        _powerState = DevicePowerState.Off; // default state
        _modeStrategy = strategy;
    }

    public void TogglePower()
    {
        // trying toggle with a ternary operator for cleaner code

        _powerState = _powerState == DevicePowerState.On // check current state and toggle
        ? DevicePowerState.Off  // if on, turn off
        : DevicePowerState.On;  // if off, turn on
    }




}
