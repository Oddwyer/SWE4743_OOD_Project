namespace SmartHome.Domain.Commands;

/// <summary>
/// String representation of the device's current history.
/// </summary>
public record CommandSnapshot
{
    public Guid DeviceId { get; init; }
    public DateTime Timestamp { get; init; }
    public string Operation { get; init; } = string.Empty();
}