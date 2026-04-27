namespace SmartHome.Domain.Devices;
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