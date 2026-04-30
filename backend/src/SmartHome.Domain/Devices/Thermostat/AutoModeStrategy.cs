using SmartHome.Domain.Devices.Thermostat.ThermostatStates;

namespace SmartHome.Domain.Devices.Thermostat;

/// <summary>
/// The AutoModeStrategy class implements the IThermostatModeStrategy interface and defines the logic for determining the next state 
/// of the thermostat when it is in Auto mode.
/// </summary>
public class AutoModeStrategy : IThermostatModeStrategy
{
    /// <summary>
    /// Determines the next state of the thermostat based on the current ambient temperature and the target temperature.
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