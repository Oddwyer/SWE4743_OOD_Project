namespace SmartHome.Domain.Devices;

public interface ILatchedDevice
{
    DeviceLatchState latchState { get; }
    void ToggleLatch();
}

public enum DeviceLatchState
{
    Locked,
    Unlocked
}