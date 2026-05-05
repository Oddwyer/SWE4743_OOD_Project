namespace SmartHome.Api.Locations;

/// <summary>
/// DTO for returning the ambient temperature of a location.
/// </summary>
public class AmbientTemperatureResponse
{
    public string Location { get; set; } = string.Empty;
    public int AmbientTemperature { get; set; }
    public int MinTemperature { get; set; }
    public int MaxTemperature { get; set; }

}