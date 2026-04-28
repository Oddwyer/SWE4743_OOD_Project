namespace SmartHome.Domain.Locations;

public record LocationEnvironmentSnapshot
{
    public string Location { get; init; } = "";
    public int AmbientTemperature { get; init; }
}