using SmartHome.Domain.Devices;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices.Light;

namespace SmartHome.Domain.Commands.Light;

public class LightCommandFactory : IDeviceCommandFactory
{
    public IDeviceCommand CreateCommand(IDevice device, CommandData context)
    {
        return context.Command switch
        {
            DeviceCommandType.SetBrightness => CreateSetBrightnessCommand(device, context),

            DeviceCommandType.SetColor => CreateSetColorCommand(device, context),
            _ => throw new ArgumentException($"Unsupported command type.")
        };
    }

    private IDeviceCommand CreateSetBrightnessCommand(IDevice device, CommandData context)
    {
        if (device is not LightDevice lightDevice)
        {
            throw new InvalidOperationException("This device does not have a brightness setting.");
        }
        if (context.Brightness is null)
        {
            throw new ArgumentException("Brightness is required for setting brightness.");
        }
        return new SetLightBrightnessCommand(lightDevice, context.Brightness.Value);
    }

    private IDeviceCommand CreateSetColorCommand(IDevice device, CommandData context)
    {
        if (device is not LightDevice colorLightDevice)
        {
            throw new InvalidOperationException("This device does not have a color setting.");
        }
        if (context.Color is null)
        {
            throw new ArgumentException("Color is required for changing light color.");
        }
        return new SetLightColorCommand(colorLightDevice, context.Color);
    }
}

