using SmartHome.Domain.Devices;
using SmartHome.Domain.Commands;

namespace SmartHome.Domain.Commands;

public class CommandHistoryEntry
{
    // this class is meant to store a single entry in the command history for a single device, 
    // it will store the command that was executed, the timestamp of when it was 
    // executed, and the device it was executed on

    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }

    // public IDeviceCommand Command { get; set; }

    // TODO - Kataali: Thoughts of this property instead of IDeviceCommand (also see constructor)?
    public string Operation { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Constructor for new history entries.
    /// </summary>
    public CommandHistoryEntry(Guid deviceId, IDeviceCommand command)
    {
        Id = Guid.NewGuid();
        DeviceId = deviceId;
        //Command = command;
        Operation = command.CommandDescription;
        Timestamp = DateTime.UtcNow;
    }

    // TODO - Kataali: Thoughts on rehydration here?
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