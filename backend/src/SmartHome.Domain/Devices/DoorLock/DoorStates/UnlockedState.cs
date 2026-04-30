namespace SmartHome.Domain.Devices.DoorLock.DoorStates;

/// <summary>
/// Represents the unlocked state of a door lock. 
/// </summary>
public class UnlockedState : IDoorState
{
    private readonly DoorLocks _doorLock;

    public UnlockedState(DoorLocks doorLock)
    {
        _doorLock = doorLock;
    }

    /// <summary>
    /// Toggles the lock state to locked. 
    /// </summary>
    public void ToggleLock()
    {
        _doorLock.Lock();
        _doorLock.SetState(_doorLock.Locked);
        _doorLock.UpdateStausMessage("Door locked.");
    }

}