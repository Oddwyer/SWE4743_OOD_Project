using SmartHome.Domain.Devices.Thermostat.ThermostatStates;

namespace SmartHome.Domain.Devices.Thermostat;

/// <summary>
/// The CoolModeStrategy class implements the IThermostatModeStrategy interface and defines the logic for determining the next state 
/// of the thermostat when it is in Cool mode.
/// </summary>
public class CoolModeStrategy : IThermostatModeStrategy
{

    /// <summary>
    /// Determines the next state of the thermostat based on the current ambient temperature and the target temperature.
    /// </summary>
    public IThermostatState DetermineNextState(ThermostatDevice thermostat, int ambientTemperature)
    {
        return ambientTemperature > thermostat.TargetTemperature
            ? thermostat.Cooling
            : thermostat.Idle;
    }
}