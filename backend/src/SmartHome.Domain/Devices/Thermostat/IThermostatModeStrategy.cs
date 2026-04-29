namespace SmartHome.Domain.Devices.Thermostat;

public interface IThermostatModeStrategy
{
    bool StartHeating(double ambientTemperature, double desiredTemperature);
    bool StartCooling(double ambientTemperature, double desiredTemperature);
}