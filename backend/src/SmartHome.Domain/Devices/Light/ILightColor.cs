namespace SmartHome.Domain.Devices.Light;

public interface ILightColor
{
    LightColorState colorState { get; }
    void ChangeColor(LightColorState newColor);
}

public enum LightColorState // might need to edit to account for the RGB values, but for now this should work for flushing out the logic and structure of the light device
{
    White = 1,
    Red = 2,
    Green = 3,
    Blue = 4,
    Yellow = 5,
    Purple = 6,
    Cyan = 7
}

