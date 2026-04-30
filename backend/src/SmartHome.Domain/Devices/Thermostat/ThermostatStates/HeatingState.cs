namespace SmartHome.Domain.Devices.Thermostat.ThermostatStates;

/// <summary>
/// The ThermostatHeatingState class represents the state of the thermostat when it is actively heating.
/// </summary>
public class HeatingState : IThermostatState
{
    private readonly ThermostatDevice _thermostat;

    public HeatingState(ThermostatDevice thermostat)
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
        if (temp < ThermostatDevice.MinTemperature || temp > ThermostatDevice.MaxTemperature)
        {
            _thermostat.UpdateStatusMessage($"Target temperature must be between {ThermostatDevice.MinTemperature} and {ThermostatDevice.MaxTemperature} degrees.");
            return;
        }
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