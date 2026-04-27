
namespace SmartHome.Domain.Services;

public class SimulationService : ISimulationService
{
    private readonly Dictionary<string, double> _ambientTemperatures = new();

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
    public double GetAmbientTemperature(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            throw new ArgumentException("No location provided.");
        }

        return _ambientTemperatures.TryGetValue(location, out var temperature) ? temperature : 72;
    }
}