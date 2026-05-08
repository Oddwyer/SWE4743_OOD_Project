namespace SmartHome.Domain.Devices.Fan;

/// <summary>
/// Speed settings for a fan device.
/// </summary>
public enum FanSpeed
{
    /// <summary>Fan is not spinning (powered down).</summary>
    Off = 0,
    /// <summary>Fan is spinning at low speed.</summary>
    Low = 1,
    /// <summary>Fan is spinning at medium speed (default when powered on).</summary>
    Medium = 2,
    /// <summary>Fan is spinning at high speed.</summary>
    High = 3
}
