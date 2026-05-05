using SmartHome.Domain.Locations;

namespace SmartHome.Domain.Simulations;

/// <summary>
/// Handles environment simulation operations such as setting and retrieving ambient temperature per location, 
/// delegating persistence to the repository.
/// </summary>
public class SimulationService : ISimulationService
{
    private int defaultAmbientTemperature = 72;
    public const int MinAmbientTemperature = 0;
    public const int MaxAmbientTemperature = 100;

    private readonly ILocationRepository _locationRepository;

    public SimulationService(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    /// <summary>
    /// Set ambient temperature based on client's requested location and temperature.
    /// </summary>
    public void SetAmbientTemperature(string location, int temperature)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            throw new ArgumentException("No location provided.");
        }

        if (temperature < MinAmbientTemperature || temperature > MaxAmbientTemperature)
        {
            throw new ArgumentOutOfRangeException(nameof(temperature), $"Temperature must be between {MinAmbientTemperature}°F and {MaxAmbientTemperature}°F.");
        }

        _locationRepository.SaveAmbientTemperature(Normalize(location), temperature);

    }

    /// <summary>
    /// /// Returns the ambient temperature for a given location, or a default value if none is stored.
    /// </summary>
    public int GetAmbientTemperature(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            throw new ArgumentException("No location provided.");
        }

        var ambientTemperature = _locationRepository.GetAmbientTemperature(Normalize(location));

        return ambientTemperature ?? defaultAmbientTemperature;
    }

    public void SetSimulationSpeed(SimulationSpeed speedMultiplier)
    {
        throw new NotImplementedException(
            "Simulation speed is pending simulation engine implementation.");
    }

    public void ResetSimulation()
    {
        throw new NotImplementedException(
            "Simulation reset is pending reset behavior implementation.");
    }

    private static string Normalize(string location) => location.Trim().ToLowerInvariant();
}
