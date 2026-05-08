namespace SmartHome.Domain.Commands;

/// <summary>
/// Commands that can be sent to smart home devices.
/// </summary>
public enum DeviceCommandType
{
    /// <summary>Toggle the device's power on or off.</summary>
    TogglePower = 0,
    /// <summary>Set the brightness level of a light device (10–100).</summary>
    SetBrightness = 1,
    /// <summary>Set the color of a light device (hex string).</summary>
    SetColor = 2,
    /// <summary>Set the speed of a fan device.</summary>
    SetFanSpeed = 3,
    /// <summary>Set the operating mode of a thermostat device.</summary>
    SetThermostatMode = 4,
    /// <summary>Set the target temperature of a thermostat device (60–80 °F).</summary>
    SetTargetTemperature = 5,
    /// <summary>Toggle the lock state of a door lock device.</summary>
    ToggleLock = 6
}
