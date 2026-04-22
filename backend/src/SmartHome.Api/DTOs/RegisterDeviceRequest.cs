namespace SmartHome.Api.DTOs;

using SmartHome.Domain;

/// <summary>
/// DTO for registering new devices into the smart home registry
/// </summary>
public class RegisterDeviceRequest
{
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceLocation { get; set; } = string.Empty;

    //TODO: Build DeviceType class 
    //public DeviceType Type {get; set;}
}