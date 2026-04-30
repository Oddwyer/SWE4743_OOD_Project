namespace SmartHome.Domain.Devices.Fan.FanStates;

/// <summary>
/// The OffState class represents the state of a fan device when it is turned off.
/// </summary>
public class OffState : IFanState
{
    private readonly FanDevice _fan;

    public OffState(FanDevice fan)
    {
        _fan = fan;
    }

    /// <summary>
    /// Toggles the power state to on.
    /// </summary>
    public void TogglePower()
    {
        _fan.TurnPowerOn();
        _fan.SetState(_fan.On);
    }

    /// <summary>
    /// Attempts to set the fan speed while the fan is off. This should have no effect.
    /// </summary>
    public void SetFanSpeed(FanSpeed speed)
    {
        // No effect when the fan is off
        _fan.UpdateStatusMessage("Cannot set fan speed when fan is off.");
    }

}