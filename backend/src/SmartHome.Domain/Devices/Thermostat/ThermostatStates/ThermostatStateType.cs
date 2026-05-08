namespace SmartHome.Domain.Devices.Thermostat.ThermostatStates;

/// <summary>
/// Runtime states a thermostat can occupy during simulation.
/// </summary>
public enum ThermostatStateType
{
    /// <summary>Thermostat is powered off.</summary>
    Off,
    /// <summary>Thermostat is powered on (legacy/transitional — use Idle, Heating, or Cooling).</summary>
    On,
    /// <summary>Thermostat is on but not actively heating or cooling.</summary>
    Idle,
    /// <summary>Thermostat is actively heating the room.</summary>
    Heating,
    /// <summary>Thermostat is actively cooling the room.</summary>
    Cooling
}
