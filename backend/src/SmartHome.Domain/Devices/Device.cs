namespace SmartHome.Domain.Devices;

/// <summary>
/// Base class for all devices in the smart home system. 
/// Defines shared identity, metadata, status, and timestamp behavior.
/// </summary>
public abstract class Device : IDevice
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public string DeviceName { get; protected set; } = string.Empty;
    public string DeviceLocation { get; protected set; } = string.Empty;
    public DeviceType Type { get; protected set; }

    public abstract string StatusMessage { get; protected set; }
    public abstract bool IsDeviceOn { get; }

    //Useful for logging and auditing.
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

    protected Device(Guid id, string name, string location, DeviceType type)
    {
        Id = id;
        DeviceName = name;
        DeviceLocation = location;
        Type = type;
    }

}
