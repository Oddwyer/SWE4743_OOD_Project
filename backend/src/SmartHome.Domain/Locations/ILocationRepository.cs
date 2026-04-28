namespace SmartHome.Domain.Locations;

public interface ILocationRepository
{
    public int? GetAmbientTemperature(string location);
    public void SaveAmbientTemperature(string location, int temperature);
}