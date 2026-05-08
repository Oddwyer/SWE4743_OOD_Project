using SmartHome.Domain.Devices;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Commands.Latched;

namespace SmartHome.Domain.Commands.DoorLock;

/// <summary>
/// Creates command objects for door lock devices (toggle lock state).
/// </summary>
public class DoorLockCommandFactory : IDeviceCommandFactory
{
    /// <summary>Creates a ToggleLock command for the door lock device.</summary>
    public IDeviceCommand CreateCommand(IDevice device, CommandData context)
    {
        if (device is not DoorLocks doorLock)
        {
            throw new InvalidOperationException($"{device.DeviceName} is not a door lock and does not support door lock commands.");
        }

        return CreateToggleLockCommand(doorLock);
    }

    /// <summary>Creates a ToggleLock command, verifying the device implements ILatchedDevice.</summary>
    private IDeviceCommand CreateToggleLockCommand(DoorLocks doorLock)
    {
        if (doorLock is not ILatchedDevice latchedDevice)
        {
            throw new InvalidOperationException("This device does not support latch control.");
        }

        return new ToggleLockCommand(doorLock, latchedDevice);
    }


}


