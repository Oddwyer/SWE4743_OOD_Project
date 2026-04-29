namespace SmartHome.Domain.Devices.Light;

/// <summary>
/// Interface for dimmable lights. Provides methods to set and get the brightness level of the light.
/// </summary>
public interface IDimLights
{
    void SetLightBrightness(int brightnessPercentage); // brightness level from 10 to 100
    int lightBrightness { get; } // can be used to check current brightness level and for rehydration purposes
}