using SmartHome.Domain.Devices;
using SmartHome.Domain.Commands;
using System.Text.Json;

namespace SmartHome.Infrastructure;

/// <summary>
/// Initial concrete repository using Json --> to be switch to SQLite w/ ORM implementation.
/// </summary>
public class JsonDeviceRepository : IDeviceRepository, ILocationRepository
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
    /// Returns all devices that match the provided filter criteria (type, location, and/or on/off state).
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
    /// Finds and returns a single device by its ID, or null if not found.
    /// </summary>
    public IDevice? FindDeviceById(Guid deviceId)
    {
        return _devices.FirstOrDefault(d => d.Id == deviceId);
    }

    /// <summary>
    /// Adds a new device or updates an existing one in memory, then persists all data to the JSON file.
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
    /// Removes a device from the repository if it exists and persists the change to the JSON file.
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
    /// Checks whether a thermostat already exists in a given location (used to enforce one-thermostat-per-location rule).
    /// </summary>
    public bool ThermostatInLocation(string location)
    {
        return _devices.Any(d => d.Type == DeviceType.Thermostat && d.DeviceLocation == location);
    }

    /// <summary>
    /// Returns all command history entries associated with a specific device.
    /// </summary>
    public IEnumerable<CommandHistoryEntry> GetHistoryForDevice(Guid deviceId)
    {

        var deviceHistory = _commandHistory.Where(d => d.DeviceId == deviceId);
        return deviceHistory.AsEnumerable();

    }

    /// <summary>
    /// Adds a new command history entry and persists the updated data to the JSON file.
    /// </summary>
    public void SaveHistoryEntry(CommandHistoryEntry entry)
    {
        _commandHistory.Add(entry);
        SaveToFile();
    }

    /// <summary>
    /// Reads the JSON file (if it exists), deserializes it into snapshots, and loads all data into memory.
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
    /// Rehydrates device snapshots into real domain device objects using the factory and stores them in memory.
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
    /// Loads stored location temperature data into the in-memory dictionary.
    /// </summary>
    private void LoadLocations(SmartHomeDataSnapshot data)
    {

        foreach (var locationSnapshot in data.Locations)
        {
            _locations[locationSnapshot.Location] = locationSnapshot.AmbientTemperature;
        }
    }

    /// <summary>
    /// Rehydrates command history snapshots into domain history entries and stores them in memory.
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
    /// Dehydrates all in-memory data (devices, history, locations), bundles it into a root snapshot, 
    /// serializes it to JSON, and writes it to file.
    /// </summary>
    private void SaveToFile()
    {
        var data = new SmartHomeDataSnapshot
        {
            Devices = DehydrateDevices(),
            CommandHistory = DehydrateHistory(),
            Locations = DehydrateLocations()
        };


        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_filePath, json);
    }

    /// <summary>
    /// Converts all domain device objects into data-only snapshots for persistence.
    /// </summary>
    private List<DeviceSnapshot> DehydrateDevices()
    {
        return _devices.Select(d => new DeviceSnapshot
        {
            Id = d.Id,
            Name = d.DeviceName,
            Location = d.DeviceLocation,
            Type = d.Type,
            IsOn = d.IsDeviceOn,
            DeviceState = null
        }).ToList();
    }

    /// <summary>
    /// Converts command history entries into snapshot objects for persistence.
    /// </summary>
    private List<CommandHistorySnapshot> DehydrateHistory()
    {
        return _commandHistory.Select(h => new CommandHistorySnapshot
        {
            Id = h.Id,
            DeviceId = h.DeviceId,
            Operation = h.Operation,
            Timestamp = h.Timestamp
        }).ToList();
    }

    /// <summary>
    /// Converts in-memory location temperature data into snapshot objects for persistence.
    /// </summary>
    private List<LocationSnapshot> DehydrateLocations()
    {
        return _locations.Select(l => new LocationSnapshot
        {
            Location = l.Key,
            AmbientTemperature = l.Value
        }).ToList();
    }

    /// <summary>
    /// Retrieves the stored ambient temperature for a given location, or returns null if no temperature has been recorded.
    /// </summary>
    public int? GetAmbientTemperature(string location)
    {
        return _locations.TryGetValue(location, out var temp) ? temp : null;
    }

    /// <summary>
    /// Adds or updates the ambient temperature for a given location and persists the updated state to the JSON file.
    /// </summary>
    public void SaveAmbientTemperature(string location, int temperature)
    {
        _locations[location] = temperature;
        SaveToFile();
    }
}