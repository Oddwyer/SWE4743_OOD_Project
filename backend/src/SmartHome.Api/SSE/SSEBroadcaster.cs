using System.Collections.Concurrent;
using System.Threading.Channels;

namespace SmartHome.Api.SSE;

/// <summary>
/// Tracks connected SSE clients and broadcasts device change events to them.
/// Each connected browser tab gets its own message channel.
/// When a device changes, a message is written to every connected client's channel.
/// </summary>
public class SSEBroadcaster : IEventBroadcaster
{
    private readonly ConcurrentDictionary<Guid, Channel<string>> _clients = new();

    /// <summary>
    /// Registers a client and keeps its SSE response stream open until disconnected.
    /// </summary>
    public async Task StreamEventsAsync(
        HttpResponse response,
        CancellationToken cancellationToken)
    {
        response.Headers.Append("Content-Type", "text/event-stream");
        response.Headers.Append("Cache-Control", "no-cache");
        response.Headers.Append("Connection", "keep-alive");

        var clientId = Guid.NewGuid();
        var channel = Channel.CreateUnbounded<string>();

        _clients.TryAdd(clientId, channel);

        try
        {
            await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
            {
                await response.WriteAsync("event: device-changed\n", cancellationToken);
                await response.WriteAsync($"data: {message}\n\n", cancellationToken);
                await response.Body.FlushAsync(cancellationToken);
            }
        }
        finally
        {
            _clients.TryRemove(clientId, out _);
        }
    }

    /// <summary>
    /// Sends a device-changed event to every connected SSE client.
    /// </summary>
    public async Task BroadcastDeviceChangedAsync(Guid deviceId)
    {
        var message = $$"""
{"deviceId":"{{deviceId}}","eventType":"device-changed"}
""";

        foreach (var client in _clients.Values)
        {
            await client.Writer.WriteAsync(message);
        }
    }
}