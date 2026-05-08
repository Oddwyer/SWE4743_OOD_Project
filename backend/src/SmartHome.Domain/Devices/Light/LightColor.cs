namespace SmartHome.Domain.Devices.Light;

// might need to edit to account for the RGB values, but for now this should work for 
//flushing out the logic and structure of the light device

/// <summary>
/// Represents the color state of a light device, such as a smart bulb that can change colors.
/// </summary>
public enum LightColor
{
    White = 1,
    Red = 2,
    Green = 3,
    Blue = 4,
    Yellow = 5,
    Purple = 6,
    Cyan = 7
}

