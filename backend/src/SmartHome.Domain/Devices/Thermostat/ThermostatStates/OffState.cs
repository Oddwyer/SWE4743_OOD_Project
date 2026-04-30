using System.Security.Cryptography;

namespace SmartHome.Domain.Devices.Thermostat.ThermostatStates;

/// <summary>
/// The ThermostatOffState class represents the state of the thermostat when it is turned off. In this state, the thermostat 
/// should not be actively heating or cooling, and any attempts to set the target temperature or evaluate the state should have 
/// no effect until the power is toggled back on.
/// </summary>
public class OffState : IThermostatState
{
    private readonly ThermostatDevice _thermostat;

    public OffState(ThermostatDevice thermostat)
    {
        _thermostat = thermostat;
    }

    /// <summary>
    /// Turns on the thermostat.
    /// </summary>
    public void TogglePower()
    {
        _thermostat.TurnPowerOn();
        _thermostat.SetState(_thermostat.Idle);

    }

    /// <summary>
    /// When setting the target temperature from the off state, we ignore the request since the thermostat is off.
    /// </summary>
    public void SetTargetTemperature(int temp)
    {
        _thermostat.UpdateStatusMessage("Cannot set target temperature while thermostat is off.");
    }

    /// <summary>
    /// When setting the target temperature from the off state, we ignore the request since the thermostat is off.
    /// </summary>
    public void Evaluate(int ambientTemperature)
    {
        _thermostat.UpdateStatusMessage("Thermostat is off. No state evaluation performed.");
    }

}