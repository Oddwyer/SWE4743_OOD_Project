using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands;

/// <summary>
/// Defines a contract for creating command objects from incoming requests to perform device operations.
/// </summary>
public interface ICommandFactory
{
    /// <summary>
    /// Creates new command.
    /// </summary>
    public IDeviceCommand CreateCommand(string command, IDevice device);

}