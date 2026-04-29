using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands.Lock;

/// <summary>
/// Command to unlock a device in the smart home system. It checks if the device can be unlocked.
/// </summary>
public class UnlockCommand : DeviceCommand
{
    public override string CommandDescription => $"Unlocked {ManipulatedDevice.DeviceName}.";

    public UnlockCommand(IDevice device) : base(device)
    {

    }

    /// <summary>
    /// Executes the unlock command by toggling the lock state of the device.
    /// </summary>
    public override void Execute()
    {
        if (ManipulatedDevice is not ILatchedDevice latchedDevice)
        {
            throw new InvalidOperationException("This device cannot be unlocked.");
        }

        if (ManipulatedDevice.IsDeviceOn)
        {
            throw new InvalidOperationException("Device is already unLocked.");
        }

        latchedDevice.ToggleLock();

    }

}