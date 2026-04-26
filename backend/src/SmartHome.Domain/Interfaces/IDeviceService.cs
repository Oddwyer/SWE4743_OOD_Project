using SmartHome.Domain.Interfaces;


namespace SmartHome.Domain;

public interface IDeviceService
{
    // TODO: DeviceFilter
    // Method to retrieve all devices in the system
    //IEnumerable<IDevice> GetAllDevices(DeviceFilter filter);
    IReadOnlyList<IDevice> GetAllDevices();
    
    // TODO: CommandHistoryEntry
    // command list for persistenance, reference and rehydration section in project doc
    //IEnumerable<CommandHistoryEntry> GetCommandHistory (Guid deviceId); 
    // this will return a list of all commands that have been executed on a specific device, which can be used for auditing and debugging purposes

    // Method to retrieve a device by its ID
    IDevice? GetDeviceById(Guid deviceId);

    // Method to add a new device to the system
    void RegisterDevice(IDevice device);

    // IDevice RegisterDevice(IDevice device);
    // we can use this method to create a new device based on a request object
    // AO: I am converting from a DTO request object to an IDevice for you via API and controller. :-)

    // Method to remove a device from the system by its ID
    void RemoveDevice(Guid deviceId);

    //TODO: DeviceCommand
   //IDevice ApplyDeviceCommand(Guid deviceId, DeviceCommand command);
    // this method will take a device ID and a command, apply the command to the device

}