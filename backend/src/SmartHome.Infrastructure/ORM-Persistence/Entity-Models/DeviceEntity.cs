using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Thermostat;

public class DeviceEntity
{
    // shared properties
    public Guid Id { get; set; } // our table's PK
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DeviceType Type { get; set; }
    public bool IsOn { get; set; }
    public string? DeviceState { get; set; }

    // fan specific properties
    public int? FanSpeed { get; set; }

    // door lock specific properties
    public bool? IsLocked { get; set; }

    // light specific properties
    public string? LightColor { get; set; }
    public int? LightBrightness { get; set; }

    // Thermostat-specific properties
    public ThermostatMode? ThermostatMode { get; set; }
    public int? TargetTemperature { get; set; }
}