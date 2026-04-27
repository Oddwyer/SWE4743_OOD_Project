using SmartHome.Domain.Devices;

namespace SmartHome.Api.Devices;

/// <summary>
/// Abstracts conditional logic that varies by device type to avoid if/else statements in 
/// DeviceResponse DTO providing a clean, simple DeviceResponse to the controller. 
/// </summary>

public static class DeviceMapper
{
    // Converts device details to UI-friendly text.
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

        // TODO - Amber: Waiting on thermostat; fan speed.
        // Response specifics assigned based on specified device type.
        switch (device)
        {
            case LightDevice light:
                response.Brightness = light.lightBrightness;
                response.Color = light.colorState.ToString();
                break;

            /*case FanDevice fan:
                response.Speed = fan.FanSpeed;
                break;

            case Thermostat thermostat:
                response.Mode = t.Mode;
                response.DesiredTemperature = t.DesiredTemperature;
                break;
            */

            case DoorLocks doorlock:
                response.IsLocked = doorlock.IsDeviceOn;
                break;
        }

        return response;
    }

}