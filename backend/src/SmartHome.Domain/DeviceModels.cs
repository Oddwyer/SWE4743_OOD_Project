// basic template of what all devices will need in the home simulator
public abstract class Device : IDevice
{   //core fields and properties accounted for first
    public Guid Id {get; protected set;} = Guid.NewGuid();
    public string DeviceName {get; protected set;}
    public string DeviceLocation {get; protected set;}
    public DeviceType Type {get; protected set;}

//each device needs to be able to control/manage its own state
    public abstract DeviceState State {get;}
    public abstract bool IsDeviceOn {get;}

//useful for logging and auditing, also mentioned within section 2
    public DateTime CreatedAt {get; protected set;} = DateTime.UtcNow;
    public DateTime UpdatedAt {get; protected set;} = DateTime.UtcNow;

    protected Device (string name, string location, DeviceType type)
    {
        DateTimeeviceName = name;
        DeviceLocation = location;
        Type = type;
    }

//thought to keep this in because of how the api signatures look
//although command is higher in abstraction than the device models these would serve as reference 
//when the command pattern needs to run on specific devices
//wanted to create a shared template for how devices handle command execution
//each device will specify how this is done

/*AO: Understood. :-) I assume we will remove it once we have the command pattern built out. */

    public void runCommands(DeviceCommand command){
        validateDeviceCommand(command); // validation should be universal across devices, help keep things DRY
        runDeviceCommand(command);
        recordCommandHistory(command);
    }

    protected abstract void runDeviceCommand(DeviceCommand command); // these can be specified within devices
    private void recordCommandHistory(DeviceCommand command);
}

public enum DeviceType
{
    Light,
    Fan,
    DoorLock,
    Thermostat
}