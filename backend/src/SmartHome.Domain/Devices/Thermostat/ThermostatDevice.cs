using SmartHome.Domain.Devices.Thermostat.ThermostatStates;

namespace SmartHome.Domain.Devices.Thermostat;

public class ThermostatDevice : Device, IPoweredDevice
{
    private DevicePowerState _powerState; // Once flushed, revisit IPoweredDevice.
    // States
    public ThermostatOnState OnState { get; }
    public ThermostatIdleState IdleState { get; }
    public ThermostatCoolingState CoolingState { get; }
    public ThermostatHeatingState HeatingState { get; }
    public ThermostatOffState OffState { get; }

    // Current state 
    private IThermostatState _currentState;

    public IThermostatModeStrategy ModeStrategy { get; private set; }
    public override bool IsDeviceOn => _powerState == DevicePowerState.On;

    public DevicePowerState PowerState => _powerState;

    public ThermostatDevice(Guid id, string deviceName, string deviceLocation, IThermostatModeStrategy strategy) : base(id, deviceName, deviceLocation, DeviceType.Thermostat)
    {
        ModeStrategy = strategy;
        _powerState = DevicePowerState.Off; // default state

        // Initialize states
        OnState = new ThermostatOnState(this);
        IdleState = new ThermostatIdleState(this);
        CoolingState = new ThermostatCoolingState(this);
        HeatingState = new ThermostatHeatingState(this);
        OffState = new ThermostatOffState(this);

        _currentState = OffState; // default state

    }

    public void TogglePower()
    {
        _currentState.TogglePower();
    }

    public void SetTargetTemperature(ThermostatDevice thermostat, double temp)
    {
        _currentState.SetTargetTemperature(thermostat, temp);
    }
    public void Evaluate(ThermostatDevice thermostat, double ambientTemperature)
    {
        _currentState.Evaluate(thermostat, ambientTemperature);
    }

    public void SetModeStrategy(IThermostatModeStrategy strategy)
    {
        ModeStrategy = strategy;
    }


}
