using SmartHome.Domain.Commands.History;

namespace SmartHome.Domain.Devices;

/// <summary>
/// Defines data-access operations for devices and their command history.
/// </summary>
public interface IDeviceRepository
{
    /// <summary>Returns all devices that match the provided filter criteria.</summary>
    IEnumerable<IDevice> FindAllDevices(DeviceFilter filter);

    /// <summary>Returns the device with the given ID, or null if not found.</summary>
    IDevice? FindDeviceById(Guid deviceId);

    /// <summary>Persists a device, inserting or updating as required.</summary>
    IDevice SaveDevice(IDevice device);

    /// <summary>Removes the device with the given ID from the store.</summary>
    void DeleteDevice(Guid deviceId);

    /// <summary>Returns true when a thermostat already exists in the specified location.</summary>
    bool ThermostatInLocation(string location);

    /// <summary>Returns command history entries for the specified device, newest first.</summary>
    IEnumerable<CommandHistoryEntry> GetHistoryForDevice(Guid deviceId);

    /// <summary>Persists a command history entry.</summary>
    void SaveHistoryEntry(CommandHistoryEntry entry);
}
