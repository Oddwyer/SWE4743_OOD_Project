using Microsoft.AspNetCore.Mvc;

namespace SmartHome.Api.Simulations;

[ApiController]
[Route("api/[controller]")]
public class SimulationController : ControllerBase
{

    private readonly ILogger<SimulationController> _logger;

    public SimulationController(ILogger<SimulationController> logger)
    {
        _logger = logger;
    }

    [HttpPost("ambient-temperature")]
    public IActionResult SetAmbientTemperature()
    {
        _logger.LogInformation("Ambient temperature update requested.");
        return Ok("Stub");
    }

    [HttpPost("reset")]
    public IActionResult ResetSimulation()
    {
        _logger.LogInformation("Simulation reset requested.");
        return Ok("Simulation reset (stub)");
    }
}



