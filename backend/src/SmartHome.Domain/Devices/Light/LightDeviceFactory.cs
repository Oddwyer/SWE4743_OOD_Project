using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices;

/// <summary>
/// Creates and restores light device instances.
/// </summary>

namespace SmartHome.Domain.Devices.Light;

public class LightDeviceFactory : IDeviceTypeFactory
{
    public DeviceType DeviceType => DeviceType.Light;

    public LightDeviceFactory()
    {

    }

    /// <summary>
    /// Creates light device instances.
    /// </summary>
    public IDevice CreateDevice(string name, string location, DeviceType type)
    {
        Guid id = Guid.NewGuid();

        return new LightDevice(id, name, location);

    }

    /// <summary>
    /// Restores a light from persisted values.
    /// </summary>
    public IDevice RehydrateDevice(DeviceRehydrationData data)
    {
        var light = new LightDevice(data.Id, data.Name, data.Location);

        var powerState = data.IsOn ? DevicePowerState.On : DevicePowerState.Off;
        var lightcolor = data.LightColor ?? LightColor.White;
        var lightBrightness = data.LightBrightness ?? 100;

        light.RehydrateState(powerState, lightcolor, lightBrightness);

        return light;
    }

}

