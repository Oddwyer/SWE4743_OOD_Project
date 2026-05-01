using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Thermostat;

namespace SmartHome.Domain.Commands.Thermostat;

/// <summary>
/// Sets the thermostat to the mode requested by the client.
/// </summary>
public class SetThermostateModeCommand : DeviceCommand
{
    public ThermostatMode Mode { get; }

    private readonly IThermostatModeStrategyFactory _strategyfactory;
    public override string CommandDescription => $"Setting mode for {ManipulatedDevice.DeviceName}.";

    public SetThermostateModeCommand(IDevice device, ThermostatMode mode, IThermostatModeStrategyFactory factory) : base(device)
    {
        Mode = mode;
        _strategyfactory = factory;

    }

    /// <summary>
    /// Executes the command to update the current mode (heat, auto, cool) of the thermostat.
    /// </summary>
    public override void Execute()
    {
        if (ManipulatedDevice is not ThermostatDevice thermostat)
        {
            throw new InvalidOperationException($"Device '{ManipulatedDevice.DeviceName}' is not a thermostat.");
        }

        var strategy = _strategyfactory.Create(Mode);

        thermostat.SetMode(Mode, strategy);

    }
}