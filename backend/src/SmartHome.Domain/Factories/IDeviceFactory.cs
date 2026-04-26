using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Factories;

/// <summary>
/// Interface for DeviceFactory for DIP principle and DI.
/// </summary>

public interface IDeviceFactory
{
    public IDevice CreateDevice(string name, string location, string type);
}