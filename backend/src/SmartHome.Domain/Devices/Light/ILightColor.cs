namespace SmartHome.Domain.Devices.Light;

public interface ILightColor
{
    LightColorState colorState { get; }
    void ChangeColor(LightColorState newColor);
}
