namespace SmartHome.Infrastructure;

/// <summary>
/// Represents the complete persisted state of the application, including devices, command history, and location environments.
/// </summary>
public record SmartHomeDataSnapshot
{
    public List<DeviceSnapshot> Devices { get; init; } = new();
    public List<CommandHistorySnapshot> CommandHistory { get; init; } = new();
    public List<LocationSnapshot> Locations { get; init; } = new();
}
