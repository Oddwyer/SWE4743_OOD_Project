using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Light;

namespace SmartHome.Domain.Devices.Light;

/// <summary>
/// Sets the color of a light device. 
/// </summary>
public class SetLightColorCommand : DeviceCommand
{
    public LightColor Color { get; }

    private readonly LightDevice _lightDevice;
    public override string CommandDescription => $"Set light color to {Color} for {_lightDevice.DeviceName}.";

    public SetLightColorCommand(LightDevice device, LightColor color) : base(device)
    {
        _lightDevice = device;
        Color = color;
    }

    /// <summary>
    /// Executes the command to change the light color. 
    /// </summary>
    public override void Execute()
    {
        _lightDevice.ChangeColor(Color);
    }

}
