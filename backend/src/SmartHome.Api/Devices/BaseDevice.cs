using SmartHome.Domain.Devices;

namespace SmartHome.Api.Devices;

/// <summary>
/// Abstract DTO for shared properties for reuse by concrete DTO classes.
/// </summary>
public abstract class BaseDevice
{
    /// <summary>Human-readable device name (minimum 2 characters).</summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>Location the device is installed in.</summary>
    public string DeviceLocation { get; set; } = string.Empty;

    /// <summary>Device category.</summary>
    public DeviceType Type { get; set; }
}
