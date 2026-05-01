using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Light;

namespace SmartHome.Domain.Commands.Light;

/// <summary>
/// Sets the brightness of a light device.
/// </summary>
public class SetLightBrightnessCommand : DeviceCommand
{
    public int Brightness { get; }
    public override string CommandDescription => $"Set light brightness to {Brightness} for {ManipulatedDevice.DeviceName}.";

    public SetLightBrightnessCommand(IDevice device, int brightness) : base(device)
    {
        Brightness = brightness;
    }

    /// <summary>
    /// Executes the command to set the brightness of the light device. 
    /// </summary>
    public override void Execute()
    {
        if (ManipulatedDevice is not LightDevice lightDevice)
        {
            throw new InvalidOperationException("This device does not have a brightness setting.");
        }

        lightDevice.SetLightBrightness(Brightness);

    }

}