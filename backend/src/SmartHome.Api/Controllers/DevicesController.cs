using Microsoft.AspNetCore.Mvc;
using System.Linq;
using SmartHome.Api.DTOs;
using SmartHome.Api.Mappers;
using SmartHome.Api;

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
        var devices = _deviceService.GetDevices();

        // For each device in devices, translate using DeviceMapper and add to response.
        var response = devices.Select(DeviceMapper.ToResponse);
        return Ok(response);
    }
}


