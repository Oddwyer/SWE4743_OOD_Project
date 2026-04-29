using SmartHome.Domain.Devices;

namespace SmartHome.Domain.Commands.Power;

/// <summary>
/// Command to turn on a device in the smart home system. It checks if the device can be powered on.
/// </summary>

public class TurnOnCommand : DeviceCommand
{
    public override string CommandDescription => "Turn device ON";
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

        if (ManipulatedDevice.IsDeviceOn)
        {
            throw new InvalidOperationException("Device is already ON.");
        }

        poweredDevice.TogglePower();

    }

    /// <summary>
    /// Provides a description of the turn on command, including the name of the device being turned on.
    /// </summary>
    public string Description => $"Turn on {ManipulatedDevice.DeviceName}";
}