namespace SmartHome.Domain.Devices.Light.LightStates;

/// <summary>
/// The ILightState interface defines the contract for the different states of a light device. 
/// Each state will implement this interface to provide specific behavior for toggling power, changing color, and 
/// setting brightness based on the current state of the light (e.g., On, Off, Dimmed, etc.).
/// </summary>
public interface ILightState
{
    void TogglePower();
    void ChangeColor(LightColor color);
    void SetLightBrightness(int brightness);

}