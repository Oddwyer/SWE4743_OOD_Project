using Microsoft.AspNetCore.Mvc;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Commands;

namespace SmartHome.Api.Devices;

/// <summary>
/// Device Controller: handles HTTP requests for device operations and coordinates responses between the client and application services.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;
    private readonly IDeviceFactory _deviceFactory;
    private readonly ICommandFactory _commandFactory;

    public DevicesController(IDeviceService deviceService, IDeviceFactory deviceFactory, ICommandFactory commandFactory)
    {
        _deviceService = deviceService;
        _deviceFactory = deviceFactory;
        _commandFactory = commandFactory;
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

        var response = DeviceMapper.ToResponse(device);

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
    [HttpPost("register-device")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<DeviceResponse> RegisterDevice([FromBody] RegisterDeviceRequest request)
    {

        // Create device via factory.
        var device = _deviceFactory.CreateDevice(
        request.DeviceName,
        request.DeviceLocation,
        request.Type
        );

        // If Device added, return success status and creation details.
        _deviceService.RegisterDevice(device);
        var response = DeviceMapper.ToResponse(device);

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
        // Retrieve device to validate existence before operation.
        var device = _deviceService.GetDeviceById(deviceId);

        // TODO: Amber: Replace stub with CommandFactory when concrete commands are implemented.
        var command = new StubDeviceCommand(device);

        var updatedDevice = _deviceService.ApplyDeviceCommand(deviceId, command);
        var response = DeviceMapper.ToResponse(updatedDevice);

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


