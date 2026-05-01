namespace SmartHome.Domain.Devices.Light.LightStates;

/// <summary>
/// The OffState class represents the state of a light device when it is turned off. In this state, 
/// the light should not emit any light, and any attempts to change the color or brightness should 
/// have no effect until the power is toggled back on.
/// </summary> 
public class OffState : ILightState
{
    private readonly LightDevice light;

    public OffState(LightDevice light)
    {
        this.light = light;
    }

    public void TogglePower()
    {
        light.TurnPowerOn();
        light.SetState(light.On);
    }

    public void ChangeColor(LightColor color)
    {

        throw new InvalidOperationException("Cannot change color when light is off.");
    }

    public void SetLightBrightness(int brightness)
    {
        throw new InvalidOperationException("Cannot set brightness when light is off.");
    }
}