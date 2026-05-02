/*
using SmartHome.Domain.Devices;
using SmartHome.Domain.Strategies;
namespace SmartHome.Domain;

public class Thermostat : IDevice, IPoweredDevice
{
    public Guid Id { get; set; }
    public string DeviceName { get; set; }
    public string DeviceLocation { get; set; }
    public DeviceType Type => DeviceType.Thermostat;
    public bool IsDeviceOn => _powerState == DevicePowerState.On;
    public double CurrentTemperature { get; set; }
    public double DesiredTemperature { get; set; }
    public ThermostatMode Mode { get; set; }
    private readonly IThermostatModeStrategy _modeStrategy;
    public DevicePowerState _powerState;


    public Thermostat(Guid id, string deviceName, string deviceLocation, double currentTemperature, IThermostatModeStrategy modeStrategy)
    {
        Id = id;
        DeviceName = deviceName;
        DeviceLocation = deviceLocation;
        CurrentTemperature = currentTemperature;
        DesiredTemperature = currentTemperature; // default to current temperature
       // Mode = ThermostatMode.Off; // default mode
        _modeStrategy = modeStrategy;
        _powerState = DevicePowerState.Off; // default power state
    }

    public DevicePowerState powerState => _powerState;

    public void SetDesiredTemperature(double temperature)
    {
        DesiredTemperature = temperature;
        UpdateMode();
    }

    public void TogglePower()
    {
        _powerState = _powerState == DevicePowerState.On // check current state and toggle
        ? DevicePowerState.Off  // if on, turn off
        : DevicePowerState.On;  // if off, turn on
    }
}

public enum ThermostatMode
{
    Heating,
    Cooling
}

*/

/*
private void UpdateMode()
    {
        if (CurrentTemperature < DesiredTemperature)
        {
            Mode = ThermostatMode.Heating;
            _modeStrategy.startHeating(CurrentTemperature, DesiredTemperature);
        }
        else if (CurrentTemperature > DesiredTemperature)
        {
            Mode = ThermostatMode.Cooling;
            _modeStrategy.startCooling(CurrentTemperature, DesiredTemperature);
        }
        else
        {
            Mode = ThermostatMode.Off;
        }
    }
*/