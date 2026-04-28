namespace SmartHome.Domain.Simulations;

/// <summary>
/// Defines operations for managing environment simulation state, including ambient temperature per location.
/// </summary>
public interface ISimulationService
{
    void SetAmbientTemperature(string location, int temperature);
    double GetAmbientTemperature(string location);
}