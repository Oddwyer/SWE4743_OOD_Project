using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands;

public interface IDeviceCommand
{
    Guid Id { get; }
    IDevice ManipulatedDevice { get; }
    string CommandDescription { get; }
    void Execute();
}
