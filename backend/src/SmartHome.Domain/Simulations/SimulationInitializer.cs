using SmartHome.Domain.Simulations;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Thermostat;

public class SimulationInitializer
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly ISimulationService _simulationService;

    public SimulationInitializer(
        IDeviceRepository deviceRepository,
        ISimulationService simulationService)
    {
        _deviceRepository = deviceRepository;
        _simulationService = simulationService;
    }

    public void Initialize()
    {
        var devices = _deviceRepository.FindAllDevices(new DeviceFilter());

        foreach (var thermostat in devices.OfType<ThermostatDevice>())
        {
            _simulationService.RegisterThermostat(thermostat);
        }
    }
}