using SmartHome.Domain.Commands.History;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Locations;
using SmartHome.Infrastructure;
using SmartHome.Infrastructure.ORM_Persistence;

/// <summary>
/// SQLite-backed implementation of IDeviceRepository and ILocationRepository using EF Core.
/// </summary>
public class SqliteRepository : IDeviceRepository, ILocationRepository
{
    private readonly SmartHomeDbContext _dbContext;
    private readonly IDeviceFactory _deviceFactory;

    /// <summary>Initializes the repository with a shared EF Core context and device factory.</summary>
    public SqliteRepository(SmartHomeDbContext dbContext, IDeviceFactory deviceFactory)
    {
        _dbContext = dbContext;
        _deviceFactory = deviceFactory;
    }

    /// <summary>Returns all devices matching the provided filter criteria.</summary>
    public IEnumerable<IDevice> FindAllDevices(DeviceFilter filter)
    {
        var query = _dbContext.Devices.AsQueryable();

        if (filter.Type != null)
        {
            query = query.Where(d => d.Type == filter.Type);
        }

        if (!string.IsNullOrWhiteSpace(filter.Location))
        {
            query = query.Where(d => d.Location == filter.Location);
        }

        if (filter.IsOn.HasValue)
        {
            query = query.Where(d => d.IsOn == filter.IsOn.Value);
        }

        return query.AsEnumerable().Select(e => _deviceFactory.RehydrateDevice(MapToRehydrationData(e)));
    }

    /// <summary>Returns the device with the given ID, or null if not found.</summary>
    public IDevice? FindDeviceById(Guid deviceId)
    {
        var entity = _dbContext.Devices.Find(deviceId);
        return entity == null 
        ? null 
        : _deviceFactory.RehydrateDevice(MapToRehydrationData(entity));
    }

    public void AddDevice(Device device)
    {
        var entity = new DeviceEntity
        {
            Name = device.DeviceName,
            Location = device.DeviceLocation,
            Type = device.Type
        };

        _dbContext.Devices.Add(entity);
        _dbContext.SaveChanges();
    }

    /// <summary>Removes the device with the given ID from the database.</summary>
    public void DeleteDevice(Guid deviceId)
    {
        var entity = _dbContext.Devices.Find(deviceId);
        if (entity != null)
        {
            _dbContext.Devices.Remove(entity);
            _dbContext.SaveChanges();
        }
    }

    /// <summary>Inserts a new device or updates the existing record; returns the saved device.</summary>
    public IDevice SaveDevice(IDevice device)
    {
        var snapshot = JsonRepository.ToDeviceSnapshot(device);
        var existingDevice = _dbContext.Devices.Find(device.Id);

        if (existingDevice != null)
        {
            MapSnapshotToEntity(snapshot, existingDevice);
            _dbContext.Devices.Update(existingDevice);
        }
        else
        {
            _dbContext.Devices.Add(MapSnapshotToNewEntity(snapshot));
        }
        _dbContext.SaveChanges();
        return device;
    }

    /// <summary>Returns all persisted locations with their ambient temperatures.</summary>
    public IEnumerable<Location> GetAllLocations()
    {
        return _dbContext.Locations.ToList().Select(e => new Location
        {
            Name = e.Location,
            AmbientTemperature = e.AmbientTemperature
        });
    }

    /// <summary>Inserts a new location record.</summary>
    public void AddLocation(Location location)
    {
        var entity = new LocationEntity
        {
            Location = location.Name,
            AmbientTemperature = location.AmbientTemperature
        };
        _dbContext.Locations.Add(entity);
        _dbContext.SaveChanges();
    }

    /// <summary>Removes a location record by name.</summary>
    public void RemoveLocation(string locationName)
    {
        var entity = _dbContext.Locations.Find(locationName);
        if (entity != null)
        {
            _dbContext.Locations.Remove(entity);
            _dbContext.SaveChanges();
        }
    }

