namespace SmartHome.Domain.Devices.Fan;

/// <summary>
/// Represents the speed settings of a fan device, such as a ceiling fan or portable fan.
/// </summary>
public enum FanSpeed
{
    Low = 1,
    Medium = 2, //default speed when fan is turned on
    High = 3
}