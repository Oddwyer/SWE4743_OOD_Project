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
    /// When toggling power from the off state, we want to turn on the thermostat and set the state to IdleState 
    /// (or the appropriate state based on the current mode and conditions).
    /// </summary>
    public void TogglePower()
    {
        _thermostat.TurnPowerOn();
        _thermostat.SetState(_thermostat.Idle);

    }

    /// <summary>
    /// When setting the target temperature from the off state, we want to ignore the request since the thermostat is off and 
    /// should not be able to change its target temperature until it is turned back on.
    /// </summary>
    public void SetTargetTemperature(int temp)
    {

    }

    /// <summary>
    /// When evaluating the off state, we want to ignore the request since the thermostat is off and should not be able to change 
    /// its state based on ambient temperature until it is turned back on.
    /// </summary>
    public void Evaluate(int ambientTemperature)
    {

    }

}