using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands.Power;

/// <summary>
/// Toggles the power state of the device.
/// </summary>
public class TogglePowerCommand : DeviceCommand
{
    public override string CommandDescription => $"Toggled power of {ManipulatedDevice.DeviceName}.";

    private IPoweredDevice _poweredDevice;

    public TogglePowerCommand(IDevice device, IPoweredDevice poweredDevice) : base(device)
    {
        _poweredDevice = poweredDevice;
    }

    /// <summary>
    /// Executes the command to toggle the power state of the device. 
    /// </summary>
    public override void Execute()
    {

        _poweredDevice.TogglePower();

    }
}