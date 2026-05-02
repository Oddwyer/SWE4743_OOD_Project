using SmartHome.Domain.Devices.Thermostat;
namespace SmartHome.Domain.Simulations;

/// <summary>
/// Defines operations for managing environment simulation state, including ambient temperature per location.
/// </summary>
public interface ISimulationService
{
    // thermostat interaction methods/points
    void RegisterThermostat(ThermostatDevice thermostat);
    void UnregisterThermostat(ThermostatDevice thermostat);

    // temperature control
    void SetAmbientTemperature(string location, int temperature);
    int GetAmbientTemperature(string location);
    // once the ambient temperature is set, the simulation can use this method to update it without needing to set it again
    void UpdateAmbientTemperature();

    // speed control
    void SetSimulationSpeed(SimulationSpeed speedMultiplier);
    
    // simulation control
    void startSimulation();
    void ResetSimulation();
    double elapsedSimulationSeconds();
}
