using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands;

/// <summary>
/// Interface for CommandFactory for DIP principle and DI.
/// </summary>
public interface ICommandFactory
{
    public IDeviceCommand CreateCommand(string command, IDevice device);

}