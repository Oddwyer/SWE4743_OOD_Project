namespace SmartHome.Api.Locations;

/// <summary>
/// DTO for returning the ambient temperature of a location.
/// </summary>
public class AmbientTemperatureResponse
{
    /// <summary>Location name.</summary>
    public string Location { get; set; } = string.Empty;
    /// <summary>Current ambient temperature in °F.</summary>
    public int AmbientTemperature { get; set; }
    /// <summary>Minimum allowed ambient temperature (°F).</summary>
    public int MinTemperature { get; set; }
    /// <summary>Maximum allowed ambient temperature (°F).</summary>
    public int MaxTemperature { get; set; }
    /// <summary>Default ambient temperature used on reset (°F).</summary>
    public int DefaultTemperature { get; set; }
}