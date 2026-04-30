using SmartHome.Domain.Devices.Fan.FanStates;

namespace SmartHome.Domain.Devices.Fan;

public class FanDevice : Device, IPoweredDevice
{
    // States
    private DevicePowerState _powerState;
    public IFanState Off { get; private set; }
    public IFanState On { get; private set; }
    private IFanState _currentState;

    public FanSpeed Speed { get; private set; } = FanSpeed.Medium;

    public FanDevice(Guid id, string name, string location) : base(id, name, location, DeviceType.Fan)
    {
        _powerState = DevicePowerState.Off;
        Off = new OffState(this);
        On = new OnState(this);
        _currentState = Off;
    }

    /// <summary>
    /// Current power state of the fan.
    /// </summary>
    public DevicePowerState PowerState => _powerState;

    /// <summary>
    /// Indicates whether the fan is on.
    /// </summary>
    public override bool IsDeviceOn => _powerState == DevicePowerState.On;

    public void TogglePower()
    {
        _currentState.TogglePower();
    }

    /// <summary>
    /// Sets the power state to on (used by states).
    /// </summary>
    internal void TurnPowerOn()
    {
        _powerState = DevicePowerState.On;
        UpdatedAt = DateTime.UtcNow;

    }

    /// <summary>
    /// Sets the power state to off (used by states).
    /// </summary>
    internal void TurnPowerOff()
    {
        _powerState = DevicePowerState.Off;
        UpdatedAt = DateTime.UtcNow;

    }

    /// <summary>
    ///  Requests a fan speed change. The current state decides if it is allowed.
    /// </summary>
    public void SetFanSpeed(FanSpeed newSpeed)
    {
        _currentState.SetFanSpeed(newSpeed);
    }

    /// <summary>
    /// Sets the fan speed (used by states).
    /// <summary>
    internal void SetFanSpeedInternal(FanSpeed newSpeed)
    {
        Speed = newSpeed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the current state of the fan (used by state classes to transition between states).
    /// </summary>
    internal void SetState(IFanState newState)
    {
        _currentState = newState;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the status message (used by states). 
    /// </summary>
    internal void UpdateStatusMessageInternal(string message)
    {
        StatusMessage = message;
        UpdatedAt = DateTime.UtcNow;
    }

}

