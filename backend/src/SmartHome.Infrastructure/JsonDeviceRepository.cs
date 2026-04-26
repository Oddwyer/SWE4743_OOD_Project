using SmartHome.Domain.Devices;
using SmartHome.Domain;
using System.Text.Json;

namespace SmartHome.Infrastructure;

/// <summary>
/// Initial concrete repository using Json. To be switch to SQLite w/ ORM implementation.
/// </summary>
public class JsonDeviceRepository : IDeviceRepository
{
    private readonly List<IDevice> _devices = new();
    private readonly string _filePath = "devices.json";

    // Returns any devices filtered by all or any location, type, or whether it is on.
    public IEnumerable<IDevice> FindAllDevices(DeviceFilter filter)
    {
        var devices = _devices.AsEnumerable();

        if (filter.Type != null)
        {
            devices = devices.Where(d => d.Type == filter.Type);
        }
        if (!string.IsNullOrWhiteSpace(filter.Location))
        {
            devices = devices.Where(d => d.DeviceLocation == filter.Location);
        }
        if (filter.IsOn.HasValue)
        {
            devices = devices.Where(d => d.IsDeviceOn == filter.IsOn);
        }

        return devices;
    }

    // Returns any devices matching given ID.
    public IDevice? FindDeviceById(Guid deviceId)
    {
        return _devices.FirstOrDefault(d => d.Id == deviceId);
    }

    // Checks if device exists. If so, updates it. If not, adds it to the repository.
    public IDevice SaveDevice(IDevice device)
    {
        var existing = _devices.FirstOrDefault(d => d.Id == device.Id);
        if (existing != null)
        {
            var index = _devices.IndexOf(existing);
            _devices[index] = device;
        }
        else
        {
            _devices.Add(device);
        }
        return device;
    }

    // Checks if device exists. If so, remove it from repository.
    public void DeleteDevice(Guid deviceId)
    {
        var existing = _devices.FirstOrDefault(d => d.Id == deviceId);
        if (existing != null)
        {
            _devices.Remove(existing);
        }

    }

    // Confirms if thermostat already exists in given location (enforcing constraint of one per location).
    public bool ThermostatInLocation(string location)
    {
        return _devices.Any(d => d.Type == DeviceType.Thermostat && d.DeviceLocation == location);
    }
}