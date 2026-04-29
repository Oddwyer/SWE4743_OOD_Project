namespace SmartHome.Domain.Commands;

/// <summary>
/// Enum representing the different types of commands that can be sent to devices in the smart home system.
/// </summary>
public enum DeviceCommandType
{
    TogglePower = 0,
    SetBrightness = 1,
    SetColor = 2,
    SetFanSpeed = 3,
    SetThermostatMode = 4,
    SetDesiredTemperature = 5,
    ToggleLock = 6
}