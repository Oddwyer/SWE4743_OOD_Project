using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Factories;

/// <summary>
/// Interface for DeviceFactory for DIP principle and DI.
/// </summary>

public interface IDeviceFactory
{
    // For frontend device creation.
    public IDevice CreateDevice(string name, string location, string type);

    // For backend device rehydration.
    public IDevice RehydrateDevice(DeviceSnapshot snapshot);
}