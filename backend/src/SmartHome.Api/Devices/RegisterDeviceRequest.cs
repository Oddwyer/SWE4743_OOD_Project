using System.ComponentModel.DataAnnotations;

namespace SmartHome.Api.Devices;

/// <summary>
/// DTO for registering new devices into the smart home registry. 
/// </summary>

public class RegisterDeviceRequest : BaseDevice
{
    // All devices. Includes DeviceName, DeviceLocation, and Type from BaseDevice
    // Must provide device name, location, and type.
}