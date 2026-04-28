using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands;

/// <summary>
/// Stubbed DeviceCommand class to allow API to continue to build. :)
/// </summary>
public class StubDeviceCommand : IDeviceCommand
{
    public Guid Id { get; }
    public string CommandDescription { get; } = "Stub command executed";
    public IDevice ManipulatedDevice { get; }

    public StubDeviceCommand(IDevice device)
    {
        ManipulatedDevice = device;
        Id = device.Id;
    }

    /// <summary>
    /// Executes command.
    /// </summary>
    public void Execute()
    {
        // TEMP: no real behavior yet
    }
}