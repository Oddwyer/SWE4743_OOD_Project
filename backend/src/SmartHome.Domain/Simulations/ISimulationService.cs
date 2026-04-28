using SmartHome.Api.Simulations;

namespace SmartHome.Domain.Simulations;

/// <summary>
/// Defines operations for managing environment simulation state, including ambient temperature per location.
/// </summary>
public interface ISimulationService
{
    void SetAmbientTemperature(string location, int temperature);
    int GetAmbientTemperature(string location);

    // TODO - Kataali: Implement full simulation behavior: tick timing,
    // thermostat Heating/Cooling/Idle transitions, simulation speed, and reset rules.
    void SetSimulationSpeed(SimulationSpeed speedMultiplier);
    void ResetSimulation();
}