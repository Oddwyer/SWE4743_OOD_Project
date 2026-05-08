using SmartHome.Domain.Devices;
using SmartHome.Infrastructure.ORM_Persistence;

public static class SmartHomeSeedData
{
    public static void Seed(SmartHomeDbContext _dbContext)
    {
        // Check if there are already devices in the database to avoid seeding duplicate data
        if (_dbContext.Devices.Any())
        {
            return; // Database has been seeded, ie devices, locations, etc already exist, so we can skip seeding
        }

        // Seed initial locations with ambient temperatures
        var locations = new List<LocationEntity>
        {
            new LocationEntity { Location = "Living Room", AmbientTemperature = 22 },
            new LocationEntity { Location = "Bedroom", AmbientTemperature = 20 },
            new LocationEntity { Location = "Kitchen", AmbientTemperature = 24 }
        };
        _dbContext.Locations.AddRange(locations);

        // Seed initial devices
        var devices = new List<DeviceEntity>
        {
            new DeviceEntity { Name = "Living Room Thermostat", Location = "Living Room", Type = DeviceType.Thermostat, IsOn = false, TargetTemperature = 22 },
            new DeviceEntity { Name = "Bedroom Light", Location = "Bedroom", Type = DeviceType.Light, IsOn = false, LightBrightness = 0, LightColor = "White" },
            new DeviceEntity { Name = "Kitchen Fan", Location = "Kitchen", Type = DeviceType.Fan, IsOn = false, FanSpeed = 0 }
        };
        _dbContext.Devices.AddRange(devices);
        _dbContext.SaveChanges();
    }
}