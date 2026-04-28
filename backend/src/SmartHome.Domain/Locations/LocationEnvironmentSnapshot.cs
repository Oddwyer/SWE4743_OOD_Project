namespace SmartHome.Domain.Locations;

public record LocationEnvironmentSnapshot
{
    public string Location { get; set; } = "";
    public int AmbientTemperature { get; set; }
}