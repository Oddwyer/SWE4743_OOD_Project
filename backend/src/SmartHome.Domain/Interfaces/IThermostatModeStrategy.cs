using SmartHome.Domain.Interfaces;

namespace SmartHome.Domain;

public interface IThermostatModeStrategy
{
    bool startHeating(double ambientTemperature, double desiredTemperature);
    bool startCooling(double ambientTemperature, double desiredTemperature);
}