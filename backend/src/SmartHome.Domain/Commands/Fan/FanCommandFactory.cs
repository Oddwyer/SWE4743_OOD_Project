using SmartHome.Domain.Devices;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Commands.Powered;

namespace SmartHome.Domain.Commands.Fan;

/// <summary>
/// Creates command objects for fan devices (toggle power, set fan speed).
/// </summary>
public class FanCommandFactory : IDeviceCommandFactory
{
    /// <summary>Creates the appropriate fan command from the request context.</summary>
    public IDeviceCommand CreateCommand(IDevice device, CommandData context)
    {
        if (device is not FanDevice fan)
        {
            throw new InvalidOperationException($"{device.DeviceName} is not a fan and does not support fan commands.");
        }

        return context.Command switch
        {
            DeviceCommandType.TogglePower => CreateTogglePowerCommand(fan),

            DeviceCommandType.SetFanSpeed => CreateSetFanSpeedCommand(fan, context),
            _ => throw new ArgumentException($"Unsupported command type.")
        };

    }

    /// <summary>Creates a TogglePower command for a fan device.</summary>
    private IDeviceCommand CreateTogglePowerCommand(FanDevice fan)
    {

        if (fan is not IPoweredDevice poweredDevice)
        {
            throw new InvalidOperationException("This device does not support power control.");
        }
        return new TogglePowerCommand(fan, poweredDevice);
    }

    /// <summary>Creates a SetFanSpeed command, validating the speed value is present.</summary>
    private IDeviceCommand CreateSetFanSpeedCommand(FanDevice fan, CommandData context)
    {

        if (context.FanSpeed is null)
        {
            throw new ArgumentException("Fan speed is required to alter fan speed.");
        }
        return new SetFanSpeedCommand(fan, context.FanSpeed.Value);
    }

}


