using Microsoft.AspNetCore.Mvc;
//using System.Linq;
//using SmartHome.Domain.Devices;
using SmartHome.Domain.Services;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Factories;
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
    private readonly ILogger<DevicesController> _logger;

    public DevicesController(IDeviceService deviceService, IDeviceFactory deviceFactory, ICommandFactory commandFactory, ILogger<DevicesController> logger)
    {
        _deviceService = deviceService;
        _deviceFactory = deviceFactory;
        _commandFactory = commandFactory;
        _logger = logger;
    }

    /// <summary>
    /// GET: api/devices
    /// Return item of type iterable list of DeviceResponses, and whether successful.
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
        _logger.LogInformation("Fetching device with id {DeviceId}.", deviceId);
        var device = _deviceService.GetDeviceById(deviceId);

        // Return 404 if device not found.
        if (device == null)
        {
            _logger.LogWarning("Device with ID {DeviceId} not found.", deviceId);
            return NotFound(new { message = $"Device with ID {deviceId} not found." });
        }

        var response = DeviceMapper.ToResponse(device);

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
            // Log request and create device via factory.
            _logger.LogInformation("Registering new device {Name} at {Location}.", request.DeviceName, request.DeviceLocation);
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
            _logger.LogError(ex, "Failed to create device.");
            return BadRequest(new
            {
                error = ex.Message,
                message = "Unable to create device. Please try again."
            });
            // TODO - Amber: Consider catch for invalid operation when attempting to add more than one thermostat per location.
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
            _logger.LogWarning("Device with ID {DeviceId} not found.", deviceId);
            return NotFound(new { message = $"Device with ID {deviceId} not found." });
        }

        _logger.LogInformation("Removing device with ID {DeviceId}.", deviceId);
        _deviceService.RemoveDevice(device.Id);

        return NoContent();
    }

    /// <summary>
    /// PUT: api/devices/{id}/state
    /// </summary>
    [HttpPut("{deviceId:guid}/state")]
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
            _logger.LogWarning("Device with ID {DeviceId} not found.", deviceId);
            return NotFound(new { message = $"Device with ID {deviceId} not found." });
        }

        if (request == null || string.IsNullOrWhiteSpace(request.Command))
        {
            return BadRequest(new { message = "Command is required." });
        }


        try
        {
            var command = new StubDeviceCommand(device); // TODO: Amber: Replace stub with CommandFactory when concrete commands are implemented.

            _logger.LogInformation("Applying command {Command} to device {DeviceId}.", request.Command, deviceId);

            var updatedDevice = _deviceService.ApplyDeviceCommand(deviceId, command);
            var response = DeviceMapper.ToResponse(updatedDevice);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid command {Command} for device {DeviceId}.", request.Command, deviceId);

            return BadRequest(new
            {
                error = ex.Message,
                message = "Invalid command request."
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Command {Command} cannot be applied to device {DeviceId}.", request.Command, deviceId);

            return BadRequest(new
            {
                error = ex.Message,
                message = "Command cannot be applied to the device in its current state."
            });
        }
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
            _logger.LogWarning("Device with ID {DeviceId} not found.", deviceId);
            return NotFound(new { message = $"Device with ID {deviceId} not found." });
        }

        _logger.LogInformation("Fetching history for device with ID {DeviceId}.", deviceId);
        var history = _deviceService.GetCommandHistory(deviceId);
        var response = CommandHistoryMapper.ToResponse(history);

        return Ok(response);
    }

}


