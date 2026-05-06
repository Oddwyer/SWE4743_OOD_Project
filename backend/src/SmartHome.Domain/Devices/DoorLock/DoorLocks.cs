using SmartHome.Domain.Devices.DoorLock.DoorStates;
using SmartHome.Domain.Commands.DoorLock;

namespace SmartHome.Domain.Devices.DoorLock;

/// <summary>
/// Represents a door lock device in the smart home system, which can be in either a locked or unlocked state.
/// </summary>
public class DoorLocks : Device, ILatchedDevice
{
    // States
    private DeviceLatchState _latchState = DeviceLatchState.Locked; // Default to locked state.
    public IDoorState Unlocked { get; private set; }
    public IDoorState Locked { get; private set; }
    private IDoorState _currentState;

    public DoorLocks(Guid id, string name, string location) : base(id, name, location, DeviceType.DoorLock, new DoorLockCommandFactory())
    {
        Unlocked = new UnlockedState(this);
        Locked = new LockedState(this);
        _currentState = Locked; // Default state is locked.
    }


    // Helper property to expose current state type for API responses and persistence.     
    public DeviceLatchState LatchState => _latchState;
    /// <summary>
    /// Indicates whether the door lock is currently locked (on) or unlocked (off).
    /// </summary>
    public override bool IsDeviceOn => true; // Always "on" for UI.

    /// <summary>
    /// Toggles the lock state of the door. 
    /// </summary>
    public void ToggleLock()
    {
        _currentState.ToggleLock();
    }

    /// <summary>
    /// Locks the door and transitions to the Locked state (used by state classes).
    /// </summary>
    internal void Lock()
    {
        _latchState = DeviceLatchState.Locked;
        UpdatedAt = DateTime.UtcNow;

    }

    /// <summary>
    /// Unlocks the door and transitions to the Unlocked state (used by state classes).
    /// </summary>
    internal void Unlock()
    {
        _latchState = DeviceLatchState.Unlocked;
        UpdatedAt = DateTime.UtcNow;

    }

    /// <summary>
    /// Sets the current state of the door lock (used by state classes to transition between states).
    /// </summary>
    internal void SetState(IDoorState newState)
    {
        _currentState = newState;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Restores device properties.
    /// <summary>
    internal void RehydrateState(DeviceLatchState latchState)
    {
        _latchState = latchState;
        _currentState = latchState == DeviceLatchState.Locked ? Locked : Unlocked;
    }

    public override void ResetToDefault()
    {
        _latchState = DeviceLatchState.Locked;
        _currentState = Locked;
        UpdatedAt = DateTime.UtcNow;
    }

}
