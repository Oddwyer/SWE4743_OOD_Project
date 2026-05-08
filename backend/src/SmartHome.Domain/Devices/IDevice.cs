using SmartHome.Domain.Commands;
using SmartHome.Domain.Contracts;

namespace SmartHome.Domain.Devices;

/// <summary>
/// Represents a smart home device with a stable identity, location, and the ability to accept commands.
/// </summary>
public interface IDevice
{
   /// <summary>Unique device identifier.</summary>
   public Guid Id { get; }
   /// <summary>Human-readable device name.</summary>
   public string DeviceName { get; }
   /// <summary>Location the device is installed in.</summary>
   public string DeviceLocation { get; }
   /// <summary>Device category.</summary>
   public DeviceType Type { get; }
   /// <summary>True when the device is considered active for filtering and display.</summary>
   public bool IsDeviceOn { get; }

   /// <summary>Timestamp of when the device was registered.</summary>
   public DateTime CreatedAt { get; }
   /// <summary>Timestamp of the most recent state change.</summary>
   public DateTime UpdatedAt { get; }

   /// <summary>Creates and returns the appropriate command for the given context.</summary>
   public IDeviceCommand CreateCommand(IDevice device, CommandData data);

   /// <summary>Resets the device to its factory-default state.</summary>
   public void ResetToDefault();
}
