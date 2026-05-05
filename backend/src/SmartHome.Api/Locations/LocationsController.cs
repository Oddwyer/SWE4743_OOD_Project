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
    /// GET: api/locations/{location}/ambient-temperature
    /// </summary>
    [HttpGet("{location}/ambient-temperature")]
    [ProducesResponseType(typeof(AmbientTemperatureResponse), StatusCodes.Status200OK)]
    public IActionResult GetAmbientTemperature(string location)
    {
        var temperature = _simulationService.GetAmbientTemperature(location);
        return Ok(new AmbientTemperatureResponse
        {
            Location = location,
            AmbientTemperature = temperature,
            MinTemperature = SimulationService.MinAmbientTemperature,
            MaxTemperature = SimulationService.MaxAmbientTemperature
        });

    }

    /// <summary>
    /// PUT: api/locations/{location}/ambient-temperature
    /// </summary>
    [HttpPut("{location}/ambient-temperature")]
    [ProducesResponseType(typeof(AmbientTemperatureResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult SetAmbientTemperature(string location, [FromBody] SetAmbientTemperatureRequest request)
    {
        _simulationService.SetAmbientTemperature(location, request.Temperature);
        return Ok(new AmbientTemperatureResponse
        {
            Location = location,
            AmbientTemperature = request.Temperature
        });
    }

}

