using Microsoft.AspNetCore.Mvc;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Commands;
using SmartHome.Domain;

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

        // Return 404 if device not found.
        if (device == null)
        {
            return NotFound(new { message = $"Device with ID {deviceId} not found." });
        }

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
        // Retrieve device to validate existence before operation.
        var device = _deviceService.GetDeviceById(deviceId);

        // Return 404 if device not found.
        if (device == null)
        {
            return NotFound(new { message = $"Device with ID {deviceId} not found." });
        }

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
        try
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
        catch (ArgumentException ex)
        {
            // Return 400 if device could not be created.
            return BadRequest(new
            {
                error = ex.Message,
                message = "Unable to create device. Please try again."
            });
            // TODO - Amber: Consider catch for invalid operation when attempting to add more than one thermostat per location.
        }
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

        // Return 404 if device not found.
        if (device == null)
        {
            return NotFound(new { message = $"Device with ID {deviceId} not found." });
        }

        if (request == null || string.IsNullOrWhiteSpace(request.Command))
        {
            return BadRequest(new { message = "Command is required." });
        }


        try
        {
            // TODO: Amber: Replace stub with CommandFactory when concrete commands are implemented.
            var command = new StubDeviceCommand(device);

            var updatedDevice = _deviceService.ApplyDeviceCommand(deviceId, command);
            var response = DeviceMapper.ToResponse(updatedDevice);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                error = ex.Message,
                message = "Invalid command request."
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                error = ex.Message,
                message = "Command cannot be applied to the device in its current state."
            });
        }
    }

    /// <summary>
    /// DELETE: api/devices/{id}
    /// </summary>
    [HttpDelete("{deviceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult RemoveDevice(Guid deviceId)
    {

        // Retrieve device to validate existence before operation.
        var device = _deviceService.GetDeviceById(deviceId);

        // Return 404 if device not found.
        if (device == null)
        {
            return NotFound(new { message = $"Device with ID {deviceId} not found." });
        }
        _deviceService.RemoveDevice(device.Id);

        return NoContent();
    }
}


