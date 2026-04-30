namespace SmartHome.Domain.Devices.Thermostat.ThermostatStates;

/// <summary>
/// The ThermostatCoolingState class represents the state of the thermostat when it is actively cooling.
/// </summary>

public class CoolingState : IThermostatState
{

    private readonly ThermostatDevice _thermostat;

    public CoolingState(ThermostatDevice thermostat)
    {
        _thermostat = thermostat;
    }

    /// <summary>
    /// When toggling power from the cooling state, we want to turn off the thermostat and set the state to OffState.
    /// </summary>
    public void TogglePower()
    {
        _thermostat.TurnPowerOff();
        _thermostat.SetState(_thermostat.Off);

    }

    /// <summary>
    /// When setting the target temperature from the cooling state, we want to update the target temperature and then evaluate 
    /// the next state based on the new target temperature and the current ambient temperature.
    /// </summary>
    public void SetTargetTemperature(int temp)
    {
        _thermostat.SetTargetTemperatureInternal(temp);

    }

    /// <summary>
    /// When evaluating the cooling state, we want to determine the next state based on the current ambient temperature and the target temperature.
    /// </summary>

    public void Evaluate(int ambientTemperature)
    {
        var nextState = _thermostat.DetermineNextState(ambientTemperature);
        _thermostat.SetState(nextState);
    }

}