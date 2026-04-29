namespace SmartHome.Domain.Devices.Thermostat.ThermostatStates;

public interface IThermostatState
{
    void TogglePower();
    void SetTargetTemperature(ThermostatDevice thermostat, double temp);
    void Evaluate(ThermostatDevice thermostat, double ambientTemperature);

}