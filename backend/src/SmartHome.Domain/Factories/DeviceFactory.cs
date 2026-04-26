using SmartHome.Domain.Devices;
using System.Collections;
using SmartHome.Domain;

namespace SmartHome.Domain.Factories;

// TODO Kataali:
// This factory belongs to the Domain layer because it creates domain objects.
// I added this so API endpoints could be tested but we still need thermostat. You're welcome to flush it out!

public class DeviceFactory : IDeviceFactory
{
    public DeviceFactory()
    {

    }

    public IDevice CreateDevice(string name, string location, string type)
    {
        Guid id = Guid.NewGuid();

        if (!Enum.TryParse<DeviceType>(type, true, out var deviceType))
        {
            throw new ArgumentException("Invalid device type.");
        }

        switch (deviceType)
        {
            case DeviceType.Light:
                return new LightDevice(id, name, location);

            case DeviceType.Fan:
                return new FanDevice(id, name, location);

            /*case DeviceType.Thermostat:
                return new Thermostat(name, location);*/

            case DeviceType.DoorLock:
                return new DoorLocks(id, name, location);

            default:
                throw new ArgumentException("Unsupported device type.");
        }
    }

    public IDevice RehydrateDevice(DeviceSnapshot snapshot)
    {
        switch (snapshot.Type)
        {
            case DeviceType.Light:
                return new LightDevice(snapshot.Id, snapshot.Name ?? "", snapshot.Location ?? "");

            case DeviceType.Fan:
                return new FanDevice(snapshot.Id, snapshot.Name ?? "", snapshot.Location ?? "");

            case DeviceType.DoorLock:
                return new DoorLocks(snapshot.Id, snapshot.Name ?? "", snapshot.Location ?? "");

            default:
                throw new ArgumentException("Unsupported device type.");
        }
    }
}

