using SmartHome.Domain.Devices;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Commands.Powered;

namespace SmartHome.Domain.Commands.Light;

/// <summary>
/// Creates command objects for light devices (toggle power, set brightness, set color).
/// </summary>
public class LightCommandFactory : IDeviceCommandFactory
{
    /// <summary>Creates the appropriate light command from the request context.</summary>
    public IDeviceCommand CreateCommand(IDevice device, CommandData context)
    {

        if (device is not LightDevice light)
        {
            throw new InvalidOperationException($"{device.DeviceName} is not a light and does not support light commands.");
        }

        return context.Command switch
        {
            DeviceCommandType.TogglePower => CreateTogglePowerCommand(light),

            DeviceCommandType.SetBrightness => CreateSetBrightnessCommand(light, context),

            DeviceCommandType.SetColor => CreateSetColorCommand(light, context),

            _ => throw new ArgumentException($"Unsupported command type.")
        };
    }

    /// <summary>Creates a TogglePower command for a light device.</summary>
    private IDeviceCommand CreateTogglePowerCommand(LightDevice light)
    {

        if (light is not IPoweredDevice poweredDevice)
        {
            throw new InvalidOperationException("This device does not support power control.");
        }

        return new TogglePowerCommand(light, poweredDevice);
    }

    /// <summary>Creates a SetBrightness command, validating the brightness value is present.</summary>
    private IDeviceCommand CreateSetBrightnessCommand(LightDevice light, CommandData context)
    {

        if (context.Brightness is null)
        {
            throw new ArgumentException("Brightness is required for setting brightness.");
        }

        return new SetLightBrightnessCommand(light, context.Brightness.Value);
    }

    /// <summary>Creates a SetColor command, validating the color value is present.</summary>
    private IDeviceCommand CreateSetColorCommand(LightDevice light, CommandData context)
    {

        if (context.Color is null)
        {
            throw new ArgumentException("Color is required for changing light color.");
        }
        return new SetLightColorCommand(light, context.Color);
    }
}

