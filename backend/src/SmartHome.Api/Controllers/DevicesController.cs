using Microsoft.AspNetCore.Mvc;
using System.Linq;
using SmartHome.Api.DTOs;
using SmartHome.Api.Mappers;
using SmartHome.Domain;

namespace SmartHome.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevicesController: ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    // Get endpoint to list all devices.
    [HttpGet]
    // Return item of type iterable list of DeviceResponses, and whether successful.
    [ProducesResponseType(typeof(IEnumerable<DeviceResponse>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<DeviceResponse>> GetDevices()
    {
        var devices = _deviceService.GetAllDevices();

        // For each device in devices, translate using DeviceMapper and add to response.
        var response = devices.Select(DeviceMapper.ToResponse);

    
        return Ok(response);
    }

    //TODO:Methods for: 
    //1. [HttpGet] -> single device
    //2. [HttpPOST] -> register device
    //3. [HttpPUT] -> update device
    //4. [HttpDelete] -> delete device
}


