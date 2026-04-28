using SmartHome.Domain.Devices;
using SmartHome.Domain.Commands;
using System.Text.Json;

namespace SmartHome.Infrastructure;

/// <summary>
/// Initial concrete repository using Json --> to be switch to SQLite w/ ORM implementation.
/// </summary>
public class JsonDeviceRepository : IDeviceRepository
{
    private readonly List<IDevice> _devices = new();
    private readonly Dictionary<string, int> _locations = new();
    private readonly List<CommandHistoryEntry> _commandHistory = new();
    private readonly string _filePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../data/devices.json"));
    private readonly IDeviceFactory _deviceFactory;

    public JsonDeviceRepository(IDeviceFactory deviceFactory)
    {
        _deviceFactory = deviceFactory;
        LoadFromFile();
    }

    /// <summary>
    /// Returns any devices filtered by all or any location, type, or whether it is on.
    /// </summary>
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

    /// <summary>
    /// Returns any devices matching given ID.
    /// </summary>
    public IDevice? FindDeviceById(Guid deviceId)
    {
        return _devices.FirstOrDefault(d => d.Id == deviceId);
    }

    /// <summary>
    /// Checks if device exists. If so, updates it. If not, adds it to the repository.
    /// </summary>
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
        SaveToFile();
        return device;
    }

    /// <summary>
    /// Checks if device exists. If so, remove it from repository.
    /// </summary>
    public void DeleteDevice(Guid deviceId)
    {
        var existing = _devices.FirstOrDefault(d => d.Id == deviceId);
        if (existing != null)
        {
            _devices.Remove(existing);
            SaveToFile();
        }

    }

    /// <summary>
    /// Confirms if thermostat already exists in given location (enforcing constraint of one per location).
    /// </summary>
    public bool ThermostatInLocation(string location)
    {
        return _devices.Any(d => d.Type == DeviceType.Thermostat && d.DeviceLocation == location);
    }

    /// <summary>
    /// Returns any history for a device with matching ID.
    /// </summary>
    public IEnumerable<CommandHistoryEntry> GetHistoryForDevice(Guid deviceId)
    {

        var deviceHistory = _commandHistory.Where(d => d.DeviceId == deviceId);
        return deviceHistory.AsEnumerable();

    }

    /// <summary>
    /// Adds history entry to repository.
    /// </summary>
    public void AddHistoryEntry(CommandHistoryEntry entry)
    {
        _commandHistory.Add(entry);
    }

    /// <summary>
    /// Loads data from file.
    /// </summary>
    private void LoadFromFile()
    {
        // Check if file path exists and if so, read JSON file, deserialize into devices, and rehydrate to local repository.
        if (!File.Exists(_filePath))
        {
            return;
        }

        var json = File.ReadAllText(_filePath);
        var data = JsonSerializer.Deserialize<SmartHomeDataSnapshot>(json);

        if (data == null)
        {
            return;
        }

        LoadDevices(data);
        LoadHistory(data);
        LoadLocations(data);
    }

    /// <summary>
    /// Loads devices from file.
    /// </summary>
    private void LoadDevices(SmartHomeDataSnapshot data)
    {

        foreach (var deviceSnapshot in data.Devices)
        {
            var device = _deviceFactory.RehydrateDevice(
                deviceSnapshot.Id,
                deviceSnapshot.Name ?? string.Empty,
                deviceSnapshot.Location ?? string.Empty,
                deviceSnapshot.Type,
                deviceSnapshot.IsOn,
                deviceSnapshot.DeviceState
            );
            _devices.Add(device);
        }

    }

    /// <summary>
    /// Loads locations from file.
    /// </summary>
    private void LoadLocations(SmartHomeDataSnapshot data)
    {

        foreach (var locationSnapshot in data.Locations)
        {
            _locations[locationSnapshot.Location] = locationSnapshot.AmbientTemperature;
        }
    }

    /// <summary>
    /// Loads command history from file.
    /// </summary>
    private void LoadHistory(SmartHomeDataSnapshot data)
    {
        foreach (var historySnapshot in data.CommandHistory)
        {
            _commandHistory.Add(CommandHistoryEntry.Rehydrate(
                historySnapshot.Id,
                historySnapshot.DeviceId,
                historySnapshot.Operation,
                historySnapshot.Timestamp
            ));
        }

    }

    /// <summary>
    /// Serialize devices for persistence.
    /// </summary>
    // TODO - Amber: Update to SaveToFile (include locations,history) Replace w/ Device.Dehydrate when implemented.
    private void SaveToFile()
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

    // TODO - Amber: Add GetAmbientTemp, SetAmbientTemp methods.
}