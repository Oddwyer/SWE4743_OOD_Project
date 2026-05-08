/// <summary>
/// Represents a named physical location with an associated ambient temperature.
/// </summary>
public class Location
{
    /// <summary>Location name (used as the unique identifier).</summary>
    public string Name { get; init; } = string.Empty;
    /// <summary>Current ambient temperature of the location in °F.</summary>
    public int AmbientTemperature { get; init; }
}
