namespace SmartHome.Domain.Devices;

public interface ILatchedDevice
{
    DeviceLatchState latchState { get; }
    void ToggleLock();
}

