using SmartHome.Domain.Commands.Latched;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Commands.Powered;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Contracts;
using Xunit;

namespace SmartHome.Domain.Tests;

public class CommandFactoryTests
{
    private readonly IDeviceCommandFactory _factory = new CommandFactory();

    [Fact]
    public void CreateCommand_ReturnsTogglePowerCommand()
    {
        var device = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");

        var command = _factory.CreateCommand(device, new CommandData
        {
            Command = DeviceCommandType.TogglePower
        });

        Assert.IsType<TogglePowerCommand>(command);
    }

    [Fact]
    public void CreateCommand_Throws_WhenBrightnessMissing()
    {
        var device = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");

        var exception = Assert.Throws<ArgumentException>(() => _factory.CreateCommand(device, new CommandData
        {
            Command = DeviceCommandType.SetBrightness
        }));

        Assert.Contains("Brightness is required", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateCommand_Throws_WhenColorMissing()
    {
        var device = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");

        var exception = Assert.Throws<ArgumentException>(() => _factory.CreateCommand(device, new CommandData
        {
            Command = DeviceCommandType.SetColor
        }));

        Assert.Contains("Color is required", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateCommand_Throws_WhenFanSpeedMissing()
    {
        var device = new FanDevice(Guid.NewGuid(), "CeilingFan", "LivingRoom");

        var exception = Assert.Throws<ArgumentException>(() => _factory.CreateCommand(device, new CommandData
        {
            Command = DeviceCommandType.SetFanSpeed
        }));

        Assert.Contains("Fan speed is required", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateCommand_Throws_WhenThermostatModeMissing()
    {
        var device = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Heat, new HeatModeStrategy());

        var exception = Assert.Throws<ArgumentException>(() => _factory.CreateCommand(device, new CommandData
        {
            Command = DeviceCommandType.SetThermostatMode
        }));

        Assert.Contains("thermostat mode is required", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateCommand_Throws_WhenTargetTemperatureMissing()
    {
        var device = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Heat, new HeatModeStrategy());

        var exception = Assert.Throws<ArgumentException>(() => _factory.CreateCommand(device, new CommandData
        {
            Command = DeviceCommandType.SetTargetTemperature
        }));

        Assert.Contains("target temperature must be provided", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateCommand_ReturnsToggleLockCommand()
    {
        var device = new DoorLocks(Guid.NewGuid(), "FrontDoor", "Entrance");

        var command = _factory.CreateCommand(device, new CommandData
        {
            Command = DeviceCommandType.ToggleLock
        });

        Assert.IsType<ToggleLockCommand>(command);
    }

    [Fact]
    public void CreateCommand_ReturnsSetThermostatModeCommand_WhenModeProvided()
    {
        var device = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Heat, new HeatModeStrategy());

        var command = _factory.CreateCommand(device, new CommandData
        {
            Command = DeviceCommandType.SetThermostatMode,
            Mode = ThermostatMode.Cool
        });

        Assert.IsType<SetThermostatModeCommand>(command);
    }

    [Fact]
    public void CreateCommand_Throws_ForUnsupportedCommandType()
    {
        var device = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");

        var unsupported = (DeviceCommandType)99;

        Assert.Throws<ArgumentException>(() => _factory.CreateCommand(device, new CommandData
        {
            Command = unsupported
        }));
    }
}
