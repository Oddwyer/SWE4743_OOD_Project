using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands;


public class TurnOffCommand : DeviceCommand
{
    public override string CommandDescription => "Turn device OFF";

    public TurnOffCommand(IDevice device) : base(device)
    {

    }

    public override void Execute()
    {
        if (ManipulatedDevice is not IPoweredDevice poweredDevice)
        {
            throw new InvalidOperationException("This device cannot be powered off.");
        }

        if (ManipulatedDevice.IsDeviceOn)
        {
            throw new InvalidOperationException("Device is already OFF.");
        }

        poweredDevice.TogglePower();

    }

    public string Description => $"Turn off {ManipulatedDevice.DeviceName}";
}