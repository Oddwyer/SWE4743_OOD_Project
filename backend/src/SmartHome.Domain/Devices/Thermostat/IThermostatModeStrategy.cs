using SmartHome.Domain.Devices.Thermostat.ThermostatStates;

namespace SmartHome.Domain.Devices.Thermostat;

public interface IThermostatModeStrategy
{
    public IThermostatState DetermineNextState(ThermostatDevice thermostat, int ambientTemperature);
}