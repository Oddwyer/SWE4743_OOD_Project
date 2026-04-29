namespace SmartHome.Domain.Devices.Thermostat.ThermostatStates;

public class ThermostatIdleState : IThermostatState
{
    private readonly IDevice thermostat;

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