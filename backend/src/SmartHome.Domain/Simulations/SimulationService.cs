namespace SmartHome.Domain.Simulations;

/// <summary>
/// Handles environment simulation operations such as setting and retrieving ambient temperature per location, 
/// delegating persistence to the repository.
/// </summary>
public class SimulationService : ISimulationService
{
    private readonly Dictionary<string, double> _ambientTemperatures = new();

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

    }

    /// <summary>
    /// Return ambient temperature for a given location.
    /// </summary>
    public double GetAmbientTemperature(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            throw new ArgumentException("No location provided.");
        }

        return _ambientTemperatures.TryGetValue(location, out var temperature) ? temperature : 72;
    }
}