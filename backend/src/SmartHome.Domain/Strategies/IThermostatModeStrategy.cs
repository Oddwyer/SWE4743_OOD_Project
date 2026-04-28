namespace SmartHome.Domain.Strategies;

public interface IThermostatModeStrategy
{
    bool startHeating(double ambientTemperature, double desiredTemperature);
    bool startCooling(double ambientTemperature, double desiredTemperature);
}