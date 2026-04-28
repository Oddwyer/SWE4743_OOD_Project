using SmartHome.Domain.Commands;

namespace SmartHome.Domain.Devices;

public interface IDeviceRepository
{
    IEnumerable<IDevice> FindAllDevices(DeviceFilter filter);
    IDevice? FindDeviceById(Guid deviceId);
    IDevice SaveDevice(IDevice device);
    void DeleteDevice(Guid deviceId);
    bool ThermostatInLocation(string location);

    // Added these so I could persist history. 
    IEnumerable<CommandHistoryEntry> GetHistoryForDevice(Guid deviceId);
    void AddHistoryEntry(CommandHistoryEntry entry);
}