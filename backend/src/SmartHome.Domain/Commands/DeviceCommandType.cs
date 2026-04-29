namespace SmartHome.Domain.Commands;

/// <summary>
/// Enum representing the different types of commands that can be sent to devices in the smart home system.
/// </summary>
public enum DeviceCommandType
{
    TogglePower,
    SetBrightness,
    SetColor,
    SetFanSpeed,
    SetThermostatMode,
    SetDesiredTemperature,
    ToggleLock
}