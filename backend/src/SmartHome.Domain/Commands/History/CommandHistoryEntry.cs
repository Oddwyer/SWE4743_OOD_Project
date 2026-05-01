namespace SmartHome.Domain.Commands.History;

/// <summary>
/// Stores a single entry in the command history for a single device.
/// </summary>
public class CommandHistoryEntry
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public string Operation { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Constructor for new history entries.
    /// </summary>
    public CommandHistoryEntry(Guid deviceId, IDeviceCommand command)
    {
        Id = Guid.NewGuid();
        DeviceId = deviceId;
        Operation = command.CommandDescription;
        Timestamp = DateTime.UtcNow;
    }


    /// <summary>
    /// Private constructor to rehydrate existing history entries.
    /// </summary>
    private CommandHistoryEntry(Guid id, Guid deviceId, string operation, DateTime timestamp)
    {
        Id = id;
        DeviceId = deviceId;
        Operation = operation;
        Timestamp = timestamp;
    }

    /// <summary>
    /// Forward facing method to rehydrate existing history entries.
    /// </summary>
    public static CommandHistoryEntry Rehydrate(Guid id, Guid deviceId, string operation, DateTime timestamp)
    {
        return new CommandHistoryEntry(id, deviceId, operation, timestamp);
    }
}