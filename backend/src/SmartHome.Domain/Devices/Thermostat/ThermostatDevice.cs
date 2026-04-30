using SmartHome.Domain.Devices.Thermostat.ThermostatStates;

namespace SmartHome.Domain.Devices.Thermostat;

public class ThermostatDevice : Device, IPoweredDevice
{
    public int TargetTemperature { get; private set; }
    public IThermostatModeStrategy CurrentMode { get; private set; }

    // States
    private DevicePowerState _powerState; // Once flushed, revisit IPoweredDevice.
    public ThermostatIdleState IdleState { get; private set; }
    public ThermostatCoolingState CoolingState { get; private set; }
    public ThermostatHeatingState HeatingState { get; private set; }
    public ThermostatOffState OffState { get; private set; }
    private IThermostatState _currentState;


    public override bool IsDeviceOn => _powerState == DevicePowerState.On;
    public DevicePowerState PowerState => _powerState;

    public ThermostatDevice(Guid id, string deviceName, string deviceLocation, IThermostatModeStrategy strategy) :
    base(id, deviceName, deviceLocation, DeviceType.Thermostat)
    {
        CurrentMode = strategy;

        // Initialize states
        _powerState = DevicePowerState.Off; // default state
        IdleState = new ThermostatIdleState(this);
        CoolingState = new ThermostatCoolingState(this);
        HeatingState = new ThermostatHeatingState(this);
        OffState = new ThermostatOffState(this);

        _currentState = OffState; // default state

    }

    /// <summary>
    /// Toggles the power state of the thermostat. If the thermostat is currently on, it will be turned off, and vice versa.
    /// </summary>
    public void TogglePower()
    {
        _currentState.TogglePower();
    }

    /// <summary>
    /// 1nternal method to turn the power on. This method is called by the state classes to update the power state of the thermostat.
    /// </summary>
    internal void TurnPowerOn()
    {
        _powerState = DevicePowerState.On;

    }

    /// <summary>
    /// Internal method to turn the power off. This method is called by the state classes to update the power state of the thermostat.
    /// </summary>
    internal void TurnPowerOff()
    {
        _powerState = DevicePowerState.Off;

    }

    /// <summary>
    /// Sets the target temperature for the thermostat. The behavior of this method will depend on the current state of the thermostat
    /// and the mode strategy in use. 
    /// 
    /// For example:
    /// - If the thermostat is in cooling mode and the target temperature is set below the ambient temperature, it may transition to the cooling state. 
    /// - If it's in heating mode and the target temperature is set above the ambient temperature, it may transition to the heating state. 
    /// - If the target temperature is set to a comfortable range around the ambient temperature, it may transition to an idle state. 
    /// The specific logic for these transitions will be defined within the respective state classes and influenced by the mode strategy.
    /// </summary>
    public void SetTargetTemperature(int targetTemperature)
    {
        _currentState.SetTargetTemperature(targetTemperature);
    }

    /// <summary>
    /// Internal method to set the target temperature. This method is called by the state classes to update the target temperature of the 
    /// thermostat based on the logic defined within those states and influenced by the mode strategy. (Prevents recursion using public method.)
    /// </summary>
    internal void SetTargetTemperatureInternal(int targetTemperature)
    {
        TargetTemperature = targetTemperature;
    }

    /// <summary>
    /// Evaluates the current state of the thermostat based on the ambient temperature and the target temperature. 
    /// This method will be called periodically (e.g., every minute) to determine if the thermostat needs to transition 
    /// to a different state (e.g., from idle to cooling or heating) based on the current conditions and the mode strategy in use.
    /// </summary>
    public void Evaluate(int ambientTemperature)
    {
        _currentState.Evaluate(ambientTemperature);
    }

    /// <summary>
    /// Sets the current state of the thermostat. This method is used by the state classes to transition the thermostat to a new state 
    /// based on the logic defined within those states and influenced by the mode strategy.
    /// </summary>
    internal void SetModeStrategy(IThermostatModeStrategy strategy)
    {
        CurrentMode = strategy;
    }

    /// <summary>
    /// Sets the current state of the thermostat. This method is used by the state classes to transition the thermostat to a new state 
    /// based on the logic defined within those states and influenced by the mode strategy.
    /// </summary>
    internal void SetState(IThermostatState newState)
    {
        _currentState = newState;
    }

    /// <summary>
    /// Determines the next state of the thermostat based on the current mode strategy and the ambient temperature. 
    /// This method is called by the state classes during evaluation to decide if a state transition is necessary.
    /// </summary>
    internal IThermostatState DetermineNextState(int ambientTemperature)
    {
        var nextState = CurrentMode.DetermineNextState(this, ambientTemperature);
        return nextState;
    }
}
