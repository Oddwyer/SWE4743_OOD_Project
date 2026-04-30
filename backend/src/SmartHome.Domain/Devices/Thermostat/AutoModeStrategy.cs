using SmartHome.Domain.Devices.Thermostat.ThermostatStates;

namespace SmartHome.Domain.Devices.Thermostat;

/// <summary>
/// In Auto mode, the thermostat will automatically switch between heating and cooling based on the ambient temperature and the target temperature.
/// </summary>
public class AutoModeStrategy : IThermostatModeStrategy
{
    /// <summary>
    /// When determining the next state in Auto mode, we want to check if the ambient temperature is below the target temperature 
    /// (in which case we should heat), above the target temperature (in which case we should cool), or equal to the target temperature 
    /// (in which case we should be idle).
    /// </summary>
    public IThermostatState DetermineNextState(ThermostatDevice thermostat, int ambientTemperature)
    {
        return ambientTemperature < thermostat.TargetTemperature
            ? thermostat.Heating
            : ambientTemperature > thermostat.TargetTemperature
                ? thermostat.Cooling
                : thermostat.Idle;
    }

}