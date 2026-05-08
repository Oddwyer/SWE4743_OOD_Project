namespace SmartHome.Domain.Devices;

/// <summary>
/// Implemented by devices that can be locked and unlocked (e.g. door locks).
/// </summary>
public interface ILatchedDevice
{
    /// <summary>Current lock state of the device.</summary>
    DeviceLatchState LatchState { get; }

    /// <summary>Toggles between locked and unlocked states.</summary>
    void ToggleLock();
}
