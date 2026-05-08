using SmartHome.Domain.Contracts;

namespace SmartHome.Domain.Devices;

/// <summary>
/// Type-specific factory for creating and rehydrating a single device category.
/// </summary>
public interface IDeviceTypeFactory
{
    /// <summary>Device category this factory handles.</summary>
    public DeviceType DeviceType { get; }

    /// <summary>Creates a new device with the given name and location.</summary>
    public IDevice CreateDevice(string name, string location);

    /// <summary>Reconstructs a device from persisted snapshot data.</summary>
    public IDevice RehydrateDevice(DeviceRehydrationData data);
}
