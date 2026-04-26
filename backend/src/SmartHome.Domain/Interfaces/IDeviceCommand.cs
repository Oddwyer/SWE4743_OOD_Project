using SmartHome.Domain.Interfaces;

namespace SmartHome.Domain;

public interface IDeviceCommand
{
    Guid Id { get; }
    IDevice manipulatedDevice { get; }
    string commandDescription { get; } // needed for logging and auditing purposes
    void Execute();
}