using SmartHome.Domain.Commands;
using SmartHome.Domain.Commands.History;
using SmartHome.Domain.Contracts;

namespace SmartHome.Domain.Devices;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IDeviceFactory _deviceFactory;

    private readonly ICommandFactory _commandFactory;

    public DeviceService(IDeviceRepository deviceRepository, IDeviceFactory factory, ICommandFactory commandFactory)
    {
        _deviceRepository = deviceRepository;
        _deviceFactory = factory;
        _commandFactory = commandFactory;
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
    public IDevice ApplyDeviceCommand(Guid deviceId, CommandData context)
    {
        var device = _deviceRepository.FindDeviceById(deviceId);

        if (device == null)
        {
            throw new KeyNotFoundException($"Device with ID {deviceId} was not found.");
        }

        var command = _commandFactory.CreateCommand(device, context);
        command.Execute();

        _deviceRepository.SaveDevice(device);
        _deviceRepository.SaveHistoryEntry(new CommandHistoryEntry(deviceId, command));

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

        return _deviceRepository.GetHistoryForDevice(deviceId);
    }


}

