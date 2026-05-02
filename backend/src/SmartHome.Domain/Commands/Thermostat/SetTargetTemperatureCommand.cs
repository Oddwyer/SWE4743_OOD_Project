using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Thermostat;

namespace SmartHome.Domain.Commands;

/// <summary>
/// Sets the target (desired) temperature for the thermostat.
/// </summary>
public class SetTargetTemperatureCommand : DeviceCommand
{
    public int TargetTemperature { get; }

    private readonly ThermostatDevice _thermostatDevice;

    public override string CommandDescription => $"Setting {TargetTemperature} degrees for {_thermostatDevice.DeviceName}.";

    public SetTargetTemperatureCommand(ThermostatDevice device, int targetTemperature) : base(device)
    {
        TargetTemperature = targetTemperature;
        _thermostatDevice = device;
    }

    /// <summary>
    /// Executes the command to set the target (desired) temperature for the thermostat.
    /// </summary>
    public override void Execute()
    {

        _thermostatDevice.SetTargetTemperature(TargetTemperature);

    }
}
