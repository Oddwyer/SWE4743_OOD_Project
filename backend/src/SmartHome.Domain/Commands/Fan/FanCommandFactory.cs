using SmartHome.Domain.Devices;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices.Fan;

namespace SmartHome.Domain.Commands.Fan;

public class FanCommandFactory : IDeviceCommandFactory
{
    public IDeviceCommand CreateCommand(IDevice device, CommandData context)
    {

        if (device is not FanDevice fanDevice)
        {
            throw new InvalidOperationException("This device does not have a speed setting.");
        }
        if (context.FanSpeed is null)
        {
            throw new ArgumentException("Fan speed is required to alter fan speed.");
        }
        return new SetFanSpeedCommand(fanDevice, context.FanSpeed.Value);
    }

}


