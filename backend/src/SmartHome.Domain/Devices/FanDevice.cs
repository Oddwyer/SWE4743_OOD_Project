namespace SmartHome.Domain.Devices;

public class FanDevice : Device, IPoweredDevice
{
    public DevicePowerState powerState { get; private set; }

    public override bool IsDeviceOn => powerState == DevicePowerState.Off;

    public FanDevice(Guid id, string name, string location) : base(id, name, location, DeviceType.Fan)
    {
        powerState = DevicePowerState.Off;
    }

    public void TogglePower()
    {
        powerState = powerState == DevicePowerState.On ? DevicePowerState.Off : DevicePowerState.On;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum FanSpeed
{
    Low,
    Medium, //default speed when fan is turned on
    High
}