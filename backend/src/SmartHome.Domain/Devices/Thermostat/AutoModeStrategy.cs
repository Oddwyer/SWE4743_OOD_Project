namespace SmartHome.Domain.Devices.Thermostat;

public class AutoModeStrategy : IThermostatModeStrategy
{
    public bool StartHeating(double ambientTemperature, double desiredTemperature)
    {
        // in auto mode, we can start heating if the ambient temperature is less than the desired temperature
        return ambientTemperature < desiredTemperature;
    }

    public bool StartCooling(double ambientTemperature, double desiredTemperature)
    {
        // in auto mode, we can start cooling if the ambient temperature is greater than the desired temperature
        return ambientTemperature > desiredTemperature;
    }
}