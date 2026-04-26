using SmartHome.Domain.Devices;

namespace SmartHome.Domain;

public record DeviceSnapshot
{
    public Guid Id { get; init; }
    public string? Name;
    public DeviceType Type { get; init; }
    public string? Location;
    public bool IsOn { get; init; }
    public string? deviceState { get; init; }
    // this will be a string representation of the device's current state, 
    // it can be used to store additional information about the device's state that is 
    // not captured by the IsOn property, for example for a light device it can store the current 
    // color and brightness, for a thermostat it can store the current temperature setting, etc.

    // Additional properties can be added as needed
}