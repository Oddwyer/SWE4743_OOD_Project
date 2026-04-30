namespace SmartHome.Domain.Devices.Thermostat;

public interface IThermostatModeStrategyFactory
{
    IThermostatModeStrategy Create(ThermostatMode mode);
}