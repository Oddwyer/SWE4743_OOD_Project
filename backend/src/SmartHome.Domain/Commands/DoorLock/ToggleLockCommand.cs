using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands;

/// <summary>
/// Locks doorlocks in the Smart Home.
/// </summary> 
public class ToggleLockCommand : DeviceCommand
{

    private readonly DoorLocks _doorLock;

    public ToggleLockCommand(DoorLocks device) : base(device)
    {
        _doorLock = device;
    }

    /// <summary>
    /// Executes the command to lock the device. 
    /// </summary>
    public override void Execute()
    {

        _doorLock.ToggleLock();
        _commandDescription = _doorLock.LatchState == DeviceLatchState.Locked
             ? $"Locked {_doorLock.DeviceName}."
             : $"Unlocked {_doorLock.DeviceName}.";

    }

}
