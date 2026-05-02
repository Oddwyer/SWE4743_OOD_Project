using SmartHome.Domain.Devices;
using SmartHome.Domain.Contracts;

namespace SmartHome.Domain.Commands;

/// <summary>
/// Defines a contract for creating command objects from incoming requests to perform device operations.
/// </summary>
public interface ICommandFactory
{
    public IDeviceCommand CreateCommand(IDevice device, CommandData context);

}
