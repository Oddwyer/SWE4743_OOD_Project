using Microsoft.AspNetCore.Mvc;

namespace SmartHome.Api.Simulations;

[ApiController]
[Route("api/[controller]")]
public class SimulationController : ControllerBase
{
    [HttpPost("ambient-temperature")]
    public IActionResult SetAmbientTemperature()
    {
        return Ok("Stub");
    }
}


