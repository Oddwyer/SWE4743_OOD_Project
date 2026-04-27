using Microsoft.AspNetCore.Mvc;
using System.Linq;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Services;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Factories;
using SmartHome.Domain;

namespace SmartHome.Api.Devices;

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

    // GET: api/devices
    [HttpGet]
    // Return item of type iterable list of DeviceResponses, and whether successful.
    [ProducesResponseType(typeof(IEnumerable<DeviceResponse>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<DeviceResponse>> GetAllDevices([FromQuery] DeviceFilter filter)
    {
        var devices = _deviceService.GetAllDevices(filter);

        // Map domain devices to API response models.
        var response = devices.Select(DeviceMapper.ToResponse);
        return Ok(response);
    }

    // GET: api/devices/{id}
    [HttpGet("{deviceId:guid}")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<DeviceResponse> GetDeviceById(Guid deviceId)
    {

        // Retrieve device to validate existence before operation.
        _logger.LogInformation("Fetching device with id {DeviceId}", deviceId);
        var device = _deviceService.GetDeviceById(deviceId);

        // Return 404 if device not found.
        if (device == null)
        {
            _logger.LogWarning("Device with ID {DeviceId} not found.", deviceId);
            return NotFound();
        }

        var response = DeviceMapper.ToResponse(device);
        return Ok(response);
    }

    // POST: api/devices/
    [HttpPost("register-device")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
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
            return BadRequest("Unable to create device. Please try again.");
        }
    }

    // DELETE: api/devices/{id}
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
            return NotFound();
        }

        _logger.LogInformation("Removing device with ID {DeviceId}", deviceId);
        _deviceService.RemoveDevice(device.Id);
        return NoContent();
    }

    // PUT: api/devices/{id}/state
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
            return NotFound();
        }

        // TODO: Replace stub with CommandFactory when concrete commands are implemented.
        var command = new StubDeviceCommand(device);
        _logger.LogInformation("Applying command {Command} to device {DeviceId}.", request.Command, deviceId);
        var updatedDevice = _deviceService.ApplyDeviceCommand(deviceId, command);
        var response = DeviceMapper.ToResponse(updatedDevice);
        return Ok(response);
    }


    // GET: api/devices/{id}/history
    [HttpGet("{deviceId:guid}/history")]
    [ProducesResponseType(typeof(IEnumerable<CommandHistoryEntry>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    public ActionResult<IEnumerable<CommandHistoryEntry>> GetDeviceHistory(Guid deviceId)
    {

        // Retrieve device to validate existence before operation.
        var device = _deviceService.GetDeviceById(deviceId);

        // Return 404 if device not found.
        if (device == null)
        {
            _logger.LogWarning("Device with ID {DeviceId} not found.", deviceId);
            return NotFound();
        }

        _logger.LogWarning("Command history for device with ID {DeviceId} provided.", deviceId);
        var history = _deviceService.GetCommandHistory(deviceId);
        return Ok(history);
    }

}


