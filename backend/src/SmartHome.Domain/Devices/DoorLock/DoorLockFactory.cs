using SmartHome.Domain.Contracts;

namespace SmartHome.Domain.Devices.DoorLock;

/// <summary>
/// Creates and restores door lock device instances.
/// </summary>
public class DoorLockFactory : IDeviceTypeFactory
{
    public DeviceType DeviceType => DeviceType.DoorLock;

    public DoorLockFactory()
    {

    }

    /// <summary>
    /// Creates door lock device instances.
    /// </summary>
    public IDevice CreateDevice(string name, string location)
    {
        Guid id = Guid.NewGuid();

        return new DoorLocks(id, name, location);

    }

    /// <summary>
    /// Restores a door lock from persisted values.
    /// </summary>
    public IDevice RehydrateDevice(DeviceRehydrationData data)
    {

        var doorlock = new DoorLocks(data.Id, data.Name, data.Location);

        var latchState = Enum.TryParse<DeviceLatchState>(data.DeviceState, ignoreCase: true, out var parsedState)
            ? parsedState
            : DeviceLatchState.Locked;

        doorlock.RehydrateState(latchState);

        return doorlock;
    }









}

