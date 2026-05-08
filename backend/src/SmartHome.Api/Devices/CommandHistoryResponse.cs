namespace SmartHome.Api.Devices;

/// <summary>
/// DTO used to represent a command history entry returned by the API.
/// </summary>
public class CommandHistoryResponse
{
    /// <summary>Identifier of the device this history entry belongs to.</summary>
    public Guid DeviceId { get; set; }
    /// <summary>Human-readable description of the command that was executed.</summary>
    public string CommandName { get; set; } = string.Empty;
    /// <summary>UTC timestamp of when the command was executed.</summary>
    public DateTime Timestamp { get; set; }
}