    /// <summary>Returns true when a thermostat device already exists in the specified location.</summary>
    public bool ThermostatInLocation(string location)
    {
        return _dbContext.Devices.Any(d => d.Location == location && d.Type == DeviceType.Thermostat);
    }

    /// <summary>Returns command history entries for the specified device, newest first.</summary>
    public IEnumerable<CommandHistoryEntry> GetHistoryForDevice(Guid deviceId)
    {
        return _dbContext.CommandHistories
            .Where(ch => ch.DeviceId == deviceId)
            .AsEnumerable()
            .OrderByDescending(ch => ch.Timestamp)
            .Select(ch => CommandHistoryEntry.Rehydrate(ch.Id, ch.DeviceId, ch.CommandExecuted, ch.Timestamp));
    }

    /// <summary>Persists a command history entry to the database.</summary>
    public void SaveHistoryEntry(CommandHistoryEntry entry)
    {
        _dbContext.CommandHistories.Add(new CommandHistoryEntity
        {
            Id = entry.Id,
            DeviceId = entry.DeviceId,
            CommandExecuted = entry.Operation,
            Timestamp = entry.Timestamp
        });
        _dbContext.SaveChanges();
    }

    /// <summary>Returns the stored ambient temperature (°F) for the given location, or null if not found.</summary>
    public int? GetAmbientTemperature(string locationName)
    {
        return _dbContext.Locations.Find(locationName)?.AmbientTemperature;
    }

    /// <summary>Inserts or updates the ambient temperature record for a location.</summary>
    public void SaveAmbientTemperature(string location, int temperature)
    {
        var entity = _dbContext.Locations.Find(location);
        if (entity != null)
        {
            entity.AmbientTemperature = temperature;
            _dbContext.Locations.Update(entity);
        }
        else
        {
            _dbContext.Locations.Add(new LocationEntity
            {
                Location = location,
                AmbientTemperature = temperature
            });
        }
        _dbContext.SaveChanges();
    }

    /// <summary>Maps a DeviceEntity to the rehydration data contract used by the device factory.</summary>
    private static DeviceRehydrationData MapToRehydrationData(DeviceEntity entity) => new DeviceRehydrationData
    {
        Id = entity.Id,
        Name = entity.Name,
        Location = entity.Location,
        Type = entity.Type,
        IsOn = entity.IsOn,
        DeviceState = entity.DeviceState,
        ThermostatMode = entity.ThermostatMode,
        TargetTemperature = entity.TargetTemperature,
        LightColor = entity.LightColor,
        LightBrightness = entity.LightBrightness,
        FanSpeed = (FanSpeed?)entity.FanSpeed
    };

    /// <summary>Creates a new DeviceEntity from a device snapshot.</summary>
    private static DeviceEntity MapSnapshotToNewEntity(DeviceSnapshot snapshot) => new()
    {
        Id = snapshot.Id,
        Name = snapshot.Name,
        Location = snapshot.Location,
        Type = snapshot.Type,
        IsOn = snapshot.IsOn,
        DeviceState = snapshot.DeviceState,
        ThermostatMode = snapshot.ThermostatMode,
        FanSpeed = (int?)snapshot.FanSpeed,
        LightBrightness = snapshot.LightBrightness,
        LightColor = snapshot.LightColor,
        TargetTemperature = snapshot.TargetTemperature
    };

    /// <summary>Updates an existing DeviceEntity in place from a device snapshot.</summary>
    private static void MapSnapshotToEntity(DeviceSnapshot snapshot, DeviceEntity entity)
    {
        entity.Name = snapshot.Name;
        entity.Location = snapshot.Location;
        entity.Type = snapshot.Type;
        entity.IsOn = snapshot.IsOn;
        entity.DeviceState = snapshot.DeviceState;
        entity.ThermostatMode = snapshot.ThermostatMode;
        entity.FanSpeed = (int?)snapshot.FanSpeed;
        entity.LightBrightness = snapshot.LightBrightness;
        entity.LightColor = snapshot.LightColor;
        entity.TargetTemperature = snapshot.TargetTemperature;
    }
}