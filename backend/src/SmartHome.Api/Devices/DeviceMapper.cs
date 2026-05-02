using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Contracts;

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

    public static DeviceResponse ToResponse(IDevice device, int ambientTemperature)
    {
        var response = new DeviceResponse
        {
            Id = device.Id,
            DeviceName = device.DeviceName,
            DeviceLocation = device.DeviceLocation,
            Type = device.Type,
            IsDeviceOn = device.IsDeviceOn,
            CreatedAt = device.CreatedAt,
            UpdatedAt = device.UpdatedAt
        };

        // Response specifics assigned based on specified device type.
        switch (device)
        {
            case LightDevice light:
                response.LightBrightness = light.LightBrightness;
                response.LightColor = light.Color;
                break;

            case FanDevice fan:
                response.FanSpeed = fan.Speed;
                break;

            case ThermostatDevice thermostat:
                response.ThermostatMode = thermostat.Mode;
                response.TargetTemperature = thermostat.TargetTemperature;
                response.AmbientTemperature = ambientTemperature;
                response.ThermostatState = thermostat.CurrentStateType; // Convert enum to string for API response
                break;

            case DoorLocks doorlock:
                response.IsLocked = doorlock.LatchState == DeviceLatchState.Locked;
                break;

            default:
                throw new ArgumentException($"Unsupported device type: {device.Type}");
        }

        return response;
    }

    public static CommandData MapToCommandData(ControlDeviceRequest request)
    {
        var context = new CommandData
        {
            Command = request.Command,
            Brightness = request.Brightness,
            Color = request.Color,
            FanSpeed = request.FanSpeed,
            Mode = request.Mode,
            TargetTemperature = request.TargetTemperature
        };

        return context;
    }

    public static RegisterDeviceData MapToRegisterData(RegisterDeviceRequest request)
    {
        var registerData = new RegisterDeviceData
        {
            DeviceName = request.DeviceName,
            DeviceLocation = request.DeviceLocation,
            DeviceType = request.Type
        };

        return registerData;
    }
}
