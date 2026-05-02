using SmartHome.Domain.Contracts;

namespace SmartHome.Domain.Devices;

/// <summary>
/// Factory interface for creating devices of different types.
/// </summary>
public interface IDeviceTypeFactory
{
    public DeviceType DeviceType { get; }

    // For frontend device creation.
    public IDevice CreateDevice(string name, string location, DeviceType type);

    // For backend device rehydration (reference section 2 in project doc).
    public IDevice RehydrateDevice(DeviceRehydrationData data);
}
