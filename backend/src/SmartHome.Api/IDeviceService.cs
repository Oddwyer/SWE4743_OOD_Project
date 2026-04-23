using SmartHome.Domain.Interfaces;

namespace SmartHome.Api;
public interface IDeviceService
{
    public IReadOnlyList<IDevice> GetDevices();

    //public IDevice GetDeviceById(string deviceID);

    //public void RegisterDevice(string name, string type, string location);

    //public void DeleteDevice(string deviceID);
}

