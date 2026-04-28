namespace SmartHome.Domain.Devices;

/// <summary>
/// Interface for DeviceFactory for DIP principle and DI.
/// </summary>

public interface IDeviceFactory
{
    // For frontend device creation.
    public IDevice CreateDevice(string name, string location, string type);

    // For backend device rehydration (reference section 2 in project doc).
    public IDevice RehydrateDevice(DeviceSnapshot snapshot);
}