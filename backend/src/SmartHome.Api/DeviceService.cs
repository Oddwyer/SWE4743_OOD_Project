using SmartHome.Domain.Interfaces;

/// TODO: Remove when built.
/// Stub for GetDevices(); until built in backend.

namespace SmartHome.Api;

public class DeviceService: IDeviceService
{
    public IReadOnlyList<IDevice> GetDevices()
    {
        //Temp stub data
        return new List<IDevice>();
    }
}

public interface IDeviceService
{
    IReadOnlyList<IDevice> GetDevices();
}


