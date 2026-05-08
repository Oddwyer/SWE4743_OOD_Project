namespace SmartHome.Infrastructure;

/// <summary>
/// Immutable snapshot of a location's ambient temperature used for JSON persistence.
/// </summary>
public record LocationSnapshot
{
    /// <summary>Location name.</summary>
    public string Location { get; init; } = "";
    /// <summary>Ambient temperature of the location in °F.</summary>
    public int AmbientTemperature { get; init; }
}
