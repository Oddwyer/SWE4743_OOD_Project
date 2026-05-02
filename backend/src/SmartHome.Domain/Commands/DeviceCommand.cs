using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands;

/// <summary>
/// Represents a command to be executed on a device. Implements the Command Pattern.
/// Encapsulates the action and provides a description for logging and auditing purposes.
/// </summary>
public abstract class DeviceCommand : IDeviceCommand
{
    public Guid Id { get; } = Guid.NewGuid();
    public IDevice ManipulatedDevice { get; }
    protected string _commandDescription = string.Empty;

    public string CommandDescription => _commandDescription;

    protected DeviceCommand(IDevice device)
    {
        ManipulatedDevice = device;
    }

    public abstract void Execute();
}
