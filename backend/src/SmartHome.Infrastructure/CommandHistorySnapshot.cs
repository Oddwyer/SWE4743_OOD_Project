namespace SmartHome.Infrastructure;

/// <summary>
/// String representation of the device's current history.
/// </summary>
public record CommandHistorySnapshot
{
    public Guid Id { get; init; }
    public Guid DeviceId { get; init; }
    public DateTime Timestamp { get; init; }
    public string Operation { get; init; } = string.Empty;
}