using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Factories;

// TODO Kataali:
// This factory belongs to the Domain layer because it creates domain objects.
// I added this temporarily so API endpoints could be tested.
public class CommandFactory : ICommandFactory
{

    public IDeviceCommand CreateCommand(string command, IDevice device)
    {
        // TODO: Placeholder for concrete commands.
        return new StubDeviceCommand(device);
    }
}