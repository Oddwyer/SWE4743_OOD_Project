namespace SmartHome.Domain.Devices.DoorLock.DoorStates;

public class UnlockedState : IDoorState
{
    private readonly DoorLocks _doorLock;

    public UnlockedState(DoorLocks doorLock)
    {
        _doorLock = doorLock;
    }

    public void ToggleLock()
    {
        _doorLock.Lock();
        _doorLock.SetState(_doorLock.Locked);
    }

}