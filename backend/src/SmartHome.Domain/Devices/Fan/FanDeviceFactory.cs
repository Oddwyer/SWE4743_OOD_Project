using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices;

/// <summary>
/// Creates and restores fan device instances.
/// </summary>

namespace SmartHome.Domain.Devices.Fan;

public class FanDeviceFactory : IDeviceTypeFactory
{
    public DeviceType DeviceType => DeviceType.Fan;

    public FanDeviceFactory()
    {

    }

    /// <summary>
    /// Creates a fan device instances.
    /// </summary>
    public IDevice CreateDevice(string name, string location, DeviceType type)
    {
        Guid id = Guid.NewGuid();

        return new FanDevice(id, name, location);

    }

    /// <summary>
    /// Restores a fan from persisted values.
    /// </summary>
    public IDevice RehydrateDevice(DeviceRehydrationData data)
    {

        var fan = new FanDevice(data.Id, data.Name, data.Location);

        var powerState = data.IsOn ? DevicePowerState.On : DevicePowerState.Off;
        var fanSpeed = data.FanSpeed ?? FanSpeed.Medium;

        fan.RehydrateState(powerState, fanSpeed);

        return fan;


    }

}

