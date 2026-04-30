namespace SmartHome.Domain.Devices.Thermostat.ThermostatStates;

/// <summary>
/// The ThermostatIdleState class represents the state of the thermostat when it is idle (i.e., not actively heating or cooling).
/// </summary>
public class IdleState : IThermostatState
{
    private readonly IDevice thermostat;

    public IdleState(IDevice thermostat)
    {
        this.thermostat = thermostat;
    }

    /// <summary>
    /// When toggling power from the idle state, we want to turn on the thermostat and set the state to HeatingState or CoolingState 
    /// based on the current ambient temperature and the target temperature.
    /// </summary>
    public void TogglePower()
    {

    }

    /// <summary>
    /// When setting the target temperature from the idle state, we want to update the target temperature and then evaluate
    /// the next state based on the new target temperature and the current ambient temperature.
    /// </summary>
    public void SetTargetTemperature(int temp)
    {

    }

    /// <summary>
    /// When evaluating the idle state, we want to determine the next state based on the current ambient temperature and the target temperature.
    /// </summary>
    public void Evaluate(int ambientTemperature)
    {

    }

}