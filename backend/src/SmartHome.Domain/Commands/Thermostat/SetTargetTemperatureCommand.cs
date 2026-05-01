using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Thermostat;

namespace SmartHome.Domain.Commands.Thermostat;

/// <summary>
/// Sets the target (desired) temperature for the thermostat.
/// </summary>
public class SetTargetTemperatureCommand : DeviceCommand
{
    public int TargetTemperature { get; }

    public override string CommandDescription => $"Setting mode for {ManipulatedDevice.DeviceName}.";

    public SetTargetTemperatureCommand(IDevice device, int targetTemperature) : base(device)
    {
        TargetTemperature = targetTemperature;
    }

    /// <summary>
    /// Executes the command to set the target (desired) temperature for the thermostat.
    /// </summary>
    public override void Execute()
    {
        if (ManipulatedDevice is not ThermostatDevice thermostat)
        {
            throw new InvalidOperationException($"Device '{ManipulatedDevice.DeviceName}' is not a thermostat.");
        }

        thermostat.SetTargetTemperature(TargetTemperature);

    }
}