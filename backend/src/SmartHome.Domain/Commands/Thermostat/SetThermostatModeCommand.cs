using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Thermostat;

namespace SmartHome.Domain.Devices.Thermostat;

/// <summary>
/// Sets the thermostat to the mode requested by the client.
/// </summary>
public class SetThermostatModeCommand : DeviceCommand
{
    public ThermostatMode Mode { get; }
    private readonly IThermostatModeStrategy _strategy;

    private readonly ThermostatDevice thermostat;

    public SetThermostatModeCommand(ThermostatDevice device, ThermostatMode mode, IThermostatModeStrategy strategy) : base(device)
    {
        Mode = mode;
        _strategy = strategy;
        thermostat = device;

    }

    /// <summary>
    /// Executes the command to update the current mode (heat, auto, cool) of the thermostat.
    /// </summary>
    public override void Execute()
    {

        thermostat.SetMode(Mode, _strategy);
        _commandDescription = $"Set mode to {Mode} for {ManipulatedDevice.DeviceName}.";

    }
}
