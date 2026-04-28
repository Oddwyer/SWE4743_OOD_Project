using Microsoft.AspNetCore.Mvc;
using SmartHome.Domain.Simulations;

namespace SmartHome.Api.Locations;

/// <summary>
/// Location Controller: handles HTTP requests for ambient temperature across locations between the client and application services.
/// </summary>

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ISimulationService _simulationService;
    public LocationsController(ISimulationService simulationService)
    {
        _simulationService = simulationService;
    }

    /// <summary>
    /// PUT: api/locations/{location}/ambient-temperature
    /// </summary>
    [HttpPut("{location}/ambient-temperature")]
    public IActionResult SetAmbientTemperature(string location, [FromBody] SetAmbientTemperatureRequest request)
    {
        _simulationService.SetAmbientTemperature(location, request.Temperature);
        return Ok(new
        {
            Location = location,
            AmbientTemperature = request.Temperature
        });
    }

    /// <summary>
    /// GET: api/locations/{location}/ambient-temperature
    /// </summary>
    [HttpGet("{location}/ambient-temperature")]
    public IActionResult GetAmbientTemperature(string location)
    {
        var temperature = _simulationService.GetAmbientTemperature(location);
        return Ok(new
        {
            Location = location,
            AmbientTemperature = temperature
        });

    }
}

