using SmartHome.Domain.Devices.Thermostat.ThermostatStates;
using SmartHome.Domain.Commands.Thermostat;

namespace SmartHome.Domain.Devices.Thermostat;

/// <summary>
/// Temperature-regulating device that heats or cools a room based on ambient and target temperature.
/// </summary>
public class ThermostatDevice : Device, IPoweredDevice
{
    public const int DefaultTemperature = 72;
    public const int MinTemperature = 60; // Minimum allowed temperature
    public const int MaxTemperature = 80; // Maximum allowed temperature

    private const ThermostatMode defaultMode = ThermostatMode.Auto;

    public int TargetTemperature { get; private set; } = DefaultTemperature;
    public IThermostatModeStrategy CurrentStrategy { get; private set; }
    public ThermostatMode Mode { get; private set; } = defaultMode;

    // States
    private DevicePowerState _powerState = DevicePowerState.Off; // Default power state; has sub-states for Idle, Heating, Cooling.
    public IdleState Idle { get; private set; }
    public CoolingState Cooling { get; private set; }
    public HeatingState Heating { get; private set; }
    public OffState Off { get; private set; }
    private IThermostatState _currentState;

    // Helper property to expose current state type for API responses and persistence.     
    public ThermostatStateType CurrentStateType
    {
        get
        {
            if (_currentState == Off)
                return ThermostatStateType.Off;

            if (_currentState == Cooling)
                return ThermostatStateType.Cooling;

            if (_currentState == Heating)
                return ThermostatStateType.Heating;

            return ThermostatStateType.Idle;
        }
    }

    public ThermostatDevice(Guid id, string deviceName, string deviceLocation, ThermostatMode mode, IThermostatModeStrategy strategy) :

    base(id, deviceName, deviceLocation, DeviceType.Thermostat, new ThermostatCommandFactory())
    {
        CurrentStrategy = strategy;
        Mode = mode;

        // Initialize states
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
    public override bool IsDeviceOn => _currentState == Heating || _currentState == Cooling;

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
        UpdatedAt = DateTime.UtcNow;

    }

    /// <summary>
    /// Sets power to off (used by states).
    /// </summary>
    internal void TurnPowerOff()
    {
        _powerState = DevicePowerState.Off;
        UpdatedAt = DateTime.UtcNow;

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
        UpdatedAt = DateTime.UtcNow;
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
    internal void SetMode(ThermostatMode mode, IThermostatModeStrategy strategy)
    {
        Mode = mode;
        CurrentStrategy = strategy;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the current state (used by states).
    /// </summary>
    internal void SetState(IThermostatState newState)
    {
        _currentState = newState;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Determines the next state using the current strategy.
    /// </summary>
    internal IThermostatState DetermineNextState(int ambientTemperature)
    {
        var nextState = CurrentStrategy.DetermineNextState(this, ambientTemperature);
        return nextState;

    }

    /// <summary>
    /// Restores device properties.
    /// </summary>
    internal void RehydrateState(DevicePowerState powerState, IThermostatModeStrategy strategy, int targetTemperature, ThermostatStateType stateType, ThermostatMode mode)
    {
        _powerState = powerState;
        CurrentStrategy = strategy;
        Mode = mode;
        TargetTemperature = targetTemperature;
        _currentState = powerState == DevicePowerState.Off
        ? Off
       : stateType switch
       {
           ThermostatStateType.Idle => Idle,
           ThermostatStateType.Heating => Heating,
           ThermostatStateType.Cooling => Cooling,
           _ => Idle
       };
    }

    /// <summary>
    /// Resets device properties to default settings.
    /// </summary>
    public override void ResetToDefault()
    {
        _powerState = DevicePowerState.Off;
        _currentState = Off;
        TargetTemperature = DefaultTemperature;
        SetMode(defaultMode, new AutoModeStrategy());
        UpdatedAt = DateTime.UtcNow;

    }
}
