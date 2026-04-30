namespace SmartHome.Domain.Devices.Thermostat.ThermostatStates;

public interface IThermostatState
{
    void TogglePower();
    void SetTargetTemperature(int targetTemperature);
    void Evaluate(int ambientTemperature);

}