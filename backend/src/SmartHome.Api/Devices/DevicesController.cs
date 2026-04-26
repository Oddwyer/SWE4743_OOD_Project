using Microsoft.AspNetCore.Mvc;
using System.Linq;
using SmartHome.Api.Devices;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Services;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Factories;
using SmartHome.Domain;
using SmartHome.Api.Factories;

namespace SmartHome.Api.Controllers;

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

    // GET: api/devices
    [HttpGet]
    // Return item of type iterable list of DeviceResponses, and whether successful.
    [ProducesResponseType(typeof(IEnumerable<DeviceResponse>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<DeviceResponse>> GetAllDevices()
    {
        var devices = _deviceService.GetAllDevices();

        // For each device in devices, translate using DeviceMapper and add to response.
        var response = devices.Select(DeviceMapper.ToResponse);


        return Ok(response);
    }

    // GET: api/devices/{id}
    [HttpGet("{deviceId:guid}")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<DeviceResponse> GetDeviceById(Guid deviceId)
    {

        // Find and return device with matching ID.
        var device = _deviceService.GetDeviceById(deviceId);

        // If device could not be found, return status as not found.
        if (device == null)
        {
            //logger.error("Device not found.");
            return NotFound();
        }

        // Return found device and successful status.
        var response = DeviceMapper.ToResponse(device);
        return Ok(response);
    }

    // POST: api/devices/
    [HttpPost]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public ActionResult<DeviceResponse> RegisterDevice([FromBody] RegisterDeviceRequest request)
    {
        try
        {
            // Use factory to create IDevice
            var device = _deviceFactory.CreateDevice(
            request.DeviceName,
            request.DeviceLocation,
            request.Type
            );

            // If Device added, return successful status and creation details.
            _deviceService.RegisterDevice(device);
            var response = DeviceMapper.ToResponse(device);
            return CreatedAtAction(nameof(GetDeviceById), new { deviceId = device.Id }, response);
        }
        catch (ArgumentException ex)
        {
            //string message = "Unable to create device. Please try again.";
            //logger.error(message, ex);
            return BadRequest("Unable to create device. Please try again.");
        }
    }

    // DELETE: api/devices/{id}
    [HttpDelete("{deviceId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult RemoveDevice(Guid deviceId)
    {

        // Find and return device with matching ID.
        var device = _deviceService.GetDeviceById(deviceId);

        // If device could not be found, return status as not found.
        if (device == null)
        {
            //logger.error("Device not found.");
            return NotFound();
        }

        // Return found device and successfull status.
        _deviceService.RemoveDevice(device.Id);
        return Ok();
    }

    // PUT: api/devices/{id}/state
    [HttpPut("{deviceId:guid}/state")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<DeviceResponse> UpdateDevice(Guid deviceId, [FromBody] ControlDeviceRequest request)
    {
        var device = _deviceService.GetDeviceById(deviceId);

        if (device == null)
        {
            return NotFound();
        }

        // TODO: Replace w/ command factory logic
        var command = new StubDeviceCommand(device);

        var updatedDevice = _deviceService.ApplyDeviceCommand(deviceId, command);

        var response = DeviceMapper.ToResponse(updatedDevice);

        return NoContent();
    }


    // GET: api/devices/{id}/history
    /* TODO: Need GetCommandHistory method
    [HttpGet ("{deviceId: guid}/history")] 
    [ProducesResponseType(typeof(IEnumerable<CommandHistoryEntry>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    public ActionResult<IEnumerable<CommandHistoryEntry>> GetDeviceHistory(Guid deviceId){
        
        // Confirm device exists
        var devicce = _deviceService.GetDeviceById(deviceId);

        if (device == null)
        {
            //logger.error("Device not found.");
            return NotFound();
        }

        // Return device's command history and successful status.
        Need GetCommandHistory | var history = _deviceService.GetCommandHistory(deviceId);
        return Ok(history);
    }*/


}


