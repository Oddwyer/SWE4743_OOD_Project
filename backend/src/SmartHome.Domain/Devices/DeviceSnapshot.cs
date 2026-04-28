namespace SmartHome.Domain.Devices;

public record DeviceSnapshot
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public DeviceType Type { get; init; }
    public string? Location { get; init; }
    public bool IsOn { get; init; }
    public string? DeviceState { get; init; }
    // this will be a string representation of the device's current state, 
    // it can be used to store additional information about the device's state that is 
    // not captured by the IsOn property, for example for a light device it can store the current 
    // color and brightness, for a thermostat it can store the current temperature setting, etc.

    // Additional properties can be added as needed

    // TODO - Kataali: Confirm which device-specific state fields must be added
    // for persistence, especially thermostat mode, desired temperature, and state.
}