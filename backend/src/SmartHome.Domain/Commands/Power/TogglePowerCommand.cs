using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands.Power;

/// <summary>
/// Toggles the power state of the device.
/// </summary>
public class TogglePowerCommand : DeviceCommand
{

    private IPoweredDevice _poweredDevice;

    public TogglePowerCommand(IDevice device) : base(device)
    {
        _poweredDevice = device as IPoweredDevice ?? throw new ArgumentException("Device must implement IPoweredDevice interface.");
    }

    /// <summary>
    /// Executes the command to toggle the power state of the device. 
    /// </summary>
    public override void Execute()
    {

        _poweredDevice.TogglePower();
        _commandDescription = _poweredDevice.PowerState == DevicePowerState.On
       ? $"Powered on {ManipulatedDevice.DeviceName}."
       : $"Powered off {ManipulatedDevice.DeviceName}.";

    }
}
