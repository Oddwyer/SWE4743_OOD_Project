namespace SmartHome.Domain.Devices;
public interface ILightColor
{
    LightColorState colorState {get;}
    void ChangeColor(LightColorState newColor);
}

public enum LightColorState // might need to edit to account for the RGB values, but for now this should work for flushing out the logic and structure of the light device
{
    White,
    Red,
    Green,
    Blue,
    Yellow,
    Purple,
    Cyan
}

public interface IDimLights
{
    void setLightBrightness(int brightnessPercentage); // brightness level from 10 to 100
    int lightBrightness {get;} // can be used to check current brightness level and for rehydration purposes
}