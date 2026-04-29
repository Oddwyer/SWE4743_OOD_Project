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

    public IDeviceCommand CreateCommand(DeviceCommandType command, IDevice device)
    {
        // TODO - Replace placeholder with concrete commands.
        //return new StubDeviceCommand(device);

        switch (command)
        {
            case DeviceCommandType.TogglePower:

            case DeviceCommandType.SetBrightness:

            case DeviceCommandType.SetColor:

            case DeviceCommandType.SetFanSpeed:

            case DeviceCommandType.SetThermostatMode:

            case DeviceCommandType.SetDesiredTemperature:

            case DeviceCommandType.ToggleLock:

            default:
                throw new ArgumentException($"Unsupported command type: {command}");



        }
    }
}