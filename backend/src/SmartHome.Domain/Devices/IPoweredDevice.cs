namespace SmartHome.Domain.Devices;

/// <summary>
/// Implemented by devices that have an explicit on/off power switch (e.g. lights, fans, thermostats).
/// </summary>
public interface IPoweredDevice
{
    /// <summary>Current power state of the device.</summary>
    DevicePowerState PowerState { get; }

    /// <summary>Toggles between on and off states.</summary>
    void TogglePower();
}
