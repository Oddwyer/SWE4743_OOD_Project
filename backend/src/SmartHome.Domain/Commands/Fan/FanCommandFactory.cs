using SmartHome.Domain.Devices;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Commands.Powered;

namespace SmartHome.Domain.Commands.Fan;

public class FanCommandFactory : IDeviceCommandFactory
{
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

    private IDeviceCommand CreateTogglePowerCommand(FanDevice fan)
    {

        if (fan is not IPoweredDevice poweredDevice)
        {
            throw new InvalidOperationException("This device does not support power control.");
        }
        return new TogglePowerCommand(fan, poweredDevice);
    }

    private IDeviceCommand CreateSetFanSpeedCommand(FanDevice fan, CommandData context)
    {

        if (context.FanSpeed is null)
        {
            throw new ArgumentException("Fan speed is required to alter fan speed.");
        }
        return new SetFanSpeedCommand(fan, context.FanSpeed.Value);
    }

}


