using SmartHome.Domain.Contracts;

namespace SmartHome.Domain.Devices;

/// <summary>
/// Defines a contract for creating and rehydrating device instances without exposing concrete device types.
/// </summary>
public interface IDeviceFactory
{
    /// <summary>Creates a new device of the specified type with the given name and location.</summary>
    public IDevice CreateDevice(string name, string location, DeviceType type);

    /// <summary>Reconstructs a device from persisted snapshot data.</summary>
    public IDevice RehydrateDevice(DeviceRehydrationData data);
}
