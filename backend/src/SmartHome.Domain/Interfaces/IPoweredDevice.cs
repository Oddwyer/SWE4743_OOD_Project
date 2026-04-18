namespace SmartHome.Domain.Interfaces;
public interface IPoweredDevice
{
    DevicePowerState powerState {get;}
    void TogglePower();
}

public enum DevicePowerState
{
    On,
    Off
}