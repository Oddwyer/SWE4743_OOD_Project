namespace SmartHome.Domain.Devices;

public interface ILatchedDevice
{
    DeviceLatchState LatchState { get; }
    void ToggleLock();
}

