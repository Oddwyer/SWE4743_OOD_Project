namespace SmartHome.Domain.Devices.Thermostat;

/// <summary>
/// Factory for creating thermostat mode strategies based on the selected mode.
/// </summary>
public class ThermostatStrategyFactory : IThermostatModeStrategyFactory
{
    /// <summary>
    /// Creates a thermostat mode strategy based on the provided mode.
    /// </summary>

    public IThermostatModeStrategy Create(ThermostatMode mode)
    {
        return mode switch
        {
            ThermostatMode.Heat => new HeatModeStrategy(),
            ThermostatMode.Cool => new CoolModeStrategy(),
            ThermostatMode.Auto => new AutoModeStrategy(),
            _ => throw new ArgumentException("Unsupported thermostat mode.")
        };
    }
}
