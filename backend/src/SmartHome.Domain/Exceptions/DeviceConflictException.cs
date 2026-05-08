namespace SmartHome.Domain.Exceptions;

public class DeviceConflictException : Exception
{
    public DeviceConflictException(string message) : base(message)
    {
    }
}