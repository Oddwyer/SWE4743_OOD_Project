namespace SmartHome.Domain.Devices.Light;

public interface ILightColor
{
    LightColorState colorState { get; }
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

