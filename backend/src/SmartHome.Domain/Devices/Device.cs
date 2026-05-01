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


    /* TODO - Kataali: Can we remove this now?
        public void runCommands(DeviceCommand command){
            validateDeviceCommand(command); // validation should be universal across devices, help keep things DRY
            runDeviceCommand(command);
            recordCommandHistory(command);
        }

        protected abstract void runDeviceCommand(DeviceCommand command); // these can be specified within devices
        private void recordCommandHistory(DeviceCommand command);
    }*/
}
