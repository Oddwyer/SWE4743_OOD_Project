using SmartHome.Domain.Devices;
using SmartHome.Domain.Commands;

namespace SmartHome.Domain.Commands;

public class CommandHistoryEntry
{
    // this class is meant to store a single entry in the command history for a single device, 
    // it will store the command that was executed, the timestamp of when it was 
    // executed, and the device it was executed on

    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public IDeviceCommand Command { get; set; }
    public DateTime Timestamp { get; set; }

    public CommandHistoryEntry(Guid deviceId, IDeviceCommand command)
    {
        Id = Guid.NewGuid();
        DeviceId = deviceId;
        Command = command;
        Timestamp = DateTime.UtcNow;
    }
}