using Microsoft.AspNetCore.Mvc;

namespace SmartHome.Api.Simulations;

/// <summary>
/// Simulation Controller: handles HTTP requests for simulation requests and coordinates responses between the client and application services.
/// </summary>

[ApiController]
[Route("api/[controller]")]
public class SimulationController : ControllerBase
{
    public SimulationController()
    {

    }

    /// <summary>
    /// POST: api/simulation/reset
    /// </summary>
    [HttpPost("reset")]
    public IActionResult ResetSimulation()
    {
        return Ok("Simulation reset (stub)");
    }

    /// <summary>
    /// PUT: api/simulation/speed
    /// </summary>
    [HttpPut("speed")]
    public IActionResult SetSimulationSpeed()
    {
        return Ok("Simulation speed set (stub)");
    }


}



