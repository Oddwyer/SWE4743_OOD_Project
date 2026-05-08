namespace SmartHome.Api.Locations;

/// <summary>
/// DTO for setting the ambient temperature of a location.
/// </summary>

public class AmbientTemperatureRequest
{
    /// <summary>New ambient temperature in °F (0–100).</summary>
    public int Temperature { get; set; }
}
