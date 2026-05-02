using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices.DoorLock;

namespace SmartHome.Domain.Commands;

/// <summary>
/// Locks doorlocks in the Smart Home.
/// </summary> 
public class ToggleLockCommand : DeviceCommand
{
    public override string CommandDescription => $"Locked {_doorLock.DeviceName}.";

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

    }

}
