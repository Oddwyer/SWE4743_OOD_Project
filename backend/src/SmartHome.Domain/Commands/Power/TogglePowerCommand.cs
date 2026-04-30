using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands.Power;

/// <summary>
/// Toggles the power state of the device.
/// </summary>
public class TogglePowerCommand : DeviceCommand
{
    public override string CommandDescription => $"Toggled power of {ManipulatedDevice.DeviceName}.";

    public TogglePowerCommand(IDevice device) : base(device)
    {

    }

    /// <summary>
    /// Executes the command to toggle the power state of the device. It checks if the device can be toggled
    /// and performs the toggle operation.
    /// </summary>
    public override void Execute()
    {
        if (ManipulatedDevice is not IPoweredDevice poweredDevice)
        {
            throw new InvalidOperationException($"Device '{ManipulatedDevice.DeviceName}' does not support power control.");
        }

        poweredDevice.TogglePower();

    }
}