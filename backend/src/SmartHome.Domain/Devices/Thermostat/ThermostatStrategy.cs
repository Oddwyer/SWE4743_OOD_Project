namespace SmartHome.Domain.Devices.Thermostat;

public class HeatModeStrategy : IThermostatModeStrategy
{
    public bool startHeating(double ambientTemperature, double desiredTemperature)
    {
        // if the ambient temperature is less than the desired temperature, we can start heating
        return ambientTemperature < desiredTemperature;
    }

    public bool startCooling(double ambientTemperature, double desiredTemperature)
    {
        // we cannot start cooling in heat mode
        return false;
    }
}

public class CoolModeStrategy : IThermostatModeStrategy
{
    public bool startHeating(double ambientTemperature, double desiredTemperature)
    {
        // we cannot start heating in cool mode
        return false;
    }

    public bool startCooling(double ambientTemperature, double desiredTemperature)
    {
        // if the ambient temperature is greater than the desired temperature, we can start cooling
        return ambientTemperature > desiredTemperature;
    }
}

public class AutoModeStrategy : IThermostatModeStrategy
{
    public bool startHeating(double ambientTemperature, double desiredTemperature)
    {
        // in auto mode, we can start heating if the ambient temperature is less than the desired temperature
        return ambientTemperature < desiredTemperature;
    }

    public bool startCooling(double ambientTemperature, double desiredTemperature)
    {
        // in auto mode, we can start cooling if the ambient temperature is greater than the desired temperature
        return ambientTemperature > desiredTemperature;
    }
}