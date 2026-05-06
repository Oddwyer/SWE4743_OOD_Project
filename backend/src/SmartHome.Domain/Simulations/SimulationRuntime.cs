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
    private readonly Dictionary<string, int> _ambientTemperatures = new();
    private readonly Dictionary<string, ThermostatDevice> _registeredThermostats = new();
    public SimulationTicker Ticker { get; }

    public SimulationRuntime(SimulationTicker ticker)
    {
        Ticker = ticker;
    }

    public IEnumerable<ThermostatDevice> RegisteredThermostats => _registeredThermostats.Values;
    public void RegisterThermostat(ThermostatDevice thermostat)
    {
        if (!_registeredThermostats.ContainsKey(thermostat.Id.ToString()))
        {
            _registeredThermostats[thermostat.Id.ToString()] = thermostat;

            if (!_ambientTemperatures.ContainsKey(thermostat.DeviceLocation))
            {
                SetAmbientTemperature(thermostat.DeviceLocation, defaultAmbientTemperature);
            }
        }

        else
        {
            throw new InvalidOperationException("Thermostat is already registered.");
        }
    }

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

