namespace SmartHome.Domain.Devices.Thermostat;

/// <summary>
/// Operating modes that control how a thermostat reacts to ambient temperature.
/// </summary>
public enum ThermostatMode
{
    /// <summary>Thermostat actively heats the room when ambient temperature falls below target.</summary>
    Heat = 1,
    /// <summary>Thermostat actively cools the room when ambient temperature rises above target.</summary>
    Cool = 2,
    /// <summary>Thermostat automatically heats or cools based on ambient temperature deviation from target.</summary>
    Auto = 3
}
