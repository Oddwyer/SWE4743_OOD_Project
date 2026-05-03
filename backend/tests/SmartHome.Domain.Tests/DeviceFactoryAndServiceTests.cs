using System;
using System.Collections.Generic;
using System.Linq;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Commands.History;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.Thermostat.ThermostatStates;
using SmartHome.Domain.Locations;
using SmartHome.Domain.Simulations;
using Xunit;

namespace SmartHome.Domain.Tests;

public class DeviceFactoryAndServiceTests
{
    private static DeviceFactory CreateDeviceFactory()
    {
        return new DeviceFactory(new IDeviceTypeFactory[]
        {
            new LightDeviceFactory(),
            new FanDeviceFactory(),
            new ThermostatDeviceFactory(new ThermostatStrategyFactory()),
            new DoorLockFactory()
        });
    }

    private class FakeCommandFactory : ICommandFactory
    {
        public IDeviceCommand CreateCommand(IDevice device, CommandData context)
        {
            return context.Command switch
            {
                DeviceCommandType.ToggleLock when device is DoorLocks doorLock => new ToggleLockCommand(doorLock),
                _ => throw new ArgumentException("Unsupported command type for fake command factory.")
            };
        }
    }

    private class FakeSimulationService : ISimulationService
    {
        public void SetAmbientTemperature(string location, int temperature) { }
        public int GetAmbientTemperature(string location) => 70;
        public void SetSimulationSpeed(SimulationSpeed speedMultiplier) { }
        public void ResetSimulation() { }
    }

    private static DeviceService CreateDeviceService(FakeDeviceRepository repository)
    {
        return new DeviceService(new FakeSimulationService(), repository, TestHelper.CreateDeviceFactory(), new FakeCommandFactory(), repository);
    }

    [Fact]
    public void DeviceFactory_CreateDevice_ReturnsLightDevice()
    {
        var factory = CreateDeviceFactory();

        var device = factory.CreateDevice("DeskLamp", "Office", DeviceType.Light);

        Assert.IsType<LightDevice>(device);
        Assert.Equal(DeviceType.Light, device.Type);
    }

    [Fact]
    public void DeviceFactory_CreateDevice_ReturnsFanDevice()
    {
        var factory = CreateDeviceFactory();

        var device = factory.CreateDevice("CeilingFan", "LivingRoom", DeviceType.Fan);

        Assert.IsType<FanDevice>(device);
        Assert.Equal(DeviceType.Fan, device.Type);
    }

    [Fact]
    public void DeviceFactory_CreateDevice_ReturnsThermostatDevice()
    {
        var factory = CreateDeviceFactory();

        var device = factory.CreateDevice("Nest", "Bedroom", DeviceType.Thermostat);

        Assert.IsType<ThermostatDevice>(device);
        Assert.Equal(DeviceType.Thermostat, device.Type);
    }

    [Fact]
    public void DeviceFactory_CreateDevice_ReturnsDoorLockDevice()
    {
        var factory = CreateDeviceFactory();

        var device = factory.CreateDevice("FrontDoor", "Entrance", DeviceType.DoorLock);

        Assert.IsType<DoorLocks>(device);
        Assert.Equal(DeviceType.DoorLock, device.Type);
    }

    [Fact]
    public void DeviceFactory_CreateDevice_ThrowsForUnsupportedDeviceType()
    {
        var factory = CreateDeviceFactory();

        Assert.Throws<ArgumentException>(() => factory.CreateDevice("Unknown", "Storage", (DeviceType)999));
    }

    [Fact]
    public void DeviceService_RegisterDevice_CreatesDefaultDeviceState()
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

        Assert.Equal(DeviceType.DoorLock, device.Type);
        Assert.IsType<DoorLocks>(device);
        Assert.Equal(DeviceLatchState.Locked, ((DoorLocks)device).LatchState);
    }

    [Fact]
    public void DeviceService_RemoveDevice_RemovesDeviceFromRepository()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);
        var device = new DoorLocks(Guid.NewGuid(), "PatioDoor", "Patio");
        repository.SaveDevice(device);

        service.RemoveDevice(device.Id);

        Assert.Null(repository.FindDeviceById(device.Id));
    }

    [Fact]
    public void DeviceService_RegisterDevice_ThrowsWhenSecondThermostatIsInSameLocation()
    {
        var repository = new FakeDeviceRepository();
        var service = CreateDeviceService(repository);
        repository.SaveDevice(new ThermostatDevice(Guid.NewGuid(), "MainThermostat", "LivingRoom", ThermostatMode.Auto, new AutoModeStrategy()));

        var register = new RegisterDeviceData
        {
            DeviceName = "SecondThermostat",
            DeviceLocation = "LivingRoom",
            DeviceType = DeviceType.Thermostat
        };

        Assert.Throws<InvalidOperationException>(() => service.RegisterDevice(register));
    }
}
