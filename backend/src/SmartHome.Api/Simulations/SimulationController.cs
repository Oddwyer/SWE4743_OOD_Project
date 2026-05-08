using Microsoft.AspNetCore.Mvc;
using SmartHome.Domain.Simulations;
using SmartHome.Api.SSE;

namespace SmartHome.Api.Simulations;

/// <summary>
/// Simulation Controller: handles HTTP requests for simulation requests and coordinates responses between the client and application services.
/// </summary>

[ApiController]
[Route("api/[controller]")]
public class SimulationController : ControllerBase
{
    private readonly ISimulationService _simulationService;

    private readonly IEventBroadcaster _eventBroadcaster; // SSE

    public SimulationController(ISimulationService simulationService, IEventBroadcaster eventBroadcaster)
    {
        _simulationService = simulationService;
        _eventBroadcaster = eventBroadcaster;
    }

    /// <summary>
    /// POST: api/simulation/reset
    /// </summary>
    [HttpPost("reset")]
    [ProducesResponseType(typeof(SimulationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetSimulation()
    {
        _simulationService.ResetSimulation();
        await _eventBroadcaster.BroadcastDeviceChangedAsync(Guid.Empty);
        return Ok(new SimulationResponse
        {
            Message = "Simulation reset successfully."
        });
    }

    /// <summary>
    /// PUT: api/simulation/speed
    /// </summary>
    [HttpPut("speed")]
    [ProducesResponseType(typeof(SimulationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult SetSimulationSpeed([FromBody] SimulationSpeedRequest request)
    {
        _simulationService.SetSimulationSpeed(request.SpeedMultiplier);
        return Ok(new SimulationResponse
        {
            Message = $"Simulation speed set to {request.SpeedMultiplier}.",
            SpeedMultiplier = request.SpeedMultiplier
        });
    }
}



