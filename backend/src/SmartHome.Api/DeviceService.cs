using SmartHome.Domain.Interfaces;
using SmartHome.Domain;

/// TODO: Remove when built. Stub for GetDevices();

namespace SmartHome.Api;

public class DeviceService : IDeviceService
{
    private readonly List<IDevice> _devices;
    //private readonly List<CommandHistoryEntry> _commandHistory = new();

    public DeviceService(){
        _devices = new List<IDevice>
        {
            //TODO: Add seed data...
            //new LightDevice, 
            //new DoorLocks,
        };
    }


    public IReadOnlyList<IDevice> GetAllDevices()
    {
        return _devices;
    }

    public IDevice? GetDeviceById(Guid deviceId)
    {
        return _devices.FirstOrDefault(d => d.Id == deviceId);
    }

    public void RegisterDevice(IDevice device)
    {
        _devices.Add(device);
    }

    // TODO: Implement once we know constructor params required by DeviceCommand
    //public void ApplyDeviceCommand(Guid deviceId, DeviceCommand command){}

    public void RemoveDevice(Guid deviceId)
    {
        var device = GetDeviceById(deviceId);

        if(device == null)
        {
            return;
        }
            _devices.Remove(device);
    }

    // TODO: Uncomment once CommandHistoryEntry + CommandHistory repository are created.
    /*public IEnumerable<CommandHistoryEntry> GetCommandHistory (Guid deviceId){
        return _commandHistory.Where(entry => entry.DeviceId == deviceId);
    }*/
}

