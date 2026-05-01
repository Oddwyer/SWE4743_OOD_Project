using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Light;

namespace SmartHome.Domain.Commands.Light;

/// <summary>
/// Sets the color of a light device. 
/// </summary>
public class SetLightColorCommand : DeviceCommand
{
    public LightColor Color { get; }
    public override string CommandDescription => $"Set light color to {Color} for {ManipulatedDevice.DeviceName}.";

    public SetLightColorCommand(IDevice device, LightColor color) : base(device)
    {
        Color = color;
    }

    /// <summary>
    /// Executes the command to change the light color. 
    /// </summary>
    public override void Execute()
    {
        if (ManipulatedDevice is not LightDevice lightDevice)
        {
            throw new InvalidOperationException("This device does not have a color setting.");
        }

        lightDevice.ChangeColor(Color);

    }

}