namespace SmartHome.Domain.Devices.Light.LightStates;

/// <summary>
/// The OffState class represents the state of a light device when it is turned off. In this state, 
/// the light should not emit any light, and any attempts to change the color or brightness should 
/// have no effect until the power is toggled back on.
/// </summary> 
public class OnState : ILightState
{
    private readonly LightDevice light;

    public OnState(LightDevice light)
    {
        this.light = light;
    }

    public void TogglePower()
    {
        light.TurnPowerOff();
    }

    public void ChangeColor(LightColor color)
    {
        light.ChangeColorInternal(color);
    }

    public void SetLightBrightness(int brightness)
    {
        if (brightness < 10 || brightness > 100)
        {
            light.UpdateStatusMessage("Brightness must be between 10 and 100.");
            return;
        }
        light.SetLightBrightnessInternal(brightness);
    }
}