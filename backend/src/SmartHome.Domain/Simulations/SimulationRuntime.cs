using SmartHome.Domain.Locations;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.Thermostat.ThermostatStates;

namespace SmartHome.Domain.Simulations;

/// <summary>
/// Handles environment simulation operations such as setting and retrieving ambient temperature per location, 
/// delegating persistence to the repository.
/// </summary>
public class SimulationRuntime
{
    private readonly Dictionary<string, ThermostatDevice> _registeredThermostats = new();

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
    /// Registers a thermostat to participate in the simulation.
    /// </summary>
    public void RegisterThermostat(ThermostatDevice thermostat)
    {
        if (!_registeredThermostats.ContainsKey(thermostat.Id.ToString()))
        {
            _registeredThermostats[thermostat.Id.ToString()] = thermostat;
        }

        else
        {
            throw new InvalidOperationException("Thermostat is already registered.");
        }
    }

    /// <summary>
    /// Unregisters a thermostat from the simulation.
    /// </summary>
    public void UnregisterThermostat(ThermostatDevice thermostat)
    {
        if (_registeredThermostats.ContainsKey(thermostat.Id.ToString()))
        {
            _registeredThermostats.Remove(thermostat.Id.ToString());
        }
        else
        {
            throw new InvalidOperationException("Thermostat is not registered.");
        }
    }

}

