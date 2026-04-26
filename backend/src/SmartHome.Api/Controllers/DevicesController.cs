using Microsoft.AspNetCore.Mvc;
using System.Linq;
using SmartHome.Api.DTOs;
using SmartHome.Api.Mappers;
using SmartHome.Domain;
using SmartHome.Domain.Interfaces;

namespace SmartHome.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;
    private IDeviceFactory _deviceFactory;

    public DevicesController(IDeviceService deviceService, IDeviceFactory deviceFactory)
    {
        _deviceService = deviceService;
        _deviceFactory = deviceFactory;
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
    [HttpGet("{deviceId}")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<DeviceResponse> GetDeviceById(Guid deviceId)
    {

       // Find and return device with matching ID.
        var device = _deviceService.GetDeviceById(deviceId);

        // If device could not be found, return status as not found.
        if (device == null)
        {
            return NotFound();
        }

        // Return found device and successfull status.
        var response = DeviceMapper.ToResponse(device);
        return Ok(response);
    }

    // POST: api/devices/
    [HttpPost]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<DeviceResponse> AddDevice(RegisterDeviceRequest request)
    {
        // Decompose request
        var name = request.DeviceName;
        var location = request.DeviceLocation;
        var type = request.Type;

        // Use factory to create IDevice



  
    }
    
    // DELETE: api/devices/{id}
    [HttpDelete("{deviceId}")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult RemoveDevice(Guid deviceId)
    {

       // Find and return device with matching ID.
        var device = _deviceService.GetDeviceById(deviceId);

        // If device could not be found, return status as not found.
        if (device == null)
        {
            return NotFound();
        }

        // Return found device and successfull status.
        _deviceService.RemoveDevice(device.Id);
        return Ok();
    }
}


