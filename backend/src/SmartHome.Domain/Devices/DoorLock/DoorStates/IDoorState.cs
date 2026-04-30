namespace SmartHome.Domain.Devices.DoorLock.DoorStates;

/// <summary>
/// The IDoorState interface defines the contract for the different states of a door lock device.
/// </summary> <summary>
public interface IDoorState
{
    void ToggleLock();
}