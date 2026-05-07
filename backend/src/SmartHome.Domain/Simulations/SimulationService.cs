using SmartHome.Domain.Locations;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.Thermostat.ThermostatStates;
using System.Collections;
using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Simulations;

/// <summary>
/// Orchestrates simulation behavior by coordinating runtime state and ambient temperature persistence.
/// </summary>
public class SimulationService : ISimulationService
{
    public const int DefaultAmbientTemperature = 72;
    public const int MinAmbientTemperature = 0; // Minimum allowed ambient temperature (°F).
    public const int MaxAmbientTemperature = 100; // Maximum allowed ambient temperature (°F).

    private readonly ILocationRepository _locationRepository;

    private readonly IDeviceRepository _deviceRepository;
    private readonly SimulationRuntime _runtime;


    public SimulationService(ILocationRepository locationRepository, IDeviceRepository deviceRepository, SimulationRuntime runtime)
    {
        _locationRepository = locationRepository;
        _deviceRepository = deviceRepository;
        _runtime = runtime;
        _runtime.Ticker.OnTick += OnSimulationTick;
    }

    /// <summary>
    /// Handles simulation tick events by updating ambient temperatures.
    /// </summary>
    private void OnSimulationTick()
    {
        UpdateAmbientTemperature();
    }

    /// <summary>
    /// Sets the ambient temperature for a given location.
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
    /// Gets the ambient temperature for a location or returns a default if none exists.
    /// </summary>
    public int GetAmbientTemperature(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            throw new ArgumentException("No location provided.");
        }

        var ambientTemperature = _locationRepository.GetAmbientTemperature(Normalize(location));

        return ambientTemperature ?? DefaultAmbientTemperature;
    }

    /// <summary>
    /// Updates ambient temperatures based on thermostat state and target temperature.
    /// </summary>
    public void UpdateAmbientTemperature()
    {

        foreach (var thermostat in _runtime.RegisteredThermostats)
        {
            if (!thermostat.IsDeviceOn)
            {
                continue; // Skip if thermostat is off or idle.
            }
            var location = Normalize(thermostat.DeviceLocation);
            var locationTemperature = GetAmbientTemperature(location);
            var currentState = thermostat.CurrentStateType;
            var desiredTemperature = thermostat.TargetTemperature;

            if (currentState is ThermostatStateType.Heating && locationTemperature < desiredTemperature)
            {
                locationTemperature++;
                _locationRepository.SaveAmbientTemperature(location, locationTemperature);
                thermostat.Evaluate(locationTemperature);
                _deviceRepository.SaveDevice(thermostat);

            }
            else if (currentState is ThermostatStateType.Cooling && locationTemperature > desiredTemperature)
            {
                locationTemperature--;
                _locationRepository.SaveAmbientTemperature(location, locationTemperature);
                thermostat.Evaluate(locationTemperature);
                _deviceRepository.SaveDevice(thermostat);
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

    /// <summary>
    /// Sets the simulation speed multiplier.
    /// </summary>
    public void SetSimulationSpeed(SimulationSpeed speedMultiplier)
    {
        _runtime.Ticker.SetSimulationTickerSpeed(speedMultiplier);
    }

    /// <summary>
    /// Starts the simulation ticker.
    /// </summary>
    public void StartSimulation()
    {
        _runtime.Ticker.Start();
    }

    /// <summary>
    /// Resets the simulation by stopping the ticker and sets all devices to default settings.
    /// </summary>
    public void ResetSimulation()
    {
        _runtime.Ticker.Stop();
        _runtime.Ticker.SetSimulationTickerSpeed(SimulationSpeed.OneX);

        var devices = _deviceRepository.FindAllDevices(new DeviceFilter());

        foreach (var device in devices)
        {
            device.ResetToDefault();
            _deviceRepository.SaveDevice(device);
        }

    }

    /// <summary>
    /// Registers a thermostat with the simulation and ensures its location has an ambient temperature.
    /// </summary>
    public void RegisterThermostat(ThermostatDevice thermostat)
    {
        _runtime.RegisterThermostat(thermostat);

        if (_locationRepository.GetAmbientTemperature(Normalize(thermostat.DeviceLocation)) is null)
        {
            SetAmbientTemperature(thermostat.DeviceLocation, DefaultAmbientTemperature);
        }
    }

    /// <summary>
    /// Unregisters a thermostat from the simulation.
    /// </summary>
    public void UnregisterThermostat(ThermostatDevice thermostat)
    {
        _runtime.UnregisterThermostat(thermostat);

    }

    /// <summary>
    /// Normalizes location strings for consistent storage and lookup.
    /// </summary>
    private static string Normalize(string location) => location.Trim().ToLowerInvariant();
}

