namespace SmartHome.Domain.Devices.Thermostat;

public interface IThermostatModeStrategy
{
    bool startHeating(double ambientTemperature, double desiredTemperature);
    bool startCooling(double ambientTemperature, double desiredTemperature);
}