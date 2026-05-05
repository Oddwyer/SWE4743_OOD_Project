using SmartHome.Domain.Devices;
using SmartHome.Domain.Contracts;

namespace SmartHome.Domain.Commands.Power;

public class PowerCommandFactory : IDeviceCommandFactory
{
    public IDeviceCommand CreateCommand(IDevice device, CommandData context)
    {

        if (device is not IPoweredDevice poweredDevice)
        {
            throw new InvalidOperationException("This device does not support power control.");
        }
        return new TogglePowerCommand(device, poweredDevice);
    }

}


