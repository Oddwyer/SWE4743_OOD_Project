using SmartHome.Domain.Commands;

namespace SmartHome.Domain.Devices;

public interface IDeviceRepository
{
    IEnumerable<IDevice> FindAllDevices(DeviceFilter filter);
    IDevice? FindDeviceById(Guid deviceId);
    IDevice SaveDevice(IDevice device);
    void DeleteDevice(Guid deviceId);
    bool ThermostatInLocation(string location);

    // Added these to persist device history. 
    IEnumerable<CommandHistoryEntry> GetHistoryForDevice(Guid deviceId);
    void SaveHistoryEntry(CommandHistoryEntry entry);
}