using SmartHome.Domain.Devices;
using SmartHome.Domain.Commands.Fan;
using SmartHome.Domain.Commands.Light;
using SmartHome.Domain.Commands.Lock;
using SmartHome.Domain.Commands.Power;


namespace SmartHome.Domain.Commands;

/// <summary>
/// Creates command objects from API requests for device operations.
/// Encapsulates actions and provides descriptions for audit logging (Command Pattern).
/// </summary>

// TODO - Kataali: This factory belongs to the Domain layer because it creates domain objects.
// I added this temporarily so API endpoints could be tested.
public class CommandFactory : ICommandFactory
{

    public IDeviceCommand CreateCommand(CommandContext context, IDevice device)
    {
        // TODO - Replace placeholder with concrete commands.
        //return new StubDeviceCommand(device);

        switch (context.Command)
        {

            case DeviceCommandType.TogglePower:
                return new TogglePowerCommand(device);

            case DeviceCommandType.SetBrightness:
                if (context.Brightness is null)
                {
                    throw new ArgumentException("Brightness is required for setting brightness.");
                }
                return new SetLightBrightnessCommand(device, context.Brightness.Value);


            case DeviceCommandType.SetColor:
                if (context.Color is null)
                {
                    throw new ArgumentException("Color is required for changing light color.");
                }
                return new SetLightColorCommand(device, context.Color.Value);

            case DeviceCommandType.SetFanSpeed:
                if (context.FanSpeed is null)
                {
                    throw new ArgumentException("Fan speed is required to alter fan speed.");
                }
                return new SetFanSpeedCommand(device, context.FanSpeed.Value);

            case DeviceCommandType.SetThermostatMode:

            case DeviceCommandType.SetDesiredTemperature:

            case DeviceCommandType.ToggleLock:
                return new ToggleLockCommand(device);

            default:
                throw new ArgumentException($"Unsupported command type: {command}");



        }
    }
}