using Microsoft.AspNetCore.Mvc;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Simulations;

namespace SmartHome.Api.Devices;

/// <summary>
/// Device Controller: handles HTTP requests for device operations and coordinates responses between the client and application services.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;
    private readonly ISimulationService _simulationService;

    public DevicesController(IDeviceService deviceService, ISimulationService simulationService)
    {
        _deviceService = deviceService;
        _simulationService = simulationService;
    }

    /// <summary>
    /// GET: api/devices
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DeviceResponse>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<DeviceResponse>> GetAllDevices([FromQuery] DeviceFilter filter)
    {
        var devices = _deviceService.GetAllDevices(filter);

        // Map domain devices to API response models.
        var response = devices.Select(DeviceMapper.ToResponse);

        return Ok(response);
    }

    /// <summary>
    /// GET: api/devices/{id}
    /// </summary>
    [HttpGet("{deviceId:guid}")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<DeviceResponse> GetDeviceById(Guid deviceId)
    {

        // Retrieve device to validate existence before operation.
        var device = _deviceService.GetDeviceById(deviceId);
        var ambient = _simulationService.GetAmbientTemperature(device.DeviceLocation); // Get ambient temperature for device location

        var response = DeviceMapper.ToResponse(device, ambient); // Example ambient temperature

        return Ok(response);
    }

    /// <summary>
    /// GET: api/devices/{id}/history
    /// </summary>
    [HttpGet("{deviceId:guid}/history")]
    [ProducesResponseType(typeof(IEnumerable<CommandHistoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<CommandHistoryResponse>> GetDeviceHistory(Guid deviceId)
    {

        var history = _deviceService.GetCommandHistory(deviceId);
        var response = CommandHistoryMapper.ToResponse(history);

        return Ok(response);
    }

    /// <summary>
    /// POST: api/devices/
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<DeviceResponse> RegisterDevice([FromBody] RegisterDeviceRequest request)
    {

        var registerData = DeviceMapper.MapToRegisterData(request);
        // If Device added, return success status and creation details.
        var device = _deviceService.RegisterDevice(registerData);
        var ambient = _simulationService.GetAmbientTemperature(device.DeviceLocation); // Get ambient temperature for device location
        var response = DeviceMapper.ToResponse(device, ambient);
        return CreatedAtAction(nameof(GetDeviceById), new { deviceId = device.Id }, response);
    }

    /// <summary>
    /// PUT: api/devices/{id}/commands
    /// </summary>
    [HttpPut("{deviceId:guid}/commands")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<DeviceResponse> UpdateDevice(Guid deviceId, [FromBody] ControlDeviceRequest request)
    {

        var context = DeviceMapper.MapToCommandData(request);
        var updatedDevice = _deviceService.ApplyDeviceCommand(deviceId, context);
        var ambient = _simulationService.GetAmbientTemperature(updatedDevice.DeviceLocation); // Get ambient temperature for device location
        var response = DeviceMapper.ToResponse(updatedDevice, ambient);

        return Ok(response);

    }

    /// <summary>
    /// DELETE: api/devices/{id}
    /// </summary>
    [HttpDelete("{deviceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult RemoveDevice(Guid deviceId)
    {
        _deviceService.RemoveDevice(deviceId);

        return NoContent();
    }
}


