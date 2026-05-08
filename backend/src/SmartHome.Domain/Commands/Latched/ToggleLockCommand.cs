using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands.Latched;

/// <summary>
/// Locks doorlocks in the Smart Home.
/// </summary> 
public class ToggleLockCommand : DeviceCommand
{

    private ILatchedDevice _latchedDevice;

    public ToggleLockCommand(IDevice device, ILatchedDevice latchedDevice) : base(device)
    {
        _latchedDevice = latchedDevice;
    }

    /// <summary>
    /// Executes the command to lock the device. 
    /// </summary>
    public override void Execute()
    {

        _latchedDevice.ToggleLock();
        _commandDescription = _latchedDevice.LatchState == DeviceLatchState.Locked
             ? $"Locked {ManipulatedDevice.DeviceName}."
             : $"Unlocked {ManipulatedDevice.DeviceName}.";

    }

}
