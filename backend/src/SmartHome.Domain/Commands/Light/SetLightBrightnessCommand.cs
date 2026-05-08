using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Light;

namespace SmartHome.Domain.Commands.Light;

/// <summary>
/// Sets the brightness of a light device.
/// </summary>
public class SetLightBrightnessCommand : DeviceCommand
{
    public int Brightness { get; }

    private readonly LightDevice _lightDevice;

    public SetLightBrightnessCommand(LightDevice device, int brightness) : base(device)
    {
        Brightness = brightness;
        _lightDevice = device;
    }

    /// <summary>
    /// Executes the command to set the brightness of the light device. 
    /// </summary>
    public override void Execute()
    {

        _lightDevice.SetLightBrightness(Brightness);
        _commandDescription = $"Set light brightness to {Brightness}% for {_lightDevice.DeviceName}.";

    }

}
