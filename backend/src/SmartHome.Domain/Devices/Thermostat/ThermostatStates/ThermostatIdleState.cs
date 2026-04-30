namespace SmartHome.Domain.Devices.Thermostat.ThermostatStates;

public class ThermostatIdleState : IThermostatState
{
    private readonly IDevice thermostat;

    public ThermostatIdleState(IDevice thermostat)
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