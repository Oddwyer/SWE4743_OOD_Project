using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands.Lock;

/// <summary>
/// Command to lock a device in the smart home system. 
/// </summary> 
public class ToggleLockCommand : DeviceCommand
{
    public override string CommandDescription => $"Locked {ManipulatedDevice.DeviceName}.";

    public ToggleLockCommand(IDevice device) : base(device)
    {

    }

    /// <summary>
    /// Executes the command to lock the device. 
    /// </summary>
    public override void Execute()
    {
        if (ManipulatedDevice is not ILatchedDevice latchedDevice)
        {
            throw new InvalidOperationException($"Device '{ManipulatedDevice.DeviceName}' does not support locking.");
        }

        latchedDevice.ToggleLock();

    }

}