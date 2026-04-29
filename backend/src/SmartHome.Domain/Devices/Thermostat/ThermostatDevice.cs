namespace SmartHome.Domain.Devices.Thermostat;

// basic template of what all devices will need in the home simulator
public abstract class TheromstatDevice : Device, IPoweredDevice
{   //core fields and properties accounted for first
    private DevicePowerState _powerState;
    private readonly IThermostatModeStrategy _modeStrategy;
    public override bool IsDeviceOn => _powerState == DevicePowerState.On;

    public TheromstatDevice(Guid id, string deviceName, string deviceLocation, IThermostatModeStrategy strategy) : base(id, deviceName, deviceLocation, DeviceType.Thermostat)
    {
        _powerState = DevicePowerState.Off; // default state
        _modeStrategy = strategy;
    }

    public DevicePowerState PowerState => _powerState;

    public void TogglePower()
    {
        // trying toggle with a ternary operator for cleaner code

        _powerState = _powerState == DevicePowerState.On // check current state and toggle
        ? DevicePowerState.Off  // if on, turn off
        : DevicePowerState.On;  // if off, turn on
    }

    //each device needs to be able to control/manage its own state
    //TODO - Kataali: Create DeviceState class...
    //public abstract DeviceState State {get; protected set;} 




}
