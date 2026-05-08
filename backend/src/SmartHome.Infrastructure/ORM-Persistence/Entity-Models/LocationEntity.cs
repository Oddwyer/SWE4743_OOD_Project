/// <summary>
/// EF Core entity representing a persisted location with its ambient temperature.
/// Location name serves as the primary key to enforce the one-thermostat-per-location constraint.
/// </summary>
public class LocationEntity
{
    /// <summary>Location name (primary key).</summary>
    public string Location { get; set; } = string.Empty;
    /// <summary>Current ambient temperature of the location in °F.</summary>
    public int AmbientTemperature { get; set; }
}
