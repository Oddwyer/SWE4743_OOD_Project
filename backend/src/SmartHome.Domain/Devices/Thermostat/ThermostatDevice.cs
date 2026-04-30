using SmartHome.Domain.Devices.Thermostat.ThermostatStates;

namespace SmartHome.Domain.Devices.Thermostat;

public class ThermostatDevice : Device, IPoweredDevice
{
    public int TargetTemperature { get; private set; }
    public IThermostatModeStrategy CurrentMode { get; private set; }

    // States
    private DevicePowerState _powerState; // Once flushed, revisit IPoweredDevice.
    public IdleState Idle { get; private set; }
    public CoolingState Cooling { get; private set; }
    public HeatingState Heating { get; private set; }
    public OffState Off { get; private set; }
    private IThermostatState _currentState;

    public ThermostatDevice(Guid id, string deviceName, string deviceLocation, IThermostatModeStrategy strategy) :
    base(id, deviceName, deviceLocation, DeviceType.Thermostat)
    {
        CurrentMode = strategy;

        // Initialize states
        _powerState = DevicePowerState.Off; // default state
        Idle = new IdleState(this);
        Cooling = new CoolingState(this);
        Heating = new HeatingState(this);
        Off = new OffState(this);

        _currentState = Off; // default state

    }

    /// <summary>
    /// Current power state of the thermostat.
    /// </summary>
    public DevicePowerState PowerState => _powerState;

    /// <summary>
    /// Indicates whether the thermostat is on.
    /// </summary>
    public override bool IsDeviceOn => _powerState == DevicePowerState.On;

    /// <summary>
    /// Requests a power toggle. Behavior is determined by the current state.
    /// </summary>
    public void TogglePower()
    {
        _currentState.TogglePower();
    }

    /// <summary>
    /// Sets power to on (used by states).
    /// </summary>
    internal void TurnPowerOn()
    {
        _powerState = DevicePowerState.On;

    }

    /// <summary>
    /// Sets power to off (used by states).
    /// </summary>
    internal void TurnPowerOff()
    {
        _powerState = DevicePowerState.Off;

    }

    /// <summary>
    /// Requests a target temperature change. The current state decides if allowed.
    /// </summary>
    public void SetTargetTemperature(int targetTemperature)
    {
        _currentState.SetTargetTemperature(targetTemperature);
    }

    /// <summary>
    /// Applies a target temperature change (used by states).
    /// </summary>
    internal void SetTargetTemperatureInternal(int targetTemperature)
    {
        TargetTemperature = targetTemperature;
    }

    /// <summary>
    /// Requests evaluation using the current state.
    /// </summary>
    public void Evaluate(int ambientTemperature)
    {
        _currentState.Evaluate(ambientTemperature);
    }

    /// <summary>
    /// Sets the active mode strategy.
    /// </summary>
    internal void SetModeStrategy(IThermostatModeStrategy strategy)
    {
        CurrentMode = strategy;
    }

    /// <summary>
    /// Sets the current state (used by states).
    /// </summary>
    internal void SetState(IThermostatState newState)
    {
        _currentState = newState;
    }

    /// <summary>
    /// Determines the next state using the current strategy.
    /// </summary>
    internal IThermostatState DetermineNextState(int ambientTemperature)
    {
        var nextState = CurrentMode.DetermineNextState(this, ambientTemperature);
        return nextState;
    }

    /// <summary>
    /// Updates the status message (used by states). The status message can be used for logging, debugging, or providing user feedback through the API.
    /// </summary>
    internal void UpdateStatusMessage(string message)
    {
        StatusMessage = message;
    }
}
