using SmartHome.Domain.Commands;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Thermostat;
using Xunit;

namespace SmartHome.Domain.Tests;

public class StateMachineTests
{
    [Fact]
    public void LightDevice_TogglePower_OffToOn_Valid()
    {
        var light = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");

        Assert.False(light.IsDeviceOn);

        light.TogglePower();

        Assert.True(light.IsDeviceOn);
    }

    [Fact]
    public void LightDevice_TogglePower_OnToOff_Valid()
    {
        var light = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");

        light.TogglePower();
        Assert.True(light.IsDeviceOn);

        light.TogglePower();

        Assert.False(light.IsDeviceOn);
    }

    [Fact]
    public void LightDevice_SetLightBrightness_WhenOff_ThrowsInvalidOperationException()
    {
        var light = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");

        Assert.Throws<InvalidOperationException>(() => light.SetLightBrightness(50));
    }

    [Fact]
    public void LightDevice_ChangeColor_WhenOff_ThrowsInvalidOperationException()
    {
        var light = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");

        Assert.Throws<InvalidOperationException>(() => light.SetColor("Blue"));
    }

    [Fact]
    public void LightDevice_OffOnCycle_RetainsColorAndBrightness()
    {
        var light = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");

        light.TogglePower();
        light.SetColor("Red");
        light.SetLightBrightness(25);

        Assert.Equal("Red", light.Color);
        Assert.Equal(25, light.LightBrightness);

        light.TogglePower();
        Assert.False(light.IsDeviceOn);

        light.TogglePower();
        Assert.True(light.IsDeviceOn);
        Assert.Equal("Red", light.Color);
        Assert.Equal(25, light.LightBrightness);
    }

    [Fact]
    public void FanDevice_TogglePower_OffToOn_Valid()
    {
        var fan = new FanDevice(Guid.NewGuid(), "CeilingFan", "LivingRoom");

        Assert.False(fan.IsDeviceOn);

        fan.TogglePower();

        Assert.True(fan.IsDeviceOn);
    }

    [Fact]
    public void FanDevice_TogglePower_OnToOff_Valid()
    {
        var fan = new FanDevice(Guid.NewGuid(), "CeilingFan", "LivingRoom");

        fan.TogglePower();
        Assert.True(fan.IsDeviceOn);

        fan.TogglePower();

        Assert.False(fan.IsDeviceOn);
    }

    [Theory]
    [InlineData(FanSpeed.Low)]
    [InlineData(FanSpeed.Medium)]
    [InlineData(FanSpeed.High)]
    public void FanDevice_SetFanSpeed_WhenOn_Valid(FanSpeed speed)
    {
        var fan = new FanDevice(Guid.NewGuid(), "DeskFan", "Office");
        fan.TogglePower();

        fan.SetFanSpeed(speed);

        Assert.Equal(speed, fan.Speed);
    }

    [Fact]
    public void FanDevice_SetFanSpeed_WhenOff_ThrowsInvalidOperationException()
    {
        var fan = new FanDevice(Guid.NewGuid(), "DeskFan", "Office");

        var exception = Assert.Throws<InvalidOperationException>(() => fan.SetFanSpeed(FanSpeed.High));

        Assert.Contains("Cannot set fan speed when fan is off", exception.Message);
    }

    [Fact]
    public void FanDevice_OffOnCycle_RetainsSpeed()
    {
        var fan = new FanDevice(Guid.NewGuid(), "DeskFan", "Office");
        fan.TogglePower();
        fan.SetFanSpeed(FanSpeed.High);

        fan.TogglePower();
        Assert.False(fan.IsDeviceOn);

        fan.TogglePower();
        Assert.True(fan.IsDeviceOn);
        Assert.Equal(FanSpeed.High, fan.Speed);
    }

    [Fact]
    public void ThermostatDevice_OffToIdleToHeatingToIdle_Valid()
    {
        var thermostat = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Auto, new AutoModeStrategy());

        thermostat.TogglePower();
        Assert.False(thermostat.IsDeviceOn);

        thermostat.SetTargetTemperature(75);
        thermostat.Evaluate(70);
        Assert.True(thermostat.IsDeviceOn);

        thermostat.Evaluate(75);
        Assert.False(thermostat.IsDeviceOn);
    }

    [Fact]
    public void ThermostatDevice_OffToIdleToCoolingToIdle_Valid()
    {
        var thermostat = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Auto, new AutoModeStrategy());

        thermostat.TogglePower();
        Assert.False(thermostat.IsDeviceOn);

        thermostat.SetTargetTemperature(75);
        thermostat.Evaluate(80);
        Assert.True(thermostat.IsDeviceOn);

        thermostat.Evaluate(75);
        Assert.False(thermostat.IsDeviceOn);
    }

    [Fact]
    public void ThermostatDevice_EvaluateWhileOff_ThrowsInvalidOperationException()
    {
        var thermostat = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Auto, new AutoModeStrategy());

        Assert.Throws<InvalidOperationException>(() => thermostat.Evaluate(70));
    }

    [Fact]
    public void ThermostatCommandFactory_ChangeModeWhileActive_ChangesMode()
    {
        var thermostat = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Heat, new HeatModeStrategy());
        thermostat.TogglePower();
        thermostat.SetTargetTemperature(75);
        thermostat.Evaluate(70);
        Assert.True(thermostat.IsDeviceOn);

        var commandFactory = new CommandFactory();
        var command = commandFactory.CreateCommand(thermostat, new CommandData
        {
            Command = DeviceCommandType.SetThermostatMode,
            ThermostatMode = ThermostatMode.Cool
        });

        command.Execute();

        Assert.Equal(ThermostatMode.Cool, thermostat.Mode);
    }

    [Fact]
    public void DoorLocks_ToggleLock_LockedToUnlocked_Valid()
    {
        var door = new DoorLocks(Guid.NewGuid(), "FrontDoor", "Entrance");

        Assert.Equal(DeviceLatchState.Locked, door.LatchState);

        door.ToggleLock();

        Assert.Equal(DeviceLatchState.Unlocked, door.LatchState);
    }

    [Fact]
    public void DoorLocks_ToggleLock_UnlockedToLocked_Valid()
    {
        var door = new DoorLocks(Guid.NewGuid(), "FrontDoor", "Entrance");

        door.ToggleLock();
        Assert.Equal(DeviceLatchState.Unlocked, door.LatchState);

        door.ToggleLock();

        Assert.Equal(DeviceLatchState.Locked, door.LatchState);
    }
}
