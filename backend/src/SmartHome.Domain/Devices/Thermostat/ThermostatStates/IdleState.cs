namespace SmartHome.Domain.Devices.Thermostat.ThermostatStates;

/// <summary>
/// The ThermostatIdleState class represents the state of the thermostat when it is idle (i.e., not actively heating or cooling).
/// </summary>
public class IdleState : IThermostatState
{
    private readonly ThermostatDevice _thermostat;

    public IdleState(ThermostatDevice thermostat)
    {
        _thermostat = thermostat;
    }

    /// <summary>
    /// Turns off the thermostat.
    /// </summary>
    public void TogglePower()
    {
        _thermostat.TurnPowerOff();
        _thermostat.SetState(_thermostat.Off);
    }

    /// <summary>
    /// Applies a new target temperature.
    /// </summary>
    public void SetTargetTemperature(int temp)
    {
        _thermostat.SetTargetTemperatureInternal(temp);
    }

    /// <summary>
    /// Evaluates and transitions to the next state.
    /// </summary>
    public void Evaluate(int ambientTemperature)
    {
        var nextState = _thermostat.DetermineNextState(ambientTemperature);
        _thermostat.SetState(nextState);
    }

}