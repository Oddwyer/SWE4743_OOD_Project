using SmartHome.Domain.Locations;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.Thermostat.ThermostatStates;

namespace SmartHome.Domain.Simulations;

/// <summary>
/// Handles environment simulation operations such as setting and retrieving ambient temperature per location, 
/// delegating persistence to the repository.
/// </summary>
public class SimulationService : ISimulationService
{
    private readonly Dictionary<string, int> _ambientTemperatures = new();
    private readonly Dictionary<string, ThermostatDevice> _registeredThermostats = new Dictionary<string, ThermostatDevice>();
    private int defaultAmbientTemperature = 72;
    public const int MinAmbientTemperature = 0;
    public const int MaxAmbientTemperature = 100;

    private readonly ILocationRepository _locationRepository;
    private readonly SimulationTicker _ticker;

    public SimulationService(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
        _ticker = new SimulationTicker();
        _ticker.OnTick += OnSimulationTick;
    }

    private void OnSimulationTick()
    {
        UpdateAmbientTemperature();
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

        if (temperature < MinAmbientTemperature || temperature > MaxAmbientTemperature)
        {
            throw new ArgumentOutOfRangeException(nameof(temperature), $"Temperature must be between {MinAmbientTemperature}°F and {MaxAmbientTemperature}°F.");
        }

        _locationRepository.SaveAmbientTemperature(Normalize(location), temperature);

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

        var ambientTemperature = _locationRepository.GetAmbientTemperature(Normalize(location));

        return ambientTemperature ?? defaultAmbientTemperature;
    }

    public void UpdateAmbientTemperature()
    {
        // this should move the temp up or down based on the current state, strategy and the tick rate
        foreach (var thermostat in _registeredThermostats.Values)
        {
            if (!thermostat.IsDeviceOn)
            {
                continue; // skip if thermostat is off or idle
            }

            var locationTemperature = GetAmbientTemperature(thermostat.DeviceLocation);
            var currentState = thermostat.CurrentStateType;
            var desiredTemperature = thermostat.TargetTemperature;

            if (currentState is ThermostatStateType.Heating && locationTemperature < desiredTemperature)
            {
                // need to check what the tick is then increase the temp by 1 degree F per tick until it reaches the desired temp, then it should switch to idle
                startSimulation();
                locationTemperature++;
            }
            else if (currentState is ThermostatStateType.Cooling && locationTemperature > desiredTemperature)
            {
                // need to check what the tick is then decrease the temp by 1 degree F per tick until it reaches the desired temp, then it should switch to idle
                startSimulation();
                locationTemperature--;
            }
            else if (currentState is ThermostatStateType.Idle)
            {
                // if the thermostat is idle, we can assume that the ambient temperature is stable
                // and does not need to be updated
                continue;
            }
            else
            {
                // if the thermostat is on but the ambient temperature has already reached the desired temperature,
                //  we can assume that the ambient temperature is stable and does not need to be updated
                continue;
            }
        }
    }

    public void SetSimulationSpeed(SimulationSpeed speedMultiplier)
    {
        _ticker.setSimulationTickerSpeed(speedMultiplier);
    }

    public void startSimulation()
    {
        _ticker.Start();
    }

    public void ResetSimulation()
    {
        _ticker.Stop();

        foreach (var location in _ambientTemperatures.Keys.ToList())
        {
            _ambientTemperatures[location] = defaultAmbientTemperature;
            _locationRepository.SaveAmbientTemperature(location, defaultAmbientTemperature);
        }
    }

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

    private static string Normalize(string location) => location.Trim().ToLowerInvariant();
}

