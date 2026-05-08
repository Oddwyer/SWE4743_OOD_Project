namespace SmartHome.Domain.Devices.Thermostat.ThermostatStates;

/// <summary>
/// Represents a state in the thermostat state machine (Off, Idle, Heating, Cooling).
/// </summary>
public interface IThermostatState
{
    /// <summary>Transitions the thermostat to the appropriate on/off state.</summary>
    void TogglePower();
    /// <summary>Updates the target temperature within the current state.</summary>
    void SetTargetTemperature(int targetTemperature);
    /// <summary>Evaluates the ambient temperature and transitions state if heating or cooling is required.</summary>
    void Evaluate(int ambientTemperature);
}
