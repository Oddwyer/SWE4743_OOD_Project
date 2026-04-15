// basic template of what all devices will need in the home simulator
public abstract class Device
{
    public Guid id {get; set;} = Guid.NewGuid();
    public string deviceName {get; set;} = string.Empty;
    public string deviceLocation {get; set;} = string.Empty;
    public DeviceType device {get; set;}
    public Dateime createdAt {get; set;} = Dateime.UtcNow;
    public Dateime updatedAt {get; set;} = Dateime.UtcNow;

    public abstract DeviceState GetState();
    public abstract void ExecuteDeviceCommand(DeviceCommand command);
    public abstract bool ableToExecuteCommand(DeviceCommand command);
}

public enum DeviceType
{
    Light,
    Fan,
    DoorLock,
    Thermostat
}