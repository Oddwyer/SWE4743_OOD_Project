   namespace SmartHome.Domain.Interfaces;

   public interface IDevice{
   
    public Guid Id {get;}
    public String DeviceName {get;} 
    public String DeviceLocation {get;} 
    public DeviceType Type {get;}
    //TODO: Create DeviceState class
    //public DeviceState State {get;}
    public Boolean IsDeviceOn {get;}

    //TODO: Create DeviceSnapshot class
    //public DeviceSnapshot dehydrate(); // we'll need this for persistence, reference section 2 in project doc
   }