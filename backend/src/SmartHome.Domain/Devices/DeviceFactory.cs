using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.DoorLock;

/// <summary>
/// Creates and rehydrates device instances based on type or persisted data.
/// Centralizes device creation and hides concrete implementations (Factory Pattern).
/// </summary>

namespace SmartHome.Domain.Devices;

// TODO Kataali: This factory belongs to the Domain layer because it creates domain objects.
// I added this so API endpoints could be tested but we still need thermostat. You're welcome to flush it out!

public class DeviceFactory : IDeviceFactory
{

    /// <summary>
    /// Creates new specific device based on type entered.
    /// </summary>
    public IDevice CreateDevice(string name, string location, DeviceType type)
    {
        Guid id = Guid.NewGuid();

        switch (type)
        {
            case DeviceType.Light:
                return new LightDevice(id, name, location);

            case DeviceType.Fan:
                return new FanDevice(id, name, location);

            // TODO - Kataali (or I can do it): Add Thermostat creation and rehydration once thermostat constructor/state 
            // fields are finalized.

            /*case DeviceType.Thermostat:
                return new Thermostat(name, location);*/

            case DeviceType.DoorLock:
                return new DoorLocks(id, name, location);

            default:
                throw new ArgumentException("Unsupported device type.");
        }
    }

    /// <summary>
    /// Rehydrates saved data into device objects.
    /// </summary>
    public IDevice RehydrateDevice(Guid id, string name, string location, DeviceType type, bool isOn, string? deviceState)
    {
        switch (type)
        {
            case DeviceType.Light:
                return new LightDevice(id, name ?? "", location ?? "");

            case DeviceType.Fan:
                return new FanDevice(id, name ?? "", location ?? "");

            case DeviceType.DoorLock:
                return new DoorLocks(id, name ?? "", location ?? "");

            default:
                throw new ArgumentException("Unsupported device type.");
        }
    }
}

