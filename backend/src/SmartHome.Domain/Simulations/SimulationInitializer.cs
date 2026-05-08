using SmartHome.Domain.Simulations;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Thermostat;

/// <summary>
/// Loads persisted devices at startup and enrolls thermostats into the simulation before the first tick.
/// </summary>
public class SimulationInitializer
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly ISimulationService _simulationService;

    /// <summary>Initializes the loader with the device repository and simulation service.</summary>
    public SimulationInitializer(
        IDeviceRepository deviceRepository,
        ISimulationService simulationService)
    {
        _deviceRepository = deviceRepository;
        _simulationService = simulationService;
    }

    /// <summary>Registers all persisted thermostats with the simulation and starts the simulation timer.</summary>
    public void Initialize()
    {
        var devices = _deviceRepository.FindAllDevices(new DeviceFilter());

        foreach (var thermostat in devices.OfType<ThermostatDevice>())
        {
            _simulationService.RegisterThermostat(thermostat);
        }

        _simulationService.StartSimulation();
    }
}
