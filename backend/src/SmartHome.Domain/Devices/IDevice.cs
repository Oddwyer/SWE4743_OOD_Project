namespace SmartHome.Domain.Devices;

public interface IDevice
{

   public Guid Id { get; }
   public string DeviceName { get; }
   public string DeviceLocation { get; }
   public DeviceType Type { get; }

   //TODO - Kataali: Create DeviceState class?
   //public DeviceState State {get;}
   public bool IsDeviceOn { get; }

   //TODO - Kataali: Implement
   //public DeviceSnapshot dehydrate(); // we'll need this for persistence, reference section 2 in project doc
}