using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;

namespace SmartHome.Domain.Devices;

public class DeviceRehydrationData
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Location { get; init; } = "";
    public DeviceType Type { get; init; }
    public bool IsOn { get; init; }
    public string? DeviceState { get; init; }
    public ThermostatMode? ThermostatMode { get; init; }
    public int? TargetTemperature { get; init; }
    public LightColor? LightColor { get; init; }
    public int? LightBrightness { get; init; }
    public FanSpeed? FanSpeed { get; init; }
}