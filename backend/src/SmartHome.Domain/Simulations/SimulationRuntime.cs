using SmartHome.Domain.Locations;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.Thermostat.ThermostatStates;
using System.Collections.Concurrent;

namespace SmartHome.Domain.Simulations;

/// <summary>
/// Handles environment simulation operations such as setting and retrieving ambient temperature per location, 
/// delegating persistence to the repository.
/// </summary>
public class SimulationRuntime
{
    // Thermostat registry is shared across multiple threads -> made concurrent.
    private readonly ConcurrentDictionary<string, ThermostatDevice> _registeredThermostats = new();

    public SimulationTicker Ticker { get; }

    /// <summary>
    /// Initializes runtime state with the provided simulation ticker.
    /// </summary>
    public SimulationRuntime(SimulationTicker ticker)
    {
        Ticker = ticker;
    }

    /// <summary>
    /// Gets all currently registered thermostats participating in the simulation.
    /// </summary>
    public IEnumerable<ThermostatDevice> RegisteredThermostats => _registeredThermostats.Values;

    /// <summary>
    /// Registers a thermostat to participate in the simulation; replaced stale thermostat objects.
    /// </summary>
    public void RegisterThermostat(ThermostatDevice thermostat)
    {
        _registeredThermostats[thermostat.Id.ToString()] = thermostat;
    }

    /// <summary>
    /// Unregisters a thermostat from the simulation.
    /// </summary>
    public void UnregisterThermostat(ThermostatDevice thermostat)
    {
        _registeredThermostats.TryRemove(thermostat.Id.ToString(), out _);
    }

}

