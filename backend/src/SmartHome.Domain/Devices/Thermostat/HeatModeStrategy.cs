using SmartHome.Domain.Devices.Thermostat.ThermostatStates;

namespace SmartHome.Domain.Devices.Thermostat;

/// <summary>
/// In Heat mode, the thermostat will only heat the room if the ambient temperature is below the target temperature. 
/// It will not cool the room if the ambient temperature is above the target temperature.
/// </summary>
public class HeatModeStrategy : IThermostatModeStrategy
{

    /// <summary>
    /// Determines the next state of the thermostat based on the current ambient temperature and target temperature.    
    /// </summary>
    public IThermostatState DetermineNextState(ThermostatDevice thermostat, int ambientTemperature)
    {

        return ambientTemperature < thermostat.TargetTemperature
            ? thermostat.Heating
            : thermostat.Idle;
    }
}