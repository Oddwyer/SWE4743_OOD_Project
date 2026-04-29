using SmartHome.Domain.Commands;

namespace SmartHome.Domain.Devices;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;

    public DeviceService(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
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
    public void RegisterDevice(IDevice device)
    {
        _deviceRepository.SaveDevice(device);
    }

    /// <summary>
    /// Apply client command request to device.
    /// </summary>
    public IDevice ApplyDeviceCommand(Guid deviceId, IDeviceCommand command)
    {
        var device = GetDeviceById(deviceId);

        if (device == null)
        {
            throw new KeyNotFoundException($"Device with ID {deviceId} was not found.");
        }

        // TODO - Kataali: Update to .Execute(device) if needed.
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

