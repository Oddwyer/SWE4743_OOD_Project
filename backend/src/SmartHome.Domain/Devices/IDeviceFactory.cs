namespace SmartHome.Domain.Devices;

/// <summary>
/// Defines a contract for creating and rehydrating device instances without exposing concrete device types.
/// </summary>

public interface IDeviceFactory
{
    // For frontend device creation.
    public IDevice CreateDevice(string name, string location, string type);

    // For backend device rehydration (reference section 2 in project doc).
    public IDevice RehydrateDevice(DeviceSnapshot snapshot);
}