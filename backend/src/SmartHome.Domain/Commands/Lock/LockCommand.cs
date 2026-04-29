using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands.Lock;

/// <summary>
/// Command to lock a device in the smart home system. It checks if the device can be locked.
/// </summary> 
public class LockCommand : DeviceCommand
{
    public override string CommandDescription => $"Locked {ManipulatedDevice.DeviceName}.";

    public LockCommand(IDevice device) : base(device)
    {

    }

    /// <summary>
    /// Executes the lock command by toggling the lock state of the device. 
    /// It first checks if the device implements the ILatchedDevice interface, and if it is not already locked. 
    /// If either condition is not met, it throws an InvalidOperationException.
    /// </summary>
    public override void Execute()
    {
        if (ManipulatedDevice is not ILatchedDevice latchedDevice)
        {
            throw new InvalidOperationException("This device cannot be locked.");
        }

        if (ManipulatedDevice.IsDeviceOn)
        {
            throw new InvalidOperationException("Device is already Locked.");
        }

        latchedDevice.ToggleLock();

    }

}