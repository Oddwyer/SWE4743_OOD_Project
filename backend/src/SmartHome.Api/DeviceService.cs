using SmartHome.Domain.Interfaces;
using SmartHome.Domain;

/// TODO: Remove when built. Stub for GetDevices();

namespace SmartHome.Api;

public class DeviceService: IDeviceService
{
    private readonly List<IDevice> _devices;

    public DeviceService()
    {
        _devices = new List<IDevice>
        {
            //TODO: Add seed data...
            //new LightDevice, 
            //new DoorLocks,
        };
    }
    public IReadOnlyList<IDevice> GetDevices()
    {
        return _devices;
    }

    //TODO: Implement...
    //public IDevice GetDeviceById(string deviceID);

    //public void RegisterDevice(string name, string type, string location);

    //public void DeleteDevice(string deviceID);
}

