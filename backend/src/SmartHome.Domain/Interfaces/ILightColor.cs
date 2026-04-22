namespace SmartHome.Domain.Interfaces;
public interface ILightColor
{
    LightColorState colorState {get;}
    void ChangeColor(LightColorState newColor);
}

public enum LightColorState
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
    void setLightBrightness(int brightnessPercentage); // brightness level from 0 to 100
    int lightBrightness {get;} // can be used to check current brightness level and for rehydration purposes
}