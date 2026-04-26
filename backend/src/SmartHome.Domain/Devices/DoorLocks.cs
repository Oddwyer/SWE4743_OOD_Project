
namespace SmartHome.Domain.Devices;

public class DoorLocks : Device, ILatchedDevice
{
    public DeviceLatchState latchState { get; private set; }

    public override bool IsDeviceOn => latchState == DeviceLatchState.Locked;

    public DoorLocks(Guid id, string name, string location) : base(id, name, location, DeviceType.DoorLock)
    {
        latchState = DeviceLatchState.Locked;
    }

    public void ToggleLatch()
    {
        // using ternary operators again for toggling the latch state
        latchState = latchState == DeviceLatchState.Locked 
        ? DeviceLatchState.Unlocked 
        : DeviceLatchState.Locked;
       // UpdatedAt = DateTime.UtcNow;
    }
}