using SmartHome.Domain.Devices.Thermostat.ThermostatStates;

namespace SmartHome.Domain.Devices.Thermostat;

public class HeatModeStrategy : IThermostatModeStrategy
{

    public IThermostatState DetermineNextState(ThermostatDevice thermostat, int ambientTemperature)
    {
        return ambientTemperature < thermostat.TargetTemperature
            ? thermostat.HeatingState
            : thermostat.IdleState;
    }
}