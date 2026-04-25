using SmartHome.Domain.Interfaces;
using SmartHome.Domain;

/// TODO: Remove when built. Stub for GetDevices();

namespace SmartHome.Api;

public class DeviceService : IDeviceService
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
    public IReadOnlyList<IDevice> GetAllDevices()
    {
        return _devices;
    }


    public IDevice? GetDeviceById(Guid deviceId)
    {
        return _devices.FirstOrDefault(d => d.Id == deviceId);
    }

    public void AddDevice(IDevice device)
    {
        _devices.Add(device);
    }

    public void RemoveDevice(Guid deviceId)
    {
        var device = GetDeviceById(deviceId);

        if(device == null)
        {
            return;
        }
            _devices.Remove(device);
    }
}

