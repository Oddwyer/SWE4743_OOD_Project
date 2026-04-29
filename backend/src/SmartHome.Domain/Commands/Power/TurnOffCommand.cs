using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands.Power;

/// <summary>
/// Command to turn off a device in the smart home system. It checks if the device can be powered off.
/// </summary>
public class TurnOffCommand : DeviceCommand
{
    public override string CommandDescription => $"Turned off {ManipulatedDevice.DeviceName}.";

    public TurnOffCommand(IDevice device) : base(device)
    {

    }

    /// <summary>
    /// Executes the command to turn off the device. It checks if the device can be powered off
    /// and if it is already off before toggling the power state.
    /// </summary>
    public override void Execute()
    {
        if (ManipulatedDevice is not IPoweredDevice poweredDevice)
        {
            throw new InvalidOperationException("This device cannot be powered off.");
        }

        if (ManipulatedDevice.IsDeviceOn)
        {
            throw new InvalidOperationException("Device is already OFF.");
        }

        poweredDevice.TogglePower();

    }
}