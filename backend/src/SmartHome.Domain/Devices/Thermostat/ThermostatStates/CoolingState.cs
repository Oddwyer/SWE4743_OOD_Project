namespace SmartHome.Domain.Devices.Thermostat.ThermostatStates;

/// <summary>
/// Represents the thermostat cooling state.
/// </summary>

public class CoolingState : IThermostatState
{
    private readonly ThermostatDevice _thermostat;

    public CoolingState(ThermostatDevice thermostat)
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