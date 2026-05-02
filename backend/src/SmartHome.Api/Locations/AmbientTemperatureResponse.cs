namespace SmartHome.Api.Locations;

/// <summary>
/// DTO for returning the ambient temperature of a location.
/// </summary>
public class AmbientTemperatureResponse
{
    public string Location { get; set; } = string.Empty;
    public double AmbientTemperature { get; set; }
}