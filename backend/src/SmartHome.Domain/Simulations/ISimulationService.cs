using SmartHome.Domain.Devices.Thermostat;
namespace SmartHome.Domain.Simulations;

/// <summary>
/// Defines operations for managing the ambient temperature simulation and thermostat participation.
/// </summary>
public interface ISimulationService
{
    /// <summary>Registers a thermostat to receive periodic ambient temperature updates.</summary>
    void RegisterThermostat(ThermostatDevice thermostat);

    /// <summary>Removes a thermostat from the simulation.</summary>
    void UnregisterThermostat(ThermostatDevice thermostat);

    /// <summary>Sets the ambient temperature (°F) for the given location; throws if out of range (0–100).</summary>
    void SetAmbientTemperature(string location, int temperature);

    /// <summary>Returns the current ambient temperature (°F) for the given location, or the default (72) if not set.</summary>
    int GetAmbientTemperature(string location);

    /// <summary>Applies one simulation tick, advancing ambient temperatures toward target values.</summary>
    void UpdateAmbientTemperature();

    /// <summary>Updates the tick rate of the simulation timer to the specified speed multiplier.</summary>
    void SetSimulationSpeed(SimulationSpeed speedMultiplier);

    /// <summary>Starts the simulation timer.</summary>
    void StartSimulation();

    /// <summary>Resets all simulation state and restarts the timer at 1× speed.</summary>
    void ResetSimulation();
}
