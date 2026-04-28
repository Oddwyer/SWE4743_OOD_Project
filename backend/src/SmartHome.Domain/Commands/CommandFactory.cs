using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands;

/// <summary>
/// Creates command objects from API requests for device operations.
/// Encapsulates actions and provides descriptions for audit logging (Command Pattern).
/// </summary>

// TODO - Kataali: This factory belongs to the Domain layer because it creates domain objects.
// I added this temporarily so API endpoints could be tested.
public class CommandFactory : ICommandFactory
{

    public IDeviceCommand CreateCommand(string command, IDevice device)
    {
        // TODO - Kataali: Replace placeholder with concrete commands.
        return new StubDeviceCommand(device);
    }
}