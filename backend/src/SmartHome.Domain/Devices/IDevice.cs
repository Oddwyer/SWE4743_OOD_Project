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

   //TODO - Kataali: I moved this into the repository. That ok?
   //public DeviceSnapshot dehydrate();
}
