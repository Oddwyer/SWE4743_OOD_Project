namespace SmartHome.Domain.Devices.Thermostat.ThermostatStates;

public interface IThermostatState
{
    void TurnOn(ThermostatDevice thermostat);
    void TurnOff(ThermostatDevice thermostat);
    void SetTargetTemperature(ThermostatDevice thermostat, double temp);
    void Evaluate(ThermostatDevice thermostat, double ambientTemperature);

}