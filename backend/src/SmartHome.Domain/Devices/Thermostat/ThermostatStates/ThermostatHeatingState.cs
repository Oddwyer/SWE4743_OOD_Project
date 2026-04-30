namespace SmartHome.Domain.Devices.Thermostat.ThermostatStates;

public class ThermostatHeatingState : IThermostatState
{
    private readonly IDevice thermostat;

    public ThermostatHeatingState(IDevice thermostat)
    {
        this.thermostat = thermostat;
    }

    public void TogglePower()
    {

    }
    public void SetTargetTemperature(int temp)
    {

    }
    public void Evaluate(int ambientTemperature)
    {

    }

}