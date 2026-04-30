namespace SmartHome.Domain.Devices.Thermostat.ThermostatStates;

public class ThermostatHeatingState : IThermostatState
{
    private readonly ThermostatDevice _thermostat;

    public ThermostatHeatingState(ThermostatDevice thermostat)
    {
        _thermostat = thermostat;
    }

    public void TogglePower()
    {
        _thermostat.TurnPowerOff();
        _thermostat.SetState(_thermostat.OffState);

    }
    public void SetTargetTemperature(int temp)
    {
        _thermostat.SetTargetTemperatureInternal(temp);

    }
    public void Evaluate(int ambientTemperature)
    {
        var nextState = _thermostat.DetermineNextState(ambientTemperature);
        _thermostat.SetState(nextState);

    }

}