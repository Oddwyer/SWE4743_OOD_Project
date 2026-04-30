using SmartHome.Domain.Devices.DoorLock;

namespace SmartHome.Domain.Devices.DoorLock.DoorStates;

/// <summary>
/// Represents the locked state of a door lock.
/// </summary>
public class LockedState : IDoorState
{
    private readonly DoorLocks _doorLock;

    public LockedState(DoorLocks doorLock)
    {
        _doorLock = doorLock;
    }

    /// <summary>
    /// Unlocks the door and transitions to the Unlocked state.
    /// </summary>
    public void ToggleLock()
    {
        _doorLock.Unlock();
        _doorLock.SetState(_doorLock.Unlocked);
        _doorLock.UpdateStausMessage("Door unlocked.");
    }

}