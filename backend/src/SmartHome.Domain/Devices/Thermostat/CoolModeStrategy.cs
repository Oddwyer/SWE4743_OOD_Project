using SmartHome.Domain.Devices.Thermostat.ThermostatStates;

namespace SmartHome.Domain.Devices.Thermostat;

/// <summary>
/// In Cool mode, the thermostat will only activate cooling if the ambient temperature is above the target temperature. 
/// It will not activate heating under any circumstances.
/// </summary>
public class CoolModeStrategy : IThermostatModeStrategy
{

    /// <summary>
    /// When determining the next state in Cool mode, we want to check if the ambient temperature is above the target temperature
    /// (in which case we should cool) or equal to or below the target temperature (in which case we should be idle).
    /// </summary>
    public IThermostatState DetermineNextState(ThermostatDevice thermostat, int ambientTemperature)
    {
        return ambientTemperature > thermostat.TargetTemperature
            ? thermostat.Cooling
            : thermostat.Idle;
    }
}