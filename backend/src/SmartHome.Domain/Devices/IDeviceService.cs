using SmartHome.Domain.Commands.History;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Contracts;

namespace SmartHome.Domain.Devices;

/// <summary>
/// Defines application-level device operations, coordinating persistence, simulation, and command execution.
/// </summary>
public interface IDeviceService
{
    /// <summary>Returns all devices that match the provided filter criteria.</summary>
    IEnumerable<IDevice> GetAllDevices(DeviceFilter filter);

    /// <summary>Returns command history for the specified device, newest first.</summary>
    IEnumerable<CommandHistoryEntry> GetCommandHistory(Guid deviceId);

    /// <summary>Returns the device with the given ID, or throws <see cref="KeyNotFoundException"/> if not found.</summary>
    IDevice GetDeviceById(Guid deviceId);

    /// <summary>Registers a new device and enrolls thermostats into the simulation.</summary>
    IDevice RegisterDevice(RegisterDeviceData data);

    /// <summary>Removes the device with the given ID; unregisters thermostats from the simulation.</summary>
    void RemoveDevice(Guid deviceId);

    /// <summary>Applies the given command to the specified device and persists the result.</summary>
    IDevice ApplyDeviceCommand(Guid deviceId, CommandData context);
}
