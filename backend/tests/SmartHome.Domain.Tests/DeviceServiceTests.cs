using System;
using System.Collections.Generic;
using System.Linq;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Commands.History;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Commands.Lock;
using Xunit;

namespace SmartHome.Domain.Tests;

public class DeviceServiceTests
{
    private class FakeDeviceRepository : IDeviceRepository
    {
        private readonly Dictionary<Guid, IDevice> _devices = new();
        private readonly List<CommandHistoryEntry> _history = new();

        public IEnumerable<IDevice> FindAllDevices(DeviceFilter filter)
        {
            return _devices.Values.Where(device =>
                (filter.Type == null || device.Type == filter.Type) &&
                (filter.Location == null || device.DeviceLocation == filter.Location) &&
                (filter.IsOn == null || device.IsDeviceOn == filter.IsOn.Value));
        }

        public IDevice? FindDeviceById(Guid deviceId)
        {
            return _devices.TryGetValue(deviceId, out var device) ? device : null;
        }

        public IDevice SaveDevice(IDevice device)
        {
            _devices[device.Id] = device;
            return device;
        }

        public void DeleteDevice(Guid deviceId)
        {
            _devices.Remove(deviceId);
        }

        public bool ThermostatInLocation(string location)
        {
            return _devices.Values.OfType<ThermostatDevice>().Any(device => device.DeviceLocation == location);
        }

        public IEnumerable<CommandHistoryEntry> GetHistoryForDevice(Guid deviceId)
        {
            return _history.Where(entry => entry.DeviceId == deviceId);
        }

        public void SaveHistoryEntry(CommandHistoryEntry entry)
        {
            _history.Add(entry);
        }
    }

    private class FakeDeviceFactory : IDeviceFactory
    {
        public IDevice CreateDevice(string name, string location, DeviceType type)
        {
            return type switch
            {
                DeviceType.DoorLock => new DoorLocks(Guid.NewGuid(), name, location),
                _ => throw new ArgumentException("Unsupported device type for fake factory.")
            };
        }

        public IDevice RehydrateDevice(DeviceRehydrationData data)
        {
            throw new NotImplementedException();
        }
    }

    private class FakeCommandFactory : ICommandFactory
    {
        public IDeviceCommand CreateCommand(IDevice device, CommandData context)
        {
            return context.Command switch
            {
                DeviceCommandType.ToggleLock => new ToggleLockCommand(device),
                _ => throw new ArgumentException("Unsupported command type for fake command factory.")
            };
        }
    }

    private static DeviceService CreateDeviceService(FakeDeviceRepository repository)
    {
        return new DeviceService(repository, new FakeDeviceFactory(), new FakeCommandFactory());
    }

    [Fact]
    public void GetDeviceById_ReturnsDevice_WhenFound()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);
        var device = new DoorLocks(Guid.NewGuid(), "FrontDoor", "Entrance");

        repository.SaveDevice(device);

        var result = service.GetDeviceById(device.Id);

        Assert.Same(device, result);
    }

    [Fact]
    public void GetDeviceById_ThrowsKeyNotFound_WhenNotFound()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);

        Assert.Throws<KeyNotFoundException>(() => service.GetDeviceById(Guid.NewGuid()));
    }

    [Fact]
    public void RegisterDevice_SavesNewDevice()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);
        var register = new RegisterDeviceData
        {
            DeviceName = "BackDoor",
            DeviceLocation = "Hallway",
            DeviceType = DeviceType.DoorLock
        };

        var device = service.RegisterDevice(register);

        var persisted = repository.FindDeviceById(device.Id);
        Assert.Same(device, persisted);
    }

    [Fact]
    public void RegisterDevice_ThrowsWhenThermostatAlreadyExistsInLocation()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);
        var first = new ThermostatDevice(Guid.NewGuid(), "MainThermostat", "LivingRoom", ThermostatMode.Auto, null!);
        repository.SaveDevice(first);

        var register = new RegisterDeviceData
        {
            DeviceName = "SecondThermostat",
            DeviceLocation = "LivingRoom",
            DeviceType = DeviceType.Thermostat
        };

        Assert.Throws<InvalidOperationException>(() => service.RegisterDevice(register));
    }

    [Fact]
    public void ApplyDeviceCommand_SavesDeviceAndHistory()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);
        var device = new DoorLocks(Guid.NewGuid(), "GarageDoor", "Garage");
        repository.SaveDevice(device);

        var context = new CommandData
        {
            Command = DeviceCommandType.ToggleLock
        };

        var updated = service.ApplyDeviceCommand(device.Id, context);

        Assert.Same(device, updated);
        Assert.Equal(DeviceLatchState.Unlocked, device.LatchState);
        var history = repository.GetHistoryForDevice(device.Id).ToList();
        Assert.Single(history);
        Assert.Equal($"Locked {device.DeviceName}.", history[0].Operation);
    }

    [Fact]
    public void ApplyDeviceCommand_ThrowsKeyNotFound_WhenDeviceMissing()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);

        var context = new CommandData
        {
            Command = DeviceCommandType.ToggleLock
        };

        Assert.Throws<KeyNotFoundException>(() => service.ApplyDeviceCommand(Guid.NewGuid(), context));
    }

    [Fact]
    public void RemoveDevice_DeletesDevice_WhenFound()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);
        var device = new DoorLocks(Guid.NewGuid(), "PatioDoor", "Patio");
        repository.SaveDevice(device);

        service.RemoveDevice(device.Id);

        Assert.Null(repository.FindDeviceById(device.Id));
    }

    [Fact]
    public void GetCommandHistory_ReturnsHistory_WhenDeviceExists()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);
        var device = new DoorLocks(Guid.NewGuid(), "FrontDoor", "Entrance");
        repository.SaveDevice(device);
        var entry = new CommandHistoryEntry(device.Id, new ToggleLockCommand(device));
        repository.SaveHistoryEntry(entry);

        var history = service.GetCommandHistory(device.Id).ToList();

        Assert.Single(history);
        Assert.Equal(entry.Id, history[0].Id);
    }

    [Fact]
    public void GetCommandHistory_ThrowsKeyNotFound_WhenDeviceMissing()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);

        Assert.Throws<KeyNotFoundException>(() => service.GetCommandHistory(Guid.NewGuid()));
    }
}
