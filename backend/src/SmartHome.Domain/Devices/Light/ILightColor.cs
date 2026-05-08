namespace SmartHome.Domain.Devices.Light;

/// <summary>
/// Interface for light color control. Provides methods to change the color of the light and to get the current color state.
/// </summary>
public interface ILightColor
{
    string Color { get; }
    void SetColor(string newColor);
}
