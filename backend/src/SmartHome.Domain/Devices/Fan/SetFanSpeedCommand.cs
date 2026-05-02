using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Fan;

namespace SmartHome.Domain.Devices.Fan;

/// <summary>
/// Sets the speed of a fan device.
/// </summary>
public class SetFanSpeedCommand : DeviceCommand
{
    public FanSpeed NewSpeed { get; }

    private readonly FanDevice _fanDevice;
    public override string CommandDescription => $"Set fan speed to {NewSpeed} for {_fanDevice.DeviceName}.";

    public SetFanSpeedCommand(FanDevice device, FanSpeed newSpeed) : base(device)
    {
        NewSpeed = newSpeed;
        _fanDevice = device;
    }

    /// <summary>
    /// Executes the command to change the fan speed. 
    /// </summary>
    public override void Execute()
    {

        _fanDevice.SetFanSpeed(NewSpeed);

    }

}
