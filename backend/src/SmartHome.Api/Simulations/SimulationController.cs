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

    // PUT: api/simulation/speed
    [HttpPut("speed")]
    public IActionResult SetSimulationSpeed()
    {
        _logger.LogInformation("Simulation speed update requested.");
        return Ok("Simulation speed set (stub)");
    }

    // PUT: api/simulation/reset
    [HttpPost("reset")]
    public IActionResult ResetSimulation()
    {
        _logger.LogInformation("Simulation reset requested.");
        return Ok("Simulation reset (stub)");
    }
}



