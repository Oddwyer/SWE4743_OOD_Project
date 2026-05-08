using SmartHome.Domain.Devices;
using SmartHome.Domain.Contracts;


namespace SmartHome.Domain.Commands;

/// <summary>
/// Creates command objects from API requests for device operations.
/// Encapsulates actions and provides descriptions for audit logging (Command Pattern).
/// </summary>
public class CommandFactory : IDeviceCommandFactory
{
    /// <summary>Delegates command creation to the device's own factory via the Command Pattern.</summary>
    public IDeviceCommand CreateCommand(IDevice device, CommandData context)
    {
        return device.CreateCommand(device, context);
    }

}