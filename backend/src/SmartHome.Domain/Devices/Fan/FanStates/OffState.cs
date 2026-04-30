namespace SmartHome.Domain.Devices.Fan.FanStates;

/// <summary>
/// The OffState class represents the state of a light device when it is turned off. In this state, 
/// the light should not emit any light, and any attempts to change the color or brightness should 
/// have no effect until the power is toggled back on.
/// </summary> 
public class OffState : IFanState
{
    private readonly FanDevice _fan;

    public OffState(FanDevice fan)
    {
        _fan = fan;
    }

    public void TogglePower()
    {
        _fan.TurnPowerOn();
    }

    public void SetFanSpeed(FanSpeed speed)
    {
        // No effect when the fan is off
        _fan.UpdateStatusMessage("Cannot set fan speed when fan is off.");
    }

}