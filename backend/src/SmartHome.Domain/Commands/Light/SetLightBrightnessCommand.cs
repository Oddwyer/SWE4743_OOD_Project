using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Light;

namespace SmartHome.Domain.Commands.Light;

/// <summary>
/// Command to set the brightness of a light device. The brightness must be between 10% and 100%. 
/// This command will throw an exception if the device is not a light or if the light is not currently on.
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
    /// It first checks if the manipulated device is a light and if it is currently on. 
    /// If either of these conditions is not met, it throws an InvalidOperationException. 
    /// If both conditions are satisfied, it sets the brightness of the light device to the specified value.
    /// </summary>

    public override void Execute()
    {
        if (ManipulatedDevice is not LightDevice lightDevice)
        {
            throw new InvalidOperationException("This device does not have a brightness setting.");
        }

        if (!lightDevice.IsDeviceOn)
        {
            throw new InvalidOperationException("Light is not on.");
        }

        if (Brightness < 10 || Brightness > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(Brightness), "Brightness must be between 10 and 100.");
        }

        lightDevice.SetLightBrightness(Brightness);

    }

}