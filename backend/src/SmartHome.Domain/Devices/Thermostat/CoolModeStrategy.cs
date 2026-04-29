namespace SmartHome.Domain.Devices.Thermostat;

public class CoolModeStrategy //: IThermostatModeStrategy
{
    /*public IThermostatState DetermineState(ThermostatDevice thermostat, double ambientTemperature)
    {
        if (!thermostat.IsDeviceOn)
            return new ThermostatOffState();

        if (ambientTemperature < thermostat.TargetTemperature - 0.5)
            return new ThermostatHeatingState();
        else if (ambientTemperature > thermostat.TargetTemperature + 0.5)
            return new ThermostatCoolingState();
        else
            return new ThermostatIdleState();
    }*/
}