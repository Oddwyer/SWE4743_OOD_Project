namespace SmartHome.Domain.Devices.Fan.FanStates;

/// <summary>
/// The IFanState interface defines the contract for different states of a fan device. Each state
/// implements the behavior for toggling power and setting fan speed, allowing for state-specific logic 
/// to be encapsulated within each state class.
/// </summary>

public interface IFanState
{
    void TogglePower();
    void SetFanSpeed(FanSpeed speed);

}