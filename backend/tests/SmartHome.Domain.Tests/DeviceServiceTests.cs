using System;
using System.Collections.Generic;
using System.Linq;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Commands.History;
using SmartHome.Domain.Commands.Latched;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Locations;
using SmartHome.Domain.Simulations;
using Xunit;

namespace SmartHome.Domain.Tests;

public class DeviceServiceTests
{
    private class FakeDeviceRepository : IDeviceRepository, ILocationRepository
    {
        private readonly Dictionary<Guid, IDevice> _devices = new();
        private readonly List<CommandHistoryEntry> _history = new();
        private readonly Dictionary<string, int> _ambientTemperatures = new();

        public int SaveDeviceCallCount { get; private set; }
        public int DeleteDeviceCallCount { get; private set; }

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
            SaveDeviceCallCount++;
            _devices[device.Id] = device;
            return device;
        }

        public void DeleteDevice(Guid deviceId)
        {
            DeleteDeviceCallCount++;
            _devices.Remove(deviceId);
        }

        public bool ThermostatInLocation(string location)
        {
            return _devices.Values
                .OfType<ThermostatDevice>()
                .Any(device => device.DeviceLocation == location);
        }

        public IEnumerable<CommandHistoryEntry> GetHistoryForDevice(Guid deviceId)
        {
            return _history.Where(entry => entry.DeviceId == deviceId);
        }

        public void SaveHistoryEntry(CommandHistoryEntry entry)
        {
            _history.Add(entry);
        }

        public int? GetAmbientTemperature(string location)
        {
            return _ambientTemperatures.TryGetValue(location, out var temperature)
                ? temperature
                : 70;
        }

