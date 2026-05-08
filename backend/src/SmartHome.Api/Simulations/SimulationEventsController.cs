using Microsoft.AspNetCore.Mvc;

namespace SmartHome.Api.Simulations;

[ApiController]
[Route("api/simulation/events")]
public class SimulationEventsController : ControllerBase
{
    [HttpGet]
    public async Task GetEvents()
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        while (!HttpContext.RequestAborted.IsCancellationRequested)
        {
            await Response.WriteAsync("event: simulation-update\n");
            await Response.WriteAsync("data: tick\n\n");

            await Response.Body.FlushAsync();

            await Task.Delay(2000);
        }
    }
}