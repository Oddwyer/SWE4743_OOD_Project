public class LocationEntity
{
    // the location serves as a PK since we need to enforce "one thermostat per location""
    public string Location { get; set; } = string.Empty;
    public int AmbientTemperature { get; set; }
}