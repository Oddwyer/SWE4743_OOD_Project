namespace SmartHome.Infrastructure;

public record LocationSnapshot
{
    public string Location { get; init; } = "";
    public int AmbientTemperature { get; init; }
}