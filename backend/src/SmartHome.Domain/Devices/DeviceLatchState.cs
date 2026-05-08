namespace SmartHome.Domain.Devices;

/// <summary>
/// Represents the latch state of a device such as a door lock.
/// </summary>
public enum DeviceLatchState
{
    /// <summary>The device is secured/locked.</summary>
    Locked,
    /// <summary>The device is open/unlocked.</summary>
    Unlocked
}
