using SmartHome.Domain.Commands;
using SmartHome.Domain.Contracts;

namespace SmartHome.Domain.Devices;

public interface IDevice
{
   public Guid Id { get; }
   public string DeviceName { get; }
   public string DeviceLocation { get; }
   public DeviceType Type { get; }
   public bool IsDeviceOn { get; }

   public DateTime CreatedAt { get; }
   public DateTime UpdatedAt { get; }

   public IDeviceCommand CreateCommand(IDevice device, CommandData data);

   public void ResetToDefault();

}
