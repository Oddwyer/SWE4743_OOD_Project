using Microsoft.AspNetCore.Mvc;
using SmartHome.Domain.Simulations;

namespace SmartHome.Api.Simulations;

/// <summary>
/// Simulation Controller: handles HTTP requests for simulation requests and coordinates responses between the client and application services.
/// </summary>

[ApiController]
[Route("api/[controller]")]
public class SimulationController : ControllerBase
{
    private readonly ISimulationService _simulationService;

    public SimulationController(ISimulationService simulationService)
    {
        _simulationService = simulationService;
    }

    /// <summary>
    /// POST: api/simulation/reset
    /// </summary>
    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult ResetSimulation()
    {
        _simulationService.ResetSimulation();
        return Ok(new
        {
            Message = "Simulation reset successfully."
        });
    }

    /// <summary>
    /// PUT: api/simulation/speed
    /// </summary>
    [HttpPut("speed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult SetSimulationSpeed([FromBody] SetSimulationSpeedRequest request)
    {
        _simulationService.SetSimulationSpeed(request.SpeedMultiplier);
        return Ok(new
        {
            Message = $"Simulation speed set to {request.SpeedMultiplier}x."
        });
    }
}



