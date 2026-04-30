namespace SmartHome.Domain.Devices.Fan.FanStates;

/// <summary>
/// The OnState class represents the state of a fan device when it is turned on. 
/// </summary>
public class OnState : IFanState
{
    private readonly FanDevice _fan;

    public OnState(FanDevice fan)
    {
        _fan = fan;
    }

    /// <summary>
    /// Toggles the power state to off. 
    /// </summary>
    public void TogglePower()
    {
        _fan.TurnPowerOff();
        _fan.SetState(_fan.Off);
    }

    /// <summary>
    /// Attempts to set the fan speed while the fan is on. 
    /// </summary>
    public void SetFanSpeed(FanSpeed speed)
    {
        _fan.SetFanSpeedInternal(speed);

    }

}