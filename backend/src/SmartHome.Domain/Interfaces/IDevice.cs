    Guid id {get;}
    string deviceName {get; protected set;} = string.Empty;
    string deviceLocation {get; protected set;} = string.Empty;
    DeviceType device {get; protected set;}
    DeviceState state {get; protected set;}
    bool isDeviceOn {get;}
    DeviceSnapshot dehydrate(); // we'll need this for persistence, reference section 2 in project doc