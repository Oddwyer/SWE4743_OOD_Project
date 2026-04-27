using SmartHome.Domain.Devices;
using SmartHome.Domain.Commands;
using SmartHome.Domain;

namespace SmartHome.Domain.Devices;

public interface IDeviceService
{

    // Method to retrieve all devices in the system
    IEnumerable<IDevice> GetAllDevices(DeviceFilter filter);

    // Command list for persistenance, reference and rehydration section in project doc
    IEnumerable<CommandHistoryEntry> GetCommandHistory(Guid deviceId);
    // this will return a list of all commands that have been executed on a specific device, which can be used for auditing and debugging purposes

    // Method to retrieve a device by its ID
    IDevice? GetDeviceById(Guid deviceId);

    // Method to add a new device to the system
    void RegisterDevice(IDevice device);

    // Method to remove a device from the system by its ID
    void RemoveDevice(Guid deviceId);

    // Method will take a device ID and a command, apply the command to the device
    IDevice ApplyDeviceCommand(Guid deviceId, IDeviceCommand command);


}