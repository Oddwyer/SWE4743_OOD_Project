using System;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Commands.Fan;
using SmartHome.Domain.Commands.Light;
using SmartHome.Domain.Commands.Lock;
using SmartHome.Domain.Commands.Power;
using SmartHome.Domain.Commands.Thermostat;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Thermostat;
using Xunit;

namespace SmartHome.Domain.Tests;

public class CommandFactoryTests
{
    private readonly ICommandFactory _factory = new CommandFactory(new ThermostatStrategyFactory());

    [Fact]
    public void CreateCommand_ReturnsTogglePowerCommand()
    {
        var device = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");

        var command = _factory.CreateCommand(device, new CommandContext
        {
            Command = DeviceCommandType.TogglePower
        });

        Assert.IsType<TogglePowerCommand>(command);
    }

    [Fact]
    public void CreateCommand_Throws_WhenBrightnessMissing()
    {
        var device = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");

        var exception = Assert.Throws<ArgumentException>(() => _factory.CreateCommand(device, new CommandContext
        {
            Command = DeviceCommandType.SetBrightness
        }));

        Assert.Contains("Brightness is required", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateCommand_Throws_WhenColorMissing()
    {
        var device = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");

        var exception = Assert.Throws<ArgumentException>(() => _factory.CreateCommand(device, new CommandContext
        {
            Command = DeviceCommandType.SetColor
        }));

        Assert.Contains("Color is required", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateCommand_Throws_WhenFanSpeedMissing()
    {
        var device = new FanDevice(Guid.NewGuid(), "CeilingFan", "LivingRoom");

        var exception = Assert.Throws<ArgumentException>(() => _factory.CreateCommand(device, new CommandContext
        {
            Command = DeviceCommandType.SetFanSpeed
        }));

        Assert.Contains("Fan speed is required", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateCommand_Throws_WhenThermostatModeMissing()
    {
        var device = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Heat, new HeatModeStrategy());

        var exception = Assert.Throws<ArgumentException>(() => _factory.CreateCommand(device, new CommandContext
        {
            Command = DeviceCommandType.SetThermostatMode
        }));

        Assert.Contains("thermostat mode is required", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateCommand_Throws_WhenTargetTemperatureMissing()
    {
        var device = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Heat, new HeatModeStrategy());

        var exception = Assert.Throws<ArgumentException>(() => _factory.CreateCommand(device, new CommandContext
        {
            Command = DeviceCommandType.SetDesiredTemperature
        }));

        Assert.Contains("target temperature must be provided", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateCommand_ReturnsToggleLockCommand()
    {
        var device = new DoorLocks(Guid.NewGuid(), "FrontDoor", "Entrance");

        var command = _factory.CreateCommand(device, new CommandContext
        {
            Command = DeviceCommandType.ToggleLock
        });

        Assert.IsType<ToggleLockCommand>(command);
    }

    [Fact]
    public void CreateCommand_ReturnsSetThermostatModeCommand_WhenModeProvided()
    {
        var device = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Heat, new HeatModeStrategy());

        var command = _factory.CreateCommand(device, new CommandContext
        {
            Command = DeviceCommandType.SetThermostatMode,
            Mode = ThermostatMode.Cool
        });

        Assert.IsType<SetThermostateModeCommand>(command);
    }

    [Fact]
    public void CreateCommand_Throws_ForUnsupportedCommandType()
    {
        var device = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");

        var unsupported = (DeviceCommandType)99;

        Assert.Throws<ArgumentException>(() => _factory.CreateCommand(device, new CommandContext
        {
            Command = unsupported
        }));
    }
}
