public class SqliteRepository : IDeviceRepository, ILocationRepository
{
    private readonly SmartHomeDbContext _dbContext;
    private readonly IDeviceFactory _deviceFactory;

    public SqliteRepository(SmartHomeDbContext dbContext, IDeviceFactory deviceFactory)
    {
        _dbContext = dbContext;
        _deviceFactory = deviceFactory;
    }

    // Device repository methods
    public IEnumerable<IDevice> FindAllDevices(DeviceFilter filter)
    {
        var query = _dbContext.Devices.AsQueryable();

        if (filter.Types != null)
        {
            query = query.Where(d => d.Types == filter.Types);
        }

        if (!string.IsNullOrWhiteSpace(filter.Location))
        {
            query = query.Where(d => d.Location == filter.Location);
        }

        if (filter.IsOn.HasValue)
        {
            query = query.Where(d => d.IsOn == filter.IsOn.Value);
        }

        return query.AsEnumerable.Select(e => _deviceFactory.RehydrateDevice(MapToRehydrationData(e)));
    }

    public void AddDevice(Device device)
    {
        var entity = new DeviceEntity
        {
            Name = device.Name,
            Location = device.Location,
            Type = device.Type
        };

        _dbContext.Devices.Add(entity);
        _dbContext.SaveChanges();
        device.Id = entity.Id; // Update the domain model with the generated ID
    }

    public void RemoveDevice(Guid deviceId)
    {
        var entity = _dbContext.Devices.Find(deviceId);
        if (entity != null)
        {
            _dbContext.Devices.Remove(entity);
            _dbContext.SaveChanges();
        }
    }

// we'll make use of the current JsonRepository's mapping logic to convert between 
// our domain models and the database entities, which will help keep our repository focused on data access concerns

// this method saves a device to the database
// it checks if the device already exists (by ID) and updates it, otherwise it adds a new record
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

    // Location repository methods
    public IEnumerable<Location> GetAllLocations()
    {
        return _dbContext.Locations.ToList().Select(e => new Location
        {
            Name = e.Location,
            AmbientTemperature = e.AmbientTemperature
        });
    }

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

    public void RemoveLocation(string locationName)
    {
        var entity = _dbContext.Locations.Find(locationName);
        if (entity != null)
        {
            _dbContext.Locations.Remove(entity);
            _dbContext.SaveChanges();
        }
    }

    public bool IsThermostatPresentInLocation(string locationName)
    {
        return _dbContext.Devices.Any(d => d.Location == locationName && d.Types == DeviceType.Thermostat);
    }

    public IEnumerable<CommandHistoryEntry> GetDeviceCommandHistory(Guid deviceId)
    {
        return _dbContext.CommandHistories
            .Where(ch => ch.DeviceId == deviceId)
            .AsEnumerable()
            .OrderByDescending(ch => ch.Timestamp)
            .Select(ch => CommandHistoryEntry.Rehydrate(ch.Id, ch.deviceId, ch.CommandExecuted, ch.Timestamp));
    }

    public void SaveCommandHistoryEntry(CommandHistoryEntry entry)
    {
        _dbContext.CommandHistory.Add(new CommandHistoryEntity
        {
            Id = entry.Id,
            DeviceId = entry.DeviceId,
            CommandExecuted = entry.CommandExecuted,
            Timestamp = entry.Timestamp
        });
        _dbContext.SaveChanges();
    }

    public int? GetAmbientTemperature(string locationName)
    {
        return _dbContext.Locations.Find(locationName)?.AmbientTemperature;
    }

    public void UpdateAmbientTemperature(string locationName, int newTemperature)
    {
        var entity = _dbContext.Locations.Find(locationName);
        if (entity != null)
        {
            entity.AmbientTemperature = newTemperature;
            _dbContext.Locations.Update(entity);
        }
        else
        {
            _dbContext.Locations.Add(new LocationEntity
            {
                Location = locationName,
                AmbientTemperature = newTemperature
            });
        }
        _dbContext.SaveChanges();
    }

    // Helper methods for mapping between entities and domain models
    private static DeviceRehydrationData MapToRehydrationData(DeviceEntity entity) => new
    {
        Id = entity.Id,
        Name = entity.Name,
        Location = entity.Location,
        Type = entity.Type,
        IsOn = entity.IsOn,
        DeviceState = entity.DeviceState,
        ThermostatMode = entity.ThermostatMode,
        FanSpeed = entity.FanSpeed,
        LockState = entity.LockState,
        LightBrightness = entity.LightBrightness,
        LightColor = entity.LightColor,
        TargetTemp = entity.TargetTemp
    };

    private static DeviceEntity MapSnapshotToNewEntity(DeviceSnapshot snapshot) => new()
    {
        Id = snapshot.Id,
        Name = snapshot.Name,
        Location = snapshot.Location,
        Type = snapshot.Type,
        IsOn = snapshot.IsOn,
        DeviceState = snapshot.DeviceState,
        ThermostatMode = snapshot.ThermostatMode,
        FanSpeed = snapshot.FanSpeed,
        LockState = snapshot.LockState,
        LightBrightness = snapshot.LightBrightness,
        LightColor = snapshot.LightColor,
        TargetTemp = snapshot.TargetTemp
    };

    private static void MapSnapshotToEntity(DeviceSnapshot snapshot, DeviceEntity entity)
    {
        entity.Name = snapshot.Name;
        entity.Location = snapshot.Location;
        entity.Type = snapshot.Type;
        entity.IsOn = snapshot.IsOn;
        entity.DeviceState = snapshot.DeviceState;
        entity.ThermostatMode = snapshot.ThermostatMode;
        entity.FanSpeed = snapshot.FanSpeed;
        entity.LockState = snapshot.LockState;
        entity.LightBrightness = snapshot.LightBrightness;
        entity.LightColor = snapshot.LightColor;
        entity.TargetTemp = snapshot.TargetTemp;
    }
}