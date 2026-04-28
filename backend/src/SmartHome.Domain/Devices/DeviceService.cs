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
    /// Returns device matching given ID.
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    public IDevice? GetDeviceById(Guid deviceId)
    {
        return _deviceRepository.FindDeviceById(deviceId);
    }

    public void RegisterDevice(IDevice device)
    {
        _deviceRepository.SaveDevice(device);
    }

    public IDevice ApplyDeviceCommand(Guid deviceId, IDeviceCommand command)
    {
        var device = GetDeviceById(deviceId);

        if (device == null)
        {
            throw new ArgumentException("Device not found.");
        }

        // TODO - if we need to change to .Execute(device).
        command.Execute();

        _deviceRepository.SaveDevice(device);
        _deviceRepository.AddHistoryEntry(new CommandHistoryEntry(deviceId, command));

        return device;
    }

    public void RemoveDevice(Guid deviceId)
    {
        var device = GetDeviceById(deviceId);

        if (device == null)
        {
            throw new KeyNotFoundException("Device not found.");
        }
        _deviceRepository.DeleteDevice(deviceId);
    }

    public IEnumerable<CommandHistoryEntry> GetCommandHistory(Guid deviceId)
    {
        return _deviceRepository.GetHistoryForDevice(deviceId);
    }
}

