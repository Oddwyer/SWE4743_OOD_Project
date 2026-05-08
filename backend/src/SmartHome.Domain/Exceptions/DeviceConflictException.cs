namespace SmartHome.Domain.Exceptions;

/// <summary>
/// Thrown when a device registration conflicts with an existing device (e.g. duplicate thermostat in a location).
/// </summary>
public class DeviceConflictException : Exception
{
    /// <summary>Initializes the exception with a descriptive message.</summary>
    public DeviceConflictException(string message) : base(message) { }
}
