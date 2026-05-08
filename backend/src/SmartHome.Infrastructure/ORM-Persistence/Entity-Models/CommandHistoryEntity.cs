public class CommandHistoryEntity
{
    public Guid Id { get; set; } // PK for our table
    public Guid DeviceId { get; set; } // FK to DeviceEntity
    public string CommandExecuted { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}