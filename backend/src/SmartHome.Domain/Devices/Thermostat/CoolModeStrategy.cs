namespace SmartHome.Domain.Devices.Thermostat;

public class CoolModeStrategy : IThermostatModeStrategy
{
    public bool StartHeating(double ambientTemperature, double desiredTemperature)
    {
        // we cannot start heating in cool mode
        return false;
    }

    public bool StartCooling(double ambientTemperature, double desiredTemperature)
    {
        // if the ambient temperature is greater than the desired temperature, we can start cooling
        return ambientTemperature > desiredTemperature;
    }
}