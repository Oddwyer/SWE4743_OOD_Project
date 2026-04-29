using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands.Power;

/// <summary>
/// Command to turn on a device in the smart home system. It checks if the device can be powered on.
/// </summary>

public class TurnOnCommand : DeviceCommand
{
    public override string CommandDescription => $"Turned on {ManipulatedDevice.DeviceName}.";
    public TurnOnCommand(IDevice device) : base(device)
    {

    }

    /// <summary>
    /// Executes the command to turn on the device. It checks if the device can be powered on
    /// and if it is already on before toggling the power state.
    /// </summary>
    public override void Execute()
    {
        if (ManipulatedDevice is not IPoweredDevice poweredDevice)
        {
            throw new InvalidOperationException("This device cannot be powered on.");
        }

        if (poweredDevice.PowerState == DevicePowerState.On)
        {
            throw new InvalidOperationException("Device is already ON.");
        }

        poweredDevice.TogglePower();

    }

}