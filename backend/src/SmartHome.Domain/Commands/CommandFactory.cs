using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Commands.Powered;
using SmartHome.Domain.Contracts;


namespace SmartHome.Domain.Commands;

/// <summary>
/// Creates command objects from API requests for device operations.
/// Encapsulates actions and provides descriptions for audit logging (Command Pattern).
/// </summary>

public class CommandFactory : IDeviceCommandFactory
{
    public IDeviceCommand CreateCommand(IDevice device, CommandData context)
    {
        return device.CreateCommand(device, context);
    }

}