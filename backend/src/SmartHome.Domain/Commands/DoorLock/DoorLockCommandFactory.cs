using SmartHome.Domain.Devices;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices.DoorLock;

namespace SmartHome.Domain.Commands.DoorLock;

public class DoorLockCommandFactory : IDeviceCommandFactory
{
    public IDeviceCommand CreateCommand(IDevice device, CommandData context)
    {

        if (device is not DoorLocks doorLock)
        {
            throw new InvalidOperationException("This device does not have a lock setting.");
        }
        return new ToggleLockCommand(doorLock);
    }

}


