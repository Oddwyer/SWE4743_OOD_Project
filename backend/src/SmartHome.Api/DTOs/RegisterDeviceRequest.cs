namespace SmartHome.Api.DTOs;

using SmartHome.Domain;

/// <summary>
/// DTO for registering new devices into the smart home registry. Must provide device name, location, and type.
/// </summary>
public class RegisterDeviceRequest
{
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceLocation { get; set; } = string.Empty;
    public string Type {get; set;} = string.Empty;
}