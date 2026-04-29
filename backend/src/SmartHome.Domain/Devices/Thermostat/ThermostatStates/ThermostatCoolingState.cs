namespace SmartHome.Domain.Devices.Thermostat.ThermostatStates;

public class ThermostatCoolingState : IThermostatState
{
    private readonly IDevice thermostat;

    public ThermostatCoolingState(IDevice thermostat)
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