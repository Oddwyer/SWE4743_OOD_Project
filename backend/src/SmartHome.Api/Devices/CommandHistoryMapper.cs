using SmartHome.Domain.Commands;

namespace SmartHome.Api.Devices;

/// <summary>
/// Maps CommandHistoryEntry domain objects to CommandHistoryResponse DTOs for API responses.
/// </summary>

public static class CommandHistoryMapper
{
    /// <summary>
    /// Converts a CommandHistoryEntry into a CommandHistoryResponse.
    /// </summary>
    public static IEnumerable<CommandHistoryResponse> ToResponse(IEnumerable<CommandHistoryEntry> historyEntries)
    {
        return historyEntries.Select(entry => new CommandHistoryResponse
        {
            DeviceId = entry.DeviceId,
            CommandName = entry.Operation,
            Timestamp = entry.Timestamp
        });
    }
}