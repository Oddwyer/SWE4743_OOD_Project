using SmartHome.Domain.Devices;

namespace SmartHome.Domain;

public interface IDeviceRepository
{
    IEnumerable<IDevice> FindAllDevices(DeviceFilter filter);
    IDevice? FindDeviceById(Guid deviceId);
    IDevice SaveDevice(IDevice device);
    void DeleteDevice(Guid deviceId);
    bool ThermostatInLocation(string location);
}