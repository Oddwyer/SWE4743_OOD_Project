using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Light;

namespace SmartHome.Domain.Commands.Fan;

public class SetLightColorCommand : DeviceCommand
{
    public LightColorState Color { get; }
    public override string CommandDescription => $"Set light color to {Color} for {ManipulatedDevice.DeviceName}.";

    public SetLightColorCommand(IDevice device, LightColorState color) : base(device)
    {
        Color = color;
    }

    public override void Execute()
    {
        if (ManipulatedDevice is not LightDevice lightDevice)
        {
            throw new InvalidOperationException("This device does not have a color setting.");
        }

        if (!ManipulatedDevice.IsDeviceOn)
        {
            throw new InvalidOperationException("Light is not on.");
        }

        lightDevice.ChangeColor(Color);

    }

    public string Description => $"Color set to {Color} for {ManipulatedDevice.DeviceName}";
}