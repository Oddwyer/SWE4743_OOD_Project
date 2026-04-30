using SmartHome.Domain.Devices.Thermostat.ThermostatStates;

namespace SmartHome.Domain.Devices.Thermostat;

/// <summary>
/// The IThermostatModeStrategy interface defines the contract for different thermostat mode strategies (e.g., Auto, Cool, Heat).
/// Each mode strategy will implement the logic to determine the next state of the thermostat based on the current conditions 
/// (e.g., ambient temperature, target temperature)
/// </summary>
public interface IThermostatModeStrategy
{
    public IThermostatState DetermineNextState(ThermostatDevice thermostat, int ambientTemperature);
}