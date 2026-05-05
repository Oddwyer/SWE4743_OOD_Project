using SmartHome.Domain.Devices;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Commands.Latched;

namespace SmartHome.Domain.Commands.DoorLock;

public class DoorLockCommandFactory : IDeviceCommandFactory
{
    public IDeviceCommand CreateCommand(IDevice device, CommandData context)
    {
        if (device is not DoorLocks doorLock)
        {
            throw new InvalidOperationException($"{device.DeviceName} is not a door lock and does not support door lock commands.");
        }

        return CreateToggleLockCommand(doorLock);
    }

    private IDeviceCommand CreateToggleLockCommand(DoorLocks doorLock)
    {
        if (doorLock is not ILatchedDevice latchedDevice)
        {
            throw new InvalidOperationException("This device does not support latch control.");
        }

        return new ToggleLockCommand(doorLock, latchedDevice);
    }


}


