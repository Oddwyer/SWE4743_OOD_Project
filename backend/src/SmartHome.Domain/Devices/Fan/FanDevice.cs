namespace SmartHome.Domain.Devices.Fan;

public class FanDevice : Device, IPoweredDevice
{
    public DevicePowerState PowerState { get; private set; } = DevicePowerState.Off;

    public FanSpeed Speed { get; private set; } = FanSpeed.Medium;

    public override bool IsDeviceOn => PowerState == DevicePowerState.On;

    public FanDevice(Guid id, string name, string location) : base(id, name, location, DeviceType.Fan)
    {
        PowerState = DevicePowerState.Off;
    }

    public void TogglePower()
    {
        PowerState = PowerState == DevicePowerState.On ? DevicePowerState.Off : DevicePowerState.On;

        if (PowerState == DevicePowerState.On && Speed == default)
        {
            Speed = FanSpeed.Medium;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void SetSpeed(FanSpeed newSpeed)
    {
        if (PowerState == DevicePowerState.Off)
        {
            throw new InvalidOperationException("Cannot set speed when the fan is off.");
        }

        Speed = newSpeed;
        UpdatedAt = DateTime.UtcNow;
    }
}

