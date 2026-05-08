namespace SmartHome.Domain.Devices;

/// <summary>
/// Filter criteria for querying devices from the repository.
/// </summary>
public class DeviceFilter
{
    /// <summary>Filter by device category when provided.</summary>
    public DeviceType? Type { get; set; }
    /// <summary>Filter by location name when provided.</summary>
    public string? Location { get; set; }
    /// <summary>Filter by on/off state when provided.</summary>
    public bool? IsOn { get; set; }
}
