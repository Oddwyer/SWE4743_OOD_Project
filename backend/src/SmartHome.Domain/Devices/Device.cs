using System.Reflection.Metadata.Ecma335;
using SmartHome.Domain.Commands;

namespace SmartHome.Domain.Devices;

/// <summary>
/// Base class for all devices in the smart home system. This class defines 
/// common properties and methods that all devices must implement.
/// </summary>
public abstract class Device : IDevice
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public string DeviceName { get; protected set; } = string.Empty;
    public string DeviceLocation { get; protected set; } = string.Empty;
    public DeviceType Type { get; protected set; }
    public string StatusMessage { get; protected set; } = string.Empty; // Added StatusMessage to Device for better error handling and state reporting.

    //TODO - Kataali: Create DeviceState class...
    //public abstract DeviceState State {get; protected set;} 
    public abstract bool IsDeviceOn { get; }

    //useful for logging and auditing, also mentioned within section 2
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    protected Device(Guid id, string name, string location, DeviceType type)
    {
        Id = id;
        DeviceName = name;
        DeviceLocation = location;
        Type = type;
    }

    //TODO - Kataali: I moved this into JsonDeviceRepository. How does that work for you?
    // public DeviceSnapshot dehydrate(return DeviceSnapshot;);

    //thought to keep this in because of how the api signatures look
    //although command is higher in abstraction than the device models these would serve as reference 
    //when the command pattern needs to run on specific devices
    //wanted to create a shared template for how devices handle command execution
    //each device will specify how this is done

    /* TODO - Kataali
        public void runCommands(DeviceCommand command){
            validateDeviceCommand(command); // validation should be universal across devices, help keep things DRY
            runDeviceCommand(command);
            recordCommandHistory(command);
        }

        protected abstract void runDeviceCommand(DeviceCommand command); // these can be specified within devices
        private void recordCommandHistory(DeviceCommand command);
    }*/
}
