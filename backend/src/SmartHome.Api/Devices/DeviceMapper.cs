using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Devices;

namespace SmartHome.Api.Devices;

/// <summary>
/// Abstracts conditional logic that varies by device type to avoid if/else statements in 
/// DeviceResponse DTO providing a clean, simple DeviceResponse to the controller. 
/// </summary>

public static class DeviceMapper
{
    /// <summary>
    /// Converts a Device into a DeviceResponse.
    /// </summary>
    public static DeviceResponse ToResponse(IDevice device)
    {
        var response = new DeviceResponse
        {
            Id = device.Id,
            DeviceName = device.DeviceName,
            DeviceLocation = device.DeviceLocation,
            Type = device.Type,
            IsDeviceOn = device.IsDeviceOn,
        };

        // TODO - Amber: Waiting on thermostat; fan speed.
        // Response specifics assigned based on specified device type.
        switch (device)
        {
            case LightDevice light:
                response.Brightness = light.lightBrightness;
                response.Color = light.colorState;
                break;

            case FanDevice fan:
                response.Speed = fan.Speed;
                break;

            /*case Thermostat thermostat:
                response.Mode = thermostat.Mode;
                response.DesiredTemperature = thermostat.DesiredTemperature;
                break;
            */

            case DoorLocks doorlock:
                response.IsLocked = doorlock.IsDeviceOn;
                break;
        }

        return response;
    }

}