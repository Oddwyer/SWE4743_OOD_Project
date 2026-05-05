using SmartHome.Domain.Devices;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Commands.Powered;

namespace SmartHome.Domain.Commands.Light;

public class LightCommandFactory : IDeviceCommandFactory
{
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

    private IDeviceCommand CreateTogglePowerCommand(LightDevice light)
    {

        if (light is not IPoweredDevice poweredDevice)
        {
            throw new InvalidOperationException("This device does not support power control.");
        }

        return new TogglePowerCommand(light, poweredDevice);
    }

    private IDeviceCommand CreateSetBrightnessCommand(LightDevice light, CommandData context)
    {

        if (context.Brightness is null)
        {
            throw new ArgumentException("Brightness is required for setting brightness.");
        }

        return new SetLightBrightnessCommand(light, context.Brightness.Value);
    }

    private IDeviceCommand CreateSetColorCommand(LightDevice light, CommandData context)
    {

        if (context.Color is null)
        {
            throw new ArgumentException("Color is required for changing light color.");
        }
        return new SetLightColorCommand(light, context.Color);
    }
}

