namespace SmartHome.Domain.Devices;

/// <summary>
/// Supported smart home device categories.
/// </summary>
public enum DeviceType
{
    /// <summary>A controllable light with adjustable brightness and color.</summary>
    Light = 0,
    /// <summary>A fan with adjustable speed settings.</summary>
    Fan = 1,
    /// <summary>A door lock that can be locked or unlocked.</summary>
    DoorLock = 2,
    /// <summary>A thermostat that regulates room temperature.</summary>
    Thermostat = 3
}
