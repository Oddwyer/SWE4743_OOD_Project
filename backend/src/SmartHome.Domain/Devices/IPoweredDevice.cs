namespace SmartHome.Domain.Devices;

public interface IPoweredDevice
{
    DevicePowerState PowerState { get; }
    void TogglePower();
}

