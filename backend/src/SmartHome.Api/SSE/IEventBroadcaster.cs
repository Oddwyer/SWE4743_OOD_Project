namespace SmartHome.Api.SSE;

/// <summary>
/// Defines the contract for broadcasting server-side events to connected UI clients.
/// </summary>
public interface IEventBroadcaster
{
    /// <summary>
    /// Opens and maintains an SSE stream for one connected client.
    /// </summary>
    Task BroadcastDeviceChangedAsync(Guid deviceId);

    /// <summary>
    /// Broadcasts a device change notification to all connected clients.
    /// </summary>
    Task StreamEventsAsync(
        HttpResponse response,
        CancellationToken cancellationToken);
}