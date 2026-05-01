namespace SmartHome.Domain.Devices.Thermostat;

public class ThermostatStrategyFactory : IThermostatModeStrategyFactory
{
    public IThermostatModeStrategy Create(ThermostatMode mode)
    {
        return mode switch
        {
            ThermostatMode.Heat => new HeatModeStrategy(),
            ThermostatMode.Cool => new CoolModeStrategy(),
            ThermostatMode.Auto => new AutoModeStrategy(),
            _ => throw new ArgumentException("Unsupported thermostat mode.")
        };
    }
}