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
    private int defaultAmbientTemperature = 72;
    public const int MinAmbientTemperature = 0;
    public const int MaxAmbientTemperature = 100;

    private readonly ILocationRepository _locationRepository;
    private readonly SimulationRuntime _runtime;


    public SimulationService(ILocationRepository locationRepository, SimulationRuntime runtime)
    {
        _locationRepository = locationRepository;
        _runtime = runtime;
        _runtime.Ticker.OnTick += OnSimulationTick;
    }

    private void OnSimulationTick()
    {
        UpdateAmbientTemperature();
    }

    /// <summary>
    /// Sets ambient temperature based on client's requested location and temperature.
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

    /// <summary>
    /// Moves ambient temperature up or down based on thermostat state and target temperature.
    /// </summary>
    public void UpdateAmbientTemperature()
    {

        foreach (var thermostat in _runtime.RegisteredThermostats)
        {
            if (!thermostat.IsDeviceOn)
            {
                continue; // Skip if thermostat is off or idle.
            }

            var locationTemperature = GetAmbientTemperature(thermostat.DeviceLocation);
            var currentState = thermostat.CurrentStateType;
            var desiredTemperature = thermostat.TargetTemperature;

            if (currentState is ThermostatStateType.Heating && locationTemperature < desiredTemperature)
            {
                //  Check what the tick is then increase the temp by 1°F per tick until it reaches the desired temp, then switch to idle.
                startSimulation();
                locationTemperature++;
            }
            else if (currentState is ThermostatStateType.Cooling && locationTemperature > desiredTemperature)
            {
                // Check what the tick is then decrease the temp by 1°F  per tick until it reaches the desired temp, then switch to idle.
                startSimulation();
                locationTemperature--;
            }
            else if (currentState is ThermostatStateType.Idle)
            {
                // If thermostat is idle, assume that the ambient temperature is stable and does not need to be updated.
                continue;
            }
            else
            {
                // If the thermostat is on but the ambient temperature has already reached the desired temperature,
                // assume that the ambient temperature is stable and does not need to be updated.
                continue;
            }
        }
    }

    public void SetSimulationSpeed(SimulationSpeed speedMultiplier)
    {
        _runtime.Ticker.setSimulationTickerSpeed(speedMultiplier);
    }

    public void startSimulation()
    {
        _runtime.Ticker.Start();
    }

    public void ResetSimulation()
    {
        _runtime.Ticker.Stop();

        foreach (var location in _ambientTemperatures.Keys.ToList())
        {
            _ambientTemperatures[location] = defaultAmbientTemperature;
            _locationRepository.SaveAmbientTemperature(location, defaultAmbientTemperature);
        }
    }

    public void RegisterThermostat(ThermostatDevice thermostat)
    {
        _runtime.RegisterThermostat(thermostat);

        if (_locationRepository.GetAmbientTemperature(Normalize(thermostat.DeviceLocation)) is null)
        {
            SetAmbientTemperature(thermostat.DeviceLocation, defaultAmbientTemperature);
        }
    }


    public void UnregisterThermostat(ThermostatDevice thermostat)
    {
        _runtime.UnregisterThermostat(thermostat);

    }

    private static string Normalize(string location) => location.Trim().ToLowerInvariant();
}

