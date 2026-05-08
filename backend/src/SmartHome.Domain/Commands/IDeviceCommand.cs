using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands;

/// <summary>
/// Represents an executable command targeting a specific device (Command Pattern).
/// </summary>
public interface IDeviceCommand
{
    /// <summary>Unique command identifier used for history tracking.</summary>
    Guid Id { get; }

    /// <summary>The device this command operates on.</summary>
    IDevice ManipulatedDevice { get; }

    /// <summary>Human-readable description logged to command history.</summary>
    string CommandDescription { get; }

    /// <summary>Applies the command to the target device.</summary>
    void Execute();
}
