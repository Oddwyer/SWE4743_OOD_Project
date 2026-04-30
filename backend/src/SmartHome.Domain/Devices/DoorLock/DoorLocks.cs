using SmartHome.Domain.Devices.DoorLock.DoorStates;

namespace SmartHome.Domain.Devices.DoorLock;

/// <summary>
/// Represents a door lock device in the smart home system, which can be in either a locked or unlocked state.
/// </summary>
public class DoorLocks : Device, ILatchedDevice
{
    // States
    public DeviceLatchState LatchState { get; private set; }
    public IDoorState Unlocked { get; private set; }
    public IDoorState Locked { get; private set; }

    private IDoorState _currentState;

    public DoorLocks(Guid id, string name, string location) : base(id, name, location, DeviceType.DoorLock)
    {
        LatchState = DeviceLatchState.Locked;
        Unlocked = new UnlockedState(this);
        Locked = new LockedState(this);
        _currentState = Locked;
    }

    /// <summary>
    /// Indicates whether the door lock is currently locked (on) or unlocked (off).
    /// </summary>
    public override bool IsDeviceOn => LatchState == DeviceLatchState.Locked;

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
        LatchState = DeviceLatchState.Locked;
        UpdatedAt = DateTime.UtcNow;

    }

    /// <summary>
    /// Unlocks the door and transitions to the Unlocked state (used by state classes).
    /// </summary>
    internal void Unlock()
    {
        LatchState = DeviceLatchState.Unlocked;
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
    /// Updates the status message (used by states). 
    /// </summary
    internal void UpdateStausMessage(string message)
    {
        StatusMessage = message;
        UpdatedAt = DateTime.UtcNow;
    }

}