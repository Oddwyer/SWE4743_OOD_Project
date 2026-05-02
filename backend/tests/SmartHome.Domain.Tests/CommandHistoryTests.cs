using System;
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
using Xunit;

namespace SmartHome.Domain.Tests;

public class CommandHistoryTests
{
    [Fact]
    public void ApplyDeviceCommand_SavesHistoryEntry()
    {
        var repository = new FakeDeviceRepository();
        var commandFactory = new CommandFactory(new ThermostatStrategyFactory());
        var service = new DeviceService(repository, TestHelper.CreateDeviceFactory(), commandFactory);

        var light = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");
        repository.SaveDevice(light);

        service.ApplyDeviceCommand(light.Id, new CommandData { Command = DeviceCommandType.TogglePower });

        var history = repository.GetHistoryForDevice(light.Id).ToList();

        Assert.Single(history);
        Assert.Equal(light.Id, history[0].DeviceId);
        Assert.Contains("Toggled power", history[0].Operation);
        Assert.True(history[0].Timestamp <= DateTime.UtcNow);
    }

    [Fact]
    public void GetCommandHistory_ReturnsHistoryEntriesForDevice()
    {
        var repository = new FakeDeviceRepository();
        var commandFactory = new CommandFactory(new ThermostatStrategyFactory());
        var service = new DeviceService(repository, TestHelper.CreateDeviceFactory(), commandFactory);

        var fan = new FanDevice(Guid.NewGuid(), "DeskFan", "Office");
        repository.SaveDevice(fan);
        service.ApplyDeviceCommand(fan.Id, new CommandData { Command = DeviceCommandType.TogglePower });
        service.ApplyDeviceCommand(fan.Id, new CommandData { Command = DeviceCommandType.SetFanSpeed, FanSpeed = FanSpeed.High });

        var history = service.GetCommandHistory(fan.Id).ToList();

        Assert.Equal(2, history.Count);
        Assert.Contains(history, entry => entry.Operation.Contains("Toggled power"));
        Assert.Contains(history, entry => entry.Operation.Contains("Set fan speed"));
    }

    [Fact]
    public void ApplyDeviceCommand_ToMissingDevice_ThrowsKeyNotFoundException()
    {
        var repository = new FakeDeviceRepository();
        var commandFactory = new CommandFactory(new ThermostatStrategyFactory());
        var service = new DeviceService(repository, TestHelper.CreateDeviceFactory(), commandFactory);

        Assert.Throws<KeyNotFoundException>(() => service.ApplyDeviceCommand(Guid.NewGuid(), new CommandData { Command = DeviceCommandType.TogglePower }));
    }
}
