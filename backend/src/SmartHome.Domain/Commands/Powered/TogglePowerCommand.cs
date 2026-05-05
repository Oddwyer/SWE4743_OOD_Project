using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands.Powered;

/// <summary>
/// Toggles the power state of the device.
/// </summary>
public class TogglePowerCommand : DeviceCommand
{

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
        _commandDescription = _poweredDevice.PowerState == DevicePowerState.On
       ? $"Powered on {ManipulatedDevice.DeviceName}."
       : $"Powered off {ManipulatedDevice.DeviceName}.";

    }
}
