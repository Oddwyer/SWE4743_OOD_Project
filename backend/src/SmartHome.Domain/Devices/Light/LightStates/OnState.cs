namespace SmartHome.Domain.Devices.Light.LightStates;

/// <summary>
/// The OnState class represents the state of the light when it is turned on. 
/// In this state, the light can have its color changed and brightness adjusted.
/// </summary>
public class OnState : ILightState
{
    private readonly LightDevice _light;

    public OnState(LightDevice light)
    {
        _light = light;
    }

    public void TogglePower()
    {
        _light.TurnPowerOff();
        _light.SetState(_light.Off);
    }

    public void ChangeColor(LightColor color)
    {
        _light.ChangeColorInternal(color);
    }

    public void SetLightBrightness(int brightness)
    {

        if (brightness < LightDevice.MinBrightness || brightness > LightDevice.MaxBrightness)
        {
            _light.UpdateStatusMessage("Brightness must be between 10 and 100.");
            return;
        }

        _light.SetLightBrightnessInternal(brightness);
    }
}