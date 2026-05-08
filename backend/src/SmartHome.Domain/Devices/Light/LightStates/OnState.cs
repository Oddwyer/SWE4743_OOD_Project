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

    /// <summary>
    /// Changes the color of the light. Valid color values are expected to be in HEX format (e.g., #FF8800).
    /// </summary>
    public void ChangeColor(string color)
    {
        _light.SetColorInternal(color);
    }

    /// <summary>
    /// Sets the brightness of the light. Valid brightness values are between 10% and 100%.
    /// </summary>
    public void SetLightBrightness(int brightness)
    {

        if (brightness < LightDevice.MinBrightness || brightness > LightDevice.MaxBrightness)
        {
            throw new ArgumentOutOfRangeException(nameof(brightness), $"Brightness must be between {LightDevice.MinBrightness}% and {LightDevice.MaxBrightness}%.");

        }

        _light.SetLightBrightnessInternal(brightness);
    }
}
