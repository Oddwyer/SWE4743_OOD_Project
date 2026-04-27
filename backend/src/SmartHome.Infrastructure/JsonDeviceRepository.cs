using SmartHome.Domain.Devices;
using SmartHome.Domain;
using System.Text.Json;
using SmartHome.Domain.Factories;

namespace SmartHome.Infrastructure;

/// <summary>
/// Initial concrete repository using Json --> to be switch to SQLite w/ ORM implementation.
/// </summary>
public class JsonDeviceRepository : IDeviceRepository
{
    private readonly List<IDevice> _devices = new();
    private readonly string _filePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../data/devices.json"));
    private readonly IDeviceFactory _deviceFactory;

    public JsonDeviceRepository(IDeviceFactory deviceFactory)
    {
        _deviceFactory = deviceFactory;
        LoadDevicesFromFile();
    }

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
            devices = devices.Where(d => d.IsDeviceOn == filter.IsOn.Value);
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
        SaveDevicesToFile();
        return device;
    }

    // Checks if device exists. If so, remove it from repository.
    public void DeleteDevice(Guid deviceId)
    {
        var existing = _devices.FirstOrDefault(d => d.Id == deviceId);
        if (existing != null)
        {
            _devices.Remove(existing);
            SaveDevicesToFile();
        }

    }

    // Confirms if thermostat already exists in given location (enforcing constraint of one per location).
    public bool ThermostatInLocation(string location)
    {
        return _devices.Any(d => d.Type == DeviceType.Thermostat && d.DeviceLocation == location);
    }

    // Loads repository from file.
    private void LoadDevicesFromFile()
    {

        // Check if file path exists and if so, read JSON file, deserialize into devices, and rehydrate to local repository.
        if (!File.Exists(_filePath))
        {
            return;
        }
        var json = File.ReadAllText(_filePath);
        var snapshots = JsonSerializer.Deserialize<List<DeviceSnapshot>>(json);

        if (snapshots == null)
        {
            return;
        }

        foreach (var snapshot in snapshots)
        {
            var device = _deviceFactory.RehydrateDevice(snapshot);
            _devices.Add(device);
        }

    }

    // Serialize devices for persistence.
    private void SaveDevicesToFile()
    {
        var snapshots = _devices.Select(d => new DeviceSnapshot
        {
            Id = d.Id,
            Name = d.DeviceName,
            Location = d.DeviceLocation,
            Type = d.Type,
            IsOn = d.IsDeviceOn
        }).ToList();

        var json = JsonSerializer.Serialize(snapshots);

        File.WriteAllText(_filePath, json);
    }
}