using SmartHome.Domain.Devices.DoorLock;

namespace SmartHome.Domain.Devices.DoorLock.DoorStates;

public class LockedState : IDoorState
{
    private readonly DoorLocks _doorLock;

    public LockedState(DoorLocks doorLock)
    {
        _doorLock = doorLock;
    }

    public void ToggleLock()
    {
        _doorLock.Unlock();
        _doorLock.SetState(_doorLock.Unlocked);
    }

}