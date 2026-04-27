using Microsoft.AspNetCore.Mvc;

namespace SmartHome.Api.Locations;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(ILogger<LocationsController> logger)
    {
        _logger = logger;
    }

    // PUT: api/locations/{location}/ambient-temperature
    [HttpPut("{location}/ambient-temperature")]
    public IActionResult SetAmbientTemperature(string location, [FromBody] SetAmbientTemperatureRequest request)
    {
        _logger.LogInformation("Ambient temperature update requested.");
        return Ok($"Ambient temperature for {location} set to {request.Temperature} (stub)");
    }
}

