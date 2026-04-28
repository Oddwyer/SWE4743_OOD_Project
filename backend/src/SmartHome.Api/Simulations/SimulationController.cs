using Microsoft.AspNetCore.Mvc;

namespace SmartHome.Api.Simulations;

/// <summary>
/// Simulation Controller: handles HTTP requests for simulation requests and coordinates responses between the client and application services.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SimulationController : ControllerBase
{

    private readonly ILogger<SimulationController> _logger;

    public SimulationController(ILogger<SimulationController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// PUT: api/simulation/speed
    /// </summary>
    [HttpPut("speed")]
    public IActionResult SetSimulationSpeed()
    {
        _logger.LogInformation("Simulation speed update requested.");
        return Ok("Simulation speed set (stub)");
    }

    /// <summary>
    /// PUT: api/simulation/reset
    /// </summary>
    [HttpPost("reset")]
    public IActionResult ResetSimulation()
    {
        _logger.LogInformation("Simulation reset requested.");
        return Ok("Simulation reset (stub)");
    }
}



