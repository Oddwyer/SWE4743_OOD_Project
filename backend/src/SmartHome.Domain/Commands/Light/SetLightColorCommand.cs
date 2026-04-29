using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Light;

namespace SmartHome.Domain.Commands.Light;

/// <summary>
/// Command to set the color of a light device. This command checks if the device is a light and
/// if it is currently on before changing its color. If the device is not a light or is off, it throws an exception.
/// </summary>
public class SetLightColorCommand : DeviceCommand
{
    public LightColorState Color { get; }
    public override string CommandDescription => $"Set light color to {Color} for {ManipulatedDevice.DeviceName}.";

    public SetLightColorCommand(IDevice device, LightColorState color) : base(device)
    {
        Color = color;
    }

    /// <summary>
    /// Executes the command to change the light color. It first checks if the manipulated device is a light and if it is on.
    /// </summary>
    public override void Execute()
    {
        if (ManipulatedDevice is not LightDevice lightDevice)
        {
            throw new InvalidOperationException("This device does not have a color setting.");
        }

        if (!lightDevice.IsDeviceOn)
        {
            throw new InvalidOperationException("Light is not on.");
        }

        lightDevice.ChangeColor(Color);

    }

}