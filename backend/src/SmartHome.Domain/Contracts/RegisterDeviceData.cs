using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Contracts;

/// <summary>
/// Data required to register a new device in the system.
/// </summary>
public class RegisterDeviceData
{
    /// <summary>Human-readable device name (minimum 2 characters).</summary>
    public required string DeviceName { get; init; }
    /// <summary>Location the device will be installed in.</summary>
    public required string DeviceLocation { get; init; }
    /// <summary>Device category to register.</summary>
    public DeviceType DeviceType { get; init; }
}
