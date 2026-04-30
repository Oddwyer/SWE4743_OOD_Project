using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Fan;

namespace SmartHome.Domain.Commands.Fan;

/// <summary>
/// Command to set the speed of a fan device. This command checks if the device is a fan and
/// if it is currently on before changing its speed. If the device is not a fan or is off, it throws an exception.
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
    /// Executes the command to change the fan speed. It first checks if the manipulated device is a fan and if it is on.
    /// </summary>

    public override void Execute()
    {
        if (ManipulatedDevice is not FanDevice fanDevice)
        {
            throw new InvalidOperationException("This device does not have a speed setting.");
        }

        if (!ManipulatedDevice.IsDeviceOn)
        {
            throw new InvalidOperationException("Fan is not on.");
        }

        fanDevice.SetFanSpeed(NewSpeed);

    }

}