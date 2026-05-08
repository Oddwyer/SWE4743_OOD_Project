using Microsoft.AspNetCore.Mvc;

namespace SmartHome.Api.SSE;

/// <summary>
/// Exposes the Server-Sent Events endpoint used by the frontend dashboard.
/// </summary>

/// <summary>
/// Clients connect to this endpoint once and keep the connection open.
/// When device state changes occur, the server pushes events through this stream
/// so other open dashboards can refresh without manual page reloads.
/// </summary>
[ApiController]
[Route("api/devices/events")]
public class SSEController : ControllerBase
{
    private readonly IEventBroadcaster _broadcaster;

    public SSEController(IEventBroadcaster broadcaster)
    {
        _broadcaster = broadcaster;
    }

    /// <summary>
    /// Opens a long-lived SSE connection for the current client.
    /// </summary>
    [HttpGet]
    public async Task GetEvents()
    {
        await _broadcaster.StreamEventsAsync(Response, HttpContext.RequestAborted);
    }
}