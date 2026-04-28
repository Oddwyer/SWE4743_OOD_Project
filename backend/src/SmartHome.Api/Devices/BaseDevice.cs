using SmartHome.Domain.Devices;

namespace SmartHome.Api.Devices;

/// <summary>
/// Abstract DTO for shared properties for reuse by concrete DTO classes.
/// </summary>

public abstract class BaseDevice
{

    public string DeviceName { get; set; } = string.Empty;

    public string DeviceLocation { get; set; } = string.Empty;

    public DeviceType Type { get; set; }

}
