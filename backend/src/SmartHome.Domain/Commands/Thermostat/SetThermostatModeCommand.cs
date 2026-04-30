using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Thermostat;

namespace SmartHome.Domain.Commands.Thermostat;

public class SetThermostateModeCommand : DeviceCommand
{
    public override string CommandDescription => $"Setting mode for {ManipulatedDevice.DeviceName}.";

    public SetThermostateModeCommand(IDevice device, ThermostatMode mode) : base(device)
    {


    }

    /// <summary>
    /// Executes the command to toggle the power state of the device. It checks if the device can be toggled
    /// and performs the toggle operation.
    /// </summary>
    public override void Execute()
    {
        if (ManipulatedDevice is not ThermostatDevice thermostat)
        {
            throw new InvalidOperationException($"Device '{ManipulatedDevice.DeviceName}' is not a thermostat.");
        }



    }
}