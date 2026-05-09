using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Infrastructure.ORM_Persistence;

/// <summary>
/// Seeds the SQLite database with an initial smart home layout
/// when the database is empty.
/// </summary>
public static class SmartHomeSeedData
{
    /// <summary>
    /// Inserts default locations and representative devices
    /// for development/demo purposes.
    /// </summary>
    public static void Seed(SmartHomeDbContext dbContext)
    {
        // Prevent duplicate seeding.
        if (dbContext.Devices.Any())
        {
            return;
        }

        // =========================
        // LOCATIONS
        // =========================

        var locations = new List<LocationEntity>
        {
            new() { Location = "Living Room", AmbientTemperature = 68 },
            new() { Location = "Master Bedroom", AmbientTemperature = 68 },
            new() { Location = "Entryway", AmbientTemperature = 68 }
        };

        dbContext.Locations.AddRange(locations);

        // =========================
        // DEVICES
        // =========================

        var devices = new List<DeviceEntity>
        {
            // =========================
            // LIVING ROOM
            // =========================

            new()
            {
                Id = Guid.NewGuid(),
                Name = "Living Room Thermostat",
                Location = "Living Room",
                Type = DeviceType.Thermostat,
                IsOn = false,
                DeviceState = "Off",
                ThermostatMode = ThermostatMode.Auto,
                TargetTemperature = 72
            },

            new()
            {
                Id = Guid.NewGuid(),
                Name = "Ceiling Fan",
                Location = "Living Room",
                Type = DeviceType.Fan,
                IsOn = false,
                FanSpeed = (int)FanSpeed.Medium
            },

            new()
            {
                Id = Guid.NewGuid(),
                Name = "Living Room Floor Lamp",
                Location = "Living Room",
                Type = DeviceType.Light,
                IsOn = false,
                LightColor = "#FFFFFF",
                LightBrightness = 100
            },

            new()
            {
                Id = Guid.NewGuid(),
                Name = "Back Door Lock",
                Location = "Living Room",
                Type = DeviceType.DoorLock,
                IsOn = true,
                DeviceState = "Locked"
            },

            // =========================
            // MASTER BEDROOM
            // =========================

            new()
            {
                Id = Guid.NewGuid(),
                Name = "Bedroom Ceiling Fan",
                Location = "Master Bedroom",
                Type = DeviceType.Fan,
                IsOn = false,
                FanSpeed = (int)FanSpeed.Medium
            },

            new()
            {
                Id = Guid.NewGuid(),
                Name = "Left Sconce",
                Location = "Master Bedroom",
                Type = DeviceType.Light,
                IsOn = false,
                LightColor = "#FFFFFF",
                LightBrightness = 100
            },

            new()
            {
                Id = Guid.NewGuid(),
                Name = "Right Sconce",
                Location = "Master Bedroom",
                Type = DeviceType.Light,
                IsOn = false,
                LightColor = "#FFFFFF",
                LightBrightness = 100
            },

            // =========================
            // ENTRYWAY
            // =========================

            new()
            {
                Id = Guid.NewGuid(),
                Name = "Overhead Light",
                Location = "Entryway",
                Type = DeviceType.Light,
                IsOn = false,
                LightColor = "#FFFFFF",
                LightBrightness = 100
            },

            new()
            {
                Id = Guid.NewGuid(),
                Name = "Front Door Lock",
                Location = "Entryway",
                Type = DeviceType.DoorLock,
                IsOn = true,
                DeviceState = "Locked"
            }
        };

        dbContext.Devices.AddRange(devices);

        // =========================
        // SAVE
        // =========================

        dbContext.SaveChanges();
    }
}