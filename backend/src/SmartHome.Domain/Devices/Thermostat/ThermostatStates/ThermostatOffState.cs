namespace SmartHome.Domain.Devices.Thermostat.ThermostatStates;

public class ThermostatOffState : IThermostatState
{
    private readonly IDevice thermostat;

    public ThermostatOffState(IDevice thermostat)
    {
        this.thermostat = thermostat;
    }
    public void TogglePower()
    {

    }
    public void SetTargetTemperature(ThermostatDevice thermostat, double temp)
    {

    }
    public void Evaluate(ThermostatDevice thermostat, double ambientTemperature)
    {

    }

}