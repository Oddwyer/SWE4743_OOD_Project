using SmartHome.Domain.Devices.DoorLock.DoorStates;

namespace SmartHome.Domain.Devices.DoorLock;

public class DoorLocks : Device, ILatchedDevice
{
    // States
    public DeviceLatchState LatchState { get; private set; }
    public IDoorState Unlocked { get; private set; }

    public IDoorState Locked { get; private set; }

    private IDoorState _currentState;

    public override bool IsDeviceOn => LatchState == DeviceLatchState.Locked;

    public DoorLocks(Guid id, string name, string location) : base(id, name, location, DeviceType.DoorLock)
    {
        LatchState = DeviceLatchState.Locked;
        Unlocked = new UnlockedState(this);
        Locked = new LockedState(this);
        _currentState = Locked;
    }

    public void ToggleLock()
    {
        _currentState.ToggleLock();
    }

    internal void Lock()
    {
        LatchState = DeviceLatchState.Locked;

    }
    internal void Unlock()
    {
        LatchState = DeviceLatchState.Unlocked;

    }

    internal void SetState(IDoorState newState)
    {
        _currentState = newState;
    }
}