namespace SmartHome.Domain.Locations;

/// <summary>
/// Defines data-access operations for location ambient temperature data.
/// </summary>
public interface ILocationRepository
{
    /// <summary>Returns the stored ambient temperature (°F) for the given location, or null if not found.</summary>
    public int? GetAmbientTemperature(string location);

    /// <summary>Persists the ambient temperature (°F) for the given location.</summary>
    public void SaveAmbientTemperature(string location, int temperature);
}
