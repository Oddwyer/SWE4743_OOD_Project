using SmartHome.Domain.Devices.Thermostat.ThermostatStates;

namespace SmartHome.Domain.Devices.Thermostat;

public class AutoModeStrategy  //: IThermostatModeStrategy
{
    /* public IThermostatState DetermineState(
         double ambientTemperature,
         double desiredTemperature)
     {
         if (ambientTemperature < desiredTemperature)
             return new ThermostatHeatingState();

         if (ambientTemperature > desiredTemperature)
             return new ThermostatCoolingState();

         return new ThermostatIdleState();
     }*/
}