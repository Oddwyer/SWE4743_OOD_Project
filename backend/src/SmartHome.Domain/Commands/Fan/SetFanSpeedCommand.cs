using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Fan;

namespace SmartHome.Domain.Commands.Fan;

public class SetFanSpeedCommand : DeviceCommand
{
    public FanSpeed NewSpeed { get; }
    public override string CommandDescription => $"Set fan speed to {NewSpeed} for {ManipulatedDevice.DeviceName}.";

    public SetFanSpeedCommand(IDevice device, FanSpeed newSpeed) : base(device)
    {
        NewSpeed = newSpeed;
    }

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

        fanDevice.SetSpeed(NewSpeed);

    }

    public string Description => $"Speed set to {NewSpeed} for {ManipulatedDevice.DeviceName}";
}