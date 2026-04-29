using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands;


public class TurnOnCommand : DeviceCommand
{
    public override string CommandDescription => "Turn device ON";

    public TurnOnCommand(IDevice device) : base(device)
    {

    }

    public override void Execute()
    {
        if (ManipulatedDevice is not IPoweredDevice poweredDevice)
        {
            throw new InvalidOperationException("This device cannot be powered on.");
        }

        if (ManipulatedDevice.IsDeviceOn)
        {
            throw new InvalidOperationException("Device is already ON.");
        }

        poweredDevice.TogglePower();

    }

    public string Description => $"Turn on {ManipulatedDevice.DeviceName}";
}