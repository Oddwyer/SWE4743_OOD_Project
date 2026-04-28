
using SmartHome.Domain.Locations;

namespace SmartHome.Domain.Simulations;

/// <summary>
/// Handles environment simulation operations such as setting and retrieving ambient temperature per location, 
/// delegating persistence to the repository.
/// </summary>
public class SimulationService : ISimulationService
{
    private const int DefaultAmbientTemperature = 72;
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

        if (temperature < 0 || temperature > 120)
        {
            throw new ArgumentOutOfRangeException(nameof(temperature), "Temperature must be between 0 and 120.");
        }

        _locationRepository.SaveAmbientTemperature(location, temperature);

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

        return _locationRepository.GetAmbientTemperature(location) ?? DefaultAmbientTemperature;
    }
}