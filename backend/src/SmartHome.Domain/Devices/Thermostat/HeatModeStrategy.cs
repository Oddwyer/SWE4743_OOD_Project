using SmartHome.Domain.Devices.Thermostat.ThermostatStates;

namespace SmartHome.Domain.Devices.Thermostat;

/// <summary>
/// In Heat mode, the thermostat will only heat the room if the ambient temperature is below the target temperature. 
/// It will not cool the room if the ambient temperature is above the target temperature.
/// </summary>
public class HeatModeStrategy : IThermostatModeStrategy
{

    /// <summary>
    /// When determining the next state in Heat mode, we want to check if the ambient temperature is below the target temperature
    /// (in which case we should heat) or equal to or above the target temperature (in which case we should be idle).
    /// </summary>
    public IThermostatState DetermineNextState(ThermostatDevice thermostat, int ambientTemperature)
    {

        return ambientTemperature < thermostat.TargetTemperature
            ? thermostat.Heating
            : thermostat.Idle;
    }
}