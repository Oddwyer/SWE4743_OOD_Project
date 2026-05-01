using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Fan;

namespace SmartHome.Domain.Commands.Fan;

/// <summary>
/// Sets the speed of a fan device.
/// </summary>
public class SetFanSpeedCommand : DeviceCommand
{
    public FanSpeed NewSpeed { get; }
    public override string CommandDescription => $"Set fan speed to {NewSpeed} for {ManipulatedDevice.DeviceName}.";

    public SetFanSpeedCommand(IDevice device, FanSpeed newSpeed) : base(device)
    {
        NewSpeed = newSpeed;
    }

    /// <summary>
    /// Executes the command to change the fan speed. 
    /// </summary>
    public override void Execute()
    {
        if (ManipulatedDevice is not FanDevice fanDevice)
        {
            throw new InvalidOperationException("This device does not have a speed setting.");
        }

        fanDevice.SetFanSpeed(NewSpeed);

    }

}