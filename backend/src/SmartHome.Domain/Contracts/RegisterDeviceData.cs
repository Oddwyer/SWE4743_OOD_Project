using SmartHome.Domain.Devices;


namespace SmartHome.Domain.Contracts;

public class RegisterDeviceData
{

    public required string DeviceName { get; init; }
    public required string DeviceLocation { get; init; }
    public DeviceType DeviceType { get; init; }
}
