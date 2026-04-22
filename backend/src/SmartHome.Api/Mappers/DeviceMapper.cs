namespace SmartHome.Api.Mappers.DeviceMapper;

using SmartHome.Api.DTOs;
using SmartHome.Domain.Interfaces;

/// <summary>
/// Abstracts logic that varies by device type to avoid if/else statements in 
/// DeviceResponse DTO providing a clean, simple DeviceResponse to the controller. 
/// </summary>
/// 
public static class DeviceMapper
{
    public static DeviceResponse ToResponse(IDevice device)
    {
        var response = new DeviceResponse
        {
            Id = device.Id,
            DeviceName = device.DeviceName,
            DeviceLocation = device.DeviceLocation,
            Type = device.Type.ToString(),
            IsDeviceOn = device.IsDeviceOn,
        };

        // Response specifics assigned based on specified device type.
        /*switch (device)
        {
            case Light light:
                response.Brightness = light.Brightness;
                response.Color = light.Color;
                break;

            case Fan fan:
                response.Speed = fan.Speed;
                break;

            case Thermostat t:
                response.Mode = t.Mode;
                response.DesiredTemperature = t.DesiredTemperature;
                break;

            case DoorLock d:
                response.IsLocked = d.IsLocked;
                break;
        }*/

        return response;
    }
}