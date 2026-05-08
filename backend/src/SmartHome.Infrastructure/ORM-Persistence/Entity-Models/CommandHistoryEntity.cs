/// <summary>
/// EF Core entity representing a persisted command history entry.
/// Cascades delete when the associated device is removed.
/// </summary>
public class CommandHistoryEntity
{
    /// <summary>Primary key (UUID).</summary>
    public Guid Id { get; set; }
    /// <summary>Foreign key referencing the device this command was executed on.</summary>
    public Guid DeviceId { get; set; }
    /// <summary>Human-readable description of the command that was executed.</summary>
    public string CommandExecuted { get; set; } = string.Empty;
    /// <summary>UTC timestamp of when the command was executed.</summary>
    public DateTime Timestamp { get; set; }
}
