using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices;

namespace SmartHome.Api.Factories;

/// <summary>
/// Interface for CommandFactory for DIP principle and DI.
/// </summary>
public interface ICommandFactory
{
    public IDeviceCommand CreateCommand(IDevice device);

}