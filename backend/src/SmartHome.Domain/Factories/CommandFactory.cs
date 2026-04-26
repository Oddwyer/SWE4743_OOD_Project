using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Factories;

/// <summary>
/// Command Factory for command creation based on user input.
/// </summary>
public class CommandFactory : ICommandFactory
{

    public IDeviceCommand CreateCommand(string command, IDevice device)
    {
        // TODO: Placeholder for concrete commands.
        return new StubDeviceCommand(device);
    }
}