using SmartHome.Domain.Commands.History;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Locations;

namespace SmartHome.Domain.Tests;

internal static class TestHelper
{
    public static DeviceFactory CreateDeviceFactory()
    {
        return new DeviceFactory(new IDeviceTypeFactory[]
        {
            new LightDeviceFactory(),
            new FanDeviceFactory(),
            new ThermostatDeviceFactory(new ThermostatStrategyFactory()),
            new DoorLockFactory()
        });
    }
}

internal class FakeDeviceRepository : IDeviceRepository, ILocationRepository
{
    private readonly Dictionary<Guid, IDevice> _devices = new();
    private readonly List<CommandHistoryEntry> _history = new();
    private readonly Dictionary<string, int> _ambientTemperatures = new();

    public IEnumerable<IDevice> FindAllDevices(DeviceFilter filter)
    {
        return _devices.Values.Where(device =>
            (filter.Type == null || device.Type == filter.Type) &&
            (filter.Location == null || device.DeviceLocation == filter.Location) &&
            (filter.IsOn == null || device.IsDeviceOn == filter.IsOn.Value));
    }

    public IDevice? FindDeviceById(Guid deviceId)
    {
        return _devices.TryGetValue(deviceId, out var device) ? device : null;
    }

    public IDevice SaveDevice(IDevice device)
    {
        _devices[device.Id] = device;
        return device;
    }

    public void DeleteDevice(Guid deviceId)
    {
        _devices.Remove(deviceId);
    }

    public bool ThermostatInLocation(string location)
    {
        return _devices.Values.OfType<ThermostatDevice>().Any(device => device.DeviceLocation == location);
    }

    public IEnumerable<CommandHistoryEntry> GetHistoryForDevice(Guid deviceId)
    {
        return _history.Where(entry => entry.DeviceId == deviceId);
    }

    public void SaveHistoryEntry(CommandHistoryEntry entry)
    {
        _history.Add(entry);
    }

    public int? GetAmbientTemperature(string location)
    {
        return _ambientTemperatures.TryGetValue(location, out var temperature) ? temperature : 70;
    }

    public void SaveAmbientTemperature(string location, int temperature)
    {
        _ambientTemperatures[location] = temperature;
    }
}
