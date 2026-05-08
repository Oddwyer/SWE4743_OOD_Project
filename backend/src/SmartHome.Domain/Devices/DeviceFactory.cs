using SmartHome.Domain.Contracts;

namespace SmartHome.Domain.Devices;

/// <summary>
/// Factory for creating and rehydrating device instances without exposing concrete device types.
/// </summary>
public class DeviceFactory : IDeviceFactory
{
    private readonly Dictionary<DeviceType, IDeviceTypeFactory> _factories;

    public DeviceFactory(IEnumerable<IDeviceTypeFactory> factories)
    {
        _factories = factories.ToDictionary(factory => factory.DeviceType);
    }

    /// <summary>
    /// Creates and domain device instances.
    /// </summary>
    public IDevice CreateDevice(string name, string location, DeviceType type)
    {

        if (!_factories.TryGetValue(type, out var factory))
        {
            throw new ArgumentException($"Unsupported device type: {type}");
        }

        return factory.CreateDevice(name, location);

    }

    /// <summary>
    /// Restores domain device instances.
    /// </summary>
    public IDevice RehydrateDevice(DeviceRehydrationData data)
    {

        if (!_factories.TryGetValue(data.Type, out var factory))
        {
            throw new ArgumentException($"Unsupported device type: {data.Type}");
        }

        return factory.RehydrateDevice(data);
    }

}