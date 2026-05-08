namespace SmartHome.Domain.Devices.Thermostat;

/// <summary>
/// Resolves the correct <see cref="IThermostatModeStrategy"/> for a given thermostat mode.
/// </summary>
public interface IThermostatModeStrategyFactory
{
    /// <summary>Returns the strategy implementation for the specified thermostat mode.</summary>
    IThermostatModeStrategy Create(ThermostatMode mode);
}
