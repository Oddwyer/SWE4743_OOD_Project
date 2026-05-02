using SmartHome.Domain.Commands;
using SmartHome.Domain.Commands.History;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Locations;
using SmartHome.Domain.Simulations;

namespace SmartHome.Domain.Devices;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IDeviceFactory _deviceFactory;

    private readonly ICommandFactory _commandFactory;
    private readonly ISimulationService _simulationService;

    public DeviceService(ISimulationService simulationService, IDeviceRepository deviceRepository, IDeviceFactory factory, ICommandFactory commandFactory, ILocationRepository locationRepository)
    {
        _deviceRepository = deviceRepository;
        _deviceFactory = factory;
        _commandFactory = commandFactory;
        _locationRepository = locationRepository;
        _simulationService = simulationService;
    }

    /// <summary>
    /// Returns all devices with/without filtered input.
    /// </summary>
    public IEnumerable<IDevice> GetAllDevices(DeviceFilter filter)
    {
        return _deviceRepository.FindAllDevices(filter);
    }

    /// <summary>
    /// Returns device matching device ID.
    /// </summary>
    public IDevice GetDeviceById(Guid deviceId)
    {
        var device = _deviceRepository.FindDeviceById(deviceId);
        if (device == null)
        {
            throw new KeyNotFoundException($"Device with ID {deviceId} was not found.");
        }
        return device;
    }

    /// <summary>
    /// Registers new device to repository.
    /// </summary>
    public IDevice RegisterDevice(RegisterDeviceData register)
    {
        // Enforce single thermostat per location rule.
        if (_deviceRepository.ThermostatInLocation(register.DeviceLocation) && register.DeviceType == DeviceType.Thermostat)
        {
            throw new InvalidOperationException($"A thermostat already exists in location {register.DeviceLocation}.");
        }
        var device = _deviceFactory.CreateDevice(register.DeviceName, register.DeviceLocation, register.DeviceType);
        _deviceRepository.SaveDevice(device);
        return device;
    }

    /// <summary>
    /// Apply client command request to device.
    /// </summary>
    public IDevice ApplyDeviceCommand(Guid deviceId, CommandData commandData)
    {
        var device = _deviceRepository.FindDeviceById(deviceId);

        if (device == null)
        {
            throw new KeyNotFoundException($"Device with ID {deviceId} was not found.");
        }

        var command = _commandFactory.CreateCommand(device, commandData);
        command.Execute();

        if (device is ThermostatDevice thermostat && thermostat.PowerState == DevicePowerState.On)
        {
            var ambient = _simulationService.GetAmbientTemperature(thermostat.DeviceLocation);
            thermostat.Evaluate(ambient);
        }
        _deviceRepository.SaveDevice(device);
        _deviceRepository.SaveHistoryEntry(new CommandHistoryEntry(device.Id, command));

        return device;
    }


    /// <summary>
    /// Remove device with matching device ID. 
    /// </summary>
    public void RemoveDevice(Guid deviceId)
    {
        var device = GetDeviceById(deviceId);

        if (device == null)
        {
            throw new KeyNotFoundException($"Device with ID {deviceId} was not found.");
        }

        _deviceRepository.DeleteDevice(deviceId);
    }

    /// <summary>
    /// Return command history for device with matching device ID.
    /// </summary>
    public IEnumerable<CommandHistoryEntry> GetCommandHistory(Guid deviceId)
    {
        var device = _deviceRepository.FindDeviceById(deviceId);

        if (device == null)
        {
            throw new KeyNotFoundException($"Device with ID {deviceId} was not found.");
        }

        return _deviceRepository.GetHistoryForDevice(deviceId).OrderByDescending(entry => entry.Timestamp);
    }


}

