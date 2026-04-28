namespace SmartHome.Domain.Devices;

public class DeviceFilter
{
    public DeviceType? Type { get; set; }
    public string? Location { get; set; }
    public bool? IsOn { get; set; }

    // Additional filter criteria can be added as needed
    // for now we'll be filtering by device type eg fan, light, etc, 
    // location and whether the device is on or off
}