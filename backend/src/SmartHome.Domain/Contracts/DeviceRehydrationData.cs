using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices;


namespace SmartHome.Domain.Contracts;

public class DeviceRehydrationData
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public DeviceType Type { get; init; }
    public bool IsOn { get; init; }
    public string? DeviceState { get; init; }
    public ThermostatMode? ThermostatMode { get; init; }
    public int? TargetTemperature { get; init; }
    public string? LightColor { get; init; }
    public int? LightBrightness { get; init; }
    public FanSpeed? FanSpeed { get; init; }
}
