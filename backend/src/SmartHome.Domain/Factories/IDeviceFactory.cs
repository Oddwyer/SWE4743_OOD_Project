using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Factories;

public interface IDeviceFactory
{
    public IDevice CreateDevice(string name, string location, string type);
}