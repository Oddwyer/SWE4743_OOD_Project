using SmartHome.Domain.Devices;
using SmartHome.Infrastructure.ORM_Persistence;

/// <summary>
/// Seeds the database with initial locations and devices when the database is empty.
/// </summary>
public static class SmartHomeSeedData
{
    /// <summary>
    /// Inserts the default locations (Living Room, Bedroom, Kitchen) and one representative device per
    /// location if no devices exist yet.
    /// </summary>
    public static void Seed(SmartHomeDbContext _dbContext)
    {
        if (_dbContext.Devices.Any())
        {
            return;
        }

        var locations = new List<LocationEntity>
        {
            new LocationEntity { Location = "Living Room", AmbientTemperature = 65 },
            new LocationEntity { Location = "Bedroom", AmbientTemperature = 65 },
            new LocationEntity { Location = "Kitchen", AmbientTemperature = 65 }
        };
        _dbContext.Locations.AddRange(locations);

        var devices = new List<DeviceEntity>
        {
            new DeviceEntity { Name = "Living Room Thermostat", Location = "Living Room", Type = DeviceType.Thermostat, IsOn = false, TargetTemperature = 70 },
            new DeviceEntity { Name = "Bedroom Light", Location = "Bedroom", Type = DeviceType.Light, IsOn = false, LightBrightness = 0, LightColor = "White" },
            new DeviceEntity { Name = "Kitchen Fan", Location = "Kitchen", Type = DeviceType.Fan, IsOn = false, FanSpeed = 0 }
        };
        _dbContext.Devices.AddRange(devices);
        _dbContext.SaveChanges();
    }
}
