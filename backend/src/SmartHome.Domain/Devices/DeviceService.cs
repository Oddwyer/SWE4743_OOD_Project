using SmartHome.Domain.Commands;

namespace SmartHome.Domain.Devices;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;

    public DeviceService(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public IEnumerable<IDevice> GetAllDevices(DeviceFilter filter)
    {
        return _deviceRepository.FindAllDevices(filter);
    }

    public IDevice? GetDeviceById(Guid deviceId)
    {
        return _deviceRepository.FindDeviceById(deviceId);
    }

    public void RegisterDevice(IDevice device)
    {
        _deviceRepository.SaveDevice(device);
    }

    // TODO - Amber: Revisit once we know constructor params required by concrete DeviceCommand.
    public IDevice ApplyDeviceCommand(Guid deviceId, IDeviceCommand command)
    {
        var device = GetDeviceById(deviceId);

        if (device == null)
        {
            throw new ArgumentException("Device not found.");
        }

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

