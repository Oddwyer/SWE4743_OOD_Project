namespace SmartHome.Domain.Devices.Thermostat;

public class HeatModeStrategy : IThermostatModeStrategy
{
    public bool StartHeating(double ambientTemperature, double desiredTemperature)
    {
        // if the ambient temperature is less than the desired temperature, we can start heating
        return ambientTemperature < desiredTemperature;
    }

    public bool StartCooling(double ambientTemperature, double desiredTemperature)
    {
        // we cannot start cooling in heat mode
        return false;
    }
}