        public void SaveAmbientTemperature(string location, int temperature)
        {
            _ambientTemperatures[location] = temperature;
        }
    }

    private class FakeDeviceFactory : IDeviceFactory
    {
        public IDevice CreateDevice(string name, string location, DeviceType type)
        {
            return type switch
            {
                DeviceType.DoorLock => new DoorLocks(Guid.NewGuid(), name, location),
                DeviceType.Thermostat => new ThermostatDevice(
                    Guid.NewGuid(),
                    name,
                    location,
                    ThermostatMode.Auto,
                    null!),
                _ => throw new ArgumentException("Unsupported device type for fake factory.")
            };
        }

        public IDevice RehydrateDevice(DeviceRehydrationData data)
        {
            throw new NotImplementedException();
        }
    }

    private class FakeCommandFactory : IDeviceCommandFactory
    {
        public bool ThrowOnCreate { get; set; }

        public IDeviceCommand CreateCommand(IDevice device, CommandData context)
        {
            if (ThrowOnCreate)
            {
                throw new ArgumentException("Command factory failed.");
            }

            if (device is not ILatchedDevice latchedDevice)
            {
                throw new InvalidOperationException("This device does not support latch control.");
            }

            return context.Command switch
            {
                DeviceCommandType.ToggleLock when device is DoorLocks doorLock =>
                    new ToggleLockCommand(doorLock, latchedDevice),

                _ => throw new ArgumentException("Unsupported command type for fake command factory.")
            };
        }
    }

    private class FakeSimulationService : ISimulationService
    {
        private readonly Dictionary<string, int> _ambientTemperatures = new();

        public int RegisterThermostatCallCount { get; private set; }
        public int UnregisterThermostatCallCount { get; private set; }

        public ThermostatDevice? LastRegisteredThermostat { get; private set; }
        public ThermostatDevice? LastUnregisteredThermostat { get; private set; }

        public void SetAmbientTemperature(string location, int temperature)
        {
            _ambientTemperatures[location] = temperature;
        }

        public int GetAmbientTemperature(string location)
        {
            return _ambientTemperatures.TryGetValue(location, out var temperature)
                ? temperature
                : 70;
        }

        public void SetSimulationSpeed(SimulationSpeed speedMultiplier)
        {
        }

        public void ResetSimulation()
        {
        }

        public void RegisterThermostat(ThermostatDevice thermostat)
        {
            RegisterThermostatCallCount++;
            LastRegisteredThermostat = thermostat;
        }

        public void UnregisterThermostat(ThermostatDevice thermostat)
        {
            UnregisterThermostatCallCount++;
            LastUnregisteredThermostat = thermostat;
        }

        public void UpdateAmbientTemperature() { }

        public void StartSimulation() { }
    }

    private static DeviceService CreateDeviceService(
        FakeDeviceRepository repository,
        FakeSimulationService? simulationService = null,
        FakeCommandFactory? commandFactory = null)
    {
        return new DeviceService(
            simulationService ?? new FakeSimulationService(),
            repository,
            new FakeDeviceFactory(),
            commandFactory ?? new FakeCommandFactory(),
            repository);
    }

    [Fact]
    public void GetAllDevices_ReturnsAllDevices_WhenNoFilterProvided()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);

        var frontDoor = new DoorLocks(Guid.NewGuid(), "FrontDoor", "Entrance");
        var garageDoor = new DoorLocks(Guid.NewGuid(), "GarageDoor", "Garage");

        repository.SaveDevice(frontDoor);
        repository.SaveDevice(garageDoor);

        var result = service.GetAllDevices(new DeviceFilter()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(frontDoor, result);
        Assert.Contains(garageDoor, result);
    }

    [Fact]
    public void GetAllDevices_FiltersByLocation()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);

        var frontDoor = new DoorLocks(Guid.NewGuid(), "FrontDoor", "Entrance");
        var garageDoor = new DoorLocks(Guid.NewGuid(), "GarageDoor", "Garage");

        repository.SaveDevice(frontDoor);
        repository.SaveDevice(garageDoor);

        var filter = new DeviceFilter
        {
            Location = "Garage"
        };

        var result = service.GetAllDevices(filter).ToList();

        Assert.Single(result);
        Assert.Same(garageDoor, result[0]);
    }

    [Fact]
    public void GetAllDevices_FiltersByDeviceType()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);

        var doorLock = new DoorLocks(Guid.NewGuid(), "FrontDoor", "Entrance");
        var thermostat = new ThermostatDevice(
            Guid.NewGuid(),
            "MainThermostat",
            "LivingRoom",
            ThermostatMode.Auto,
            null!);

        repository.SaveDevice(doorLock);
        repository.SaveDevice(thermostat);

        var filter = new DeviceFilter
        {
            Type = DeviceType.DoorLock
        };

        var result = service.GetAllDevices(filter).ToList();

        Assert.Single(result);
        Assert.Same(doorLock, result[0]);
    }

    [Fact]
    public void GetAllDevices_FiltersByPowerStatus()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);

        var thermostat = new ThermostatDevice(
            Guid.NewGuid(),
            "MainThermostat",
            "LivingRoom",
            ThermostatMode.Auto,
            null!);

        var doorLock = new DoorLocks(Guid.NewGuid(), "FrontDoor", "Entrance");

        repository.SaveDevice(thermostat);
        repository.SaveDevice(doorLock);

        var filter = new DeviceFilter
        {
            IsOn = false
        };

        var result = service.GetAllDevices(filter).ToList();

        Assert.Contains(thermostat, result);
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
    public void RegisterDevice_CreatesAndSavesNewDevice()
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

        Assert.Equal("BackDoor", device.DeviceName);
        Assert.Equal("Hallway", device.DeviceLocation);
        Assert.Equal(DeviceType.DoorLock, device.Type);
        Assert.Same(device, repository.FindDeviceById(device.Id));
    }

    [Fact]
    public void RegisterDevice_RegistersThermostatWithSimulation_WhenDeviceIsThermostat()
    {
        var repository = new FakeDeviceRepository();
        var simulationService = new FakeSimulationService();
        var service = CreateDeviceService(repository, simulationService);

        var register = new RegisterDeviceData
        {
            DeviceName = "MainThermostat",
            DeviceLocation = "LivingRoom",
            DeviceType = DeviceType.Thermostat
        };

        var device = service.RegisterDevice(register);

        var thermostat = Assert.IsType<ThermostatDevice>(device);
        Assert.Equal(1, simulationService.RegisterThermostatCallCount);
        Assert.Same(thermostat, simulationService.LastRegisteredThermostat);
        Assert.Same(device, repository.FindDeviceById(device.Id));
    }

    [Fact]
    public void RegisterDevice_ThrowsWhenThermostatAlreadyExistsInLocation()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);

        var first = new ThermostatDevice(
            Guid.NewGuid(),
            "MainThermostat",
            "LivingRoom",
            ThermostatMode.Auto,
            null!);

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
    public void ApplyDeviceCommand_ExecutesCommandSavesDeviceAndHistory()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);
        var device = new DoorLocks(Guid.NewGuid(), "GarageDoor", "Garage");

        repository.SaveDevice(device);

        var saveCountBeforeCommand = repository.SaveDeviceCallCount;

        var context = new CommandData
        {
            Command = DeviceCommandType.ToggleLock
        };

        var updated = service.ApplyDeviceCommand(device.Id, context);

        Assert.Same(device, updated);
        Assert.Equal(DeviceLatchState.Unlocked, device.LatchState);
        Assert.Equal(saveCountBeforeCommand + 1, repository.SaveDeviceCallCount);

        var history = repository.GetHistoryForDevice(device.Id).ToList();

        Assert.Single(history);
        Assert.Equal(device.Id, history[0].DeviceId);
        Assert.Equal($"Unlocked {device.DeviceName}.", history[0].Operation);
        Assert.NotEqual(Guid.Empty, history[0].Id);
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
    public void ApplyDeviceCommand_DoesNotSaveDeviceOrHistory_WhenCommandFactoryThrows()
    {
        var repository = new FakeDeviceRepository();
        var commandFactory = new FakeCommandFactory
        {
            ThrowOnCreate = true
        };

        var service = CreateDeviceService(repository, commandFactory: commandFactory);
        var device = new DoorLocks(Guid.NewGuid(), "GarageDoor", "Garage");

        repository.SaveDevice(device);

        var saveCountBeforeCommand = repository.SaveDeviceCallCount;

        var context = new CommandData
        {
            Command = DeviceCommandType.ToggleLock
        };

        Assert.Throws<ArgumentException>(() => service.ApplyDeviceCommand(device.Id, context));

        Assert.Equal(DeviceLatchState.Locked, device.LatchState);
        Assert.Equal(saveCountBeforeCommand, repository.SaveDeviceCallCount);
        Assert.Empty(repository.GetHistoryForDevice(device.Id));
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
        Assert.Equal(1, repository.DeleteDeviceCallCount);
    }

    [Fact]
    public void RemoveDevice_ThrowsKeyNotFound_WhenDeviceMissing()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);

        Assert.Throws<KeyNotFoundException>(() => service.RemoveDevice(Guid.NewGuid()));
        Assert.Equal(0, repository.DeleteDeviceCallCount);
    }

    [Fact]
    public void RemoveDevice_UnregistersThermostat_WhenDeviceIsThermostat()
    {
        var repository = new FakeDeviceRepository();
        var simulationService = new FakeSimulationService();
        var service = CreateDeviceService(repository, simulationService);

        var thermostat = new ThermostatDevice(
            Guid.NewGuid(),
            "MainThermostat",
            "LivingRoom",
            ThermostatMode.Auto,
            null!);

        repository.SaveDevice(thermostat);

        service.RemoveDevice(thermostat.Id);

        Assert.Null(repository.FindDeviceById(thermostat.Id));
        Assert.Equal(1, simulationService.UnregisterThermostatCallCount);
        Assert.Same(thermostat, simulationService.LastUnregisteredThermostat);
    }

    [Fact]
    public void GetCommandHistory_ReturnsHistory_WhenDeviceExists()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);
        var device = new DoorLocks(Guid.NewGuid(), "FrontDoor", "Entrance");

        repository.SaveDevice(device);

        var entry = new CommandHistoryEntry(
            device.Id,
            new ToggleLockCommand(device, device));

        repository.SaveHistoryEntry(entry);

        var history = service.GetCommandHistory(device.Id).ToList();

        Assert.Single(history);
        Assert.Equal(entry.Id, history[0].Id);
        Assert.Equal(device.Id, history[0].DeviceId);
    }

    [Fact]
    public void GetCommandHistory_ReturnsOnlyHistoryForRequestedDevice()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);

        var frontDoor = new DoorLocks(Guid.NewGuid(), "FrontDoor", "Entrance");
        var garageDoor = new DoorLocks(Guid.NewGuid(), "GarageDoor", "Garage");

        repository.SaveDevice(frontDoor);
        repository.SaveDevice(garageDoor);

        var frontDoorEntry = new CommandHistoryEntry(
            frontDoor.Id,
            new ToggleLockCommand(frontDoor, frontDoor));

        var garageDoorEntry = new CommandHistoryEntry(
            garageDoor.Id,
            new ToggleLockCommand(garageDoor, garageDoor));

        repository.SaveHistoryEntry(frontDoorEntry);
        repository.SaveHistoryEntry(garageDoorEntry);

        var history = service.GetCommandHistory(frontDoor.Id).ToList();

        Assert.Single(history);
        Assert.Equal(frontDoorEntry.Id, history[0].Id);
    }

    [Fact]
    public void GetCommandHistory_ThrowsKeyNotFound_WhenDeviceMissing()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);

        Assert.Throws<KeyNotFoundException>(() => service.GetCommandHistory(Guid.NewGuid()));
    }
}