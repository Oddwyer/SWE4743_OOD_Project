using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Thermostat;
using Xunit;

namespace SmartHome.Domain.Tests;

public class DeviceTransitionTests
{
    [Fact]
    public void LightPowerToggle_TransitionsOnAndAllowsColorAndBrightness()
    {
        var light = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");

        Assert.False(light.IsDeviceOn);

        light.TogglePower();

        Assert.True(light.IsDeviceOn);

        light.SetColor("Blue");
        light.SetLightBrightness(50);

        Assert.Equal("Blue", light.Color);
        Assert.Equal(50, light.LightBrightness);
    }

    [Fact]
    public void LightSetBrightness_InvalidRange_ThrowsArgumentOutOfRangeException()
    {
        var light = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");
        light.TogglePower();

        var originalValue = light.LightBrightness;
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => light.SetLightBrightness(5));

        Assert.Equal(originalValue, light.LightBrightness);
        Assert.Contains("Brightness must be between", exception.Message);
    }

    [Fact]
    public void FanPowerToggle_TransitionsOnAndOffAndAcceptsSpeedWhenOn()
    {
        var fan = new FanDevice(Guid.NewGuid(), "CeilingFan", "LivingRoom");

        Assert.False(fan.IsDeviceOn);
        fan.TogglePower();
        Assert.True(fan.IsDeviceOn);

        fan.SetFanSpeed(FanSpeed.High);
        Assert.Equal(FanSpeed.High, fan.Speed);

        fan.TogglePower();
        Assert.False(fan.IsDeviceOn);
    }

    [Fact]
    public void FanSetSpeedWhileOff_ThrowsInvalidOperationException()
    {
        var fan = new FanDevice(Guid.NewGuid(), "DeskFan", "Office");
        var originalSpeed = fan.Speed;

        var exception = Assert.Throws<InvalidOperationException>(() => fan.SetFanSpeed(FanSpeed.High));

        Assert.Equal(originalSpeed, fan.Speed);
        Assert.Contains("Cannot set fan speed when fan is off", exception.Message);
    }

    [Fact]
    public void ThermostatOffPowerToggle_TransitionsToIdleAndThenHeatingOnEvaluate()
    {
        var thermostat = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Auto, new AutoModeStrategy());

        Assert.False(thermostat.IsDeviceOn);

        thermostat.TogglePower();

        Assert.False(thermostat.IsDeviceOn); // idle is not actively heating or cooling

        thermostat.SetTargetTemperature(75);
        thermostat.Evaluate(70);

        Assert.True(thermostat.IsDeviceOn);
    }

    [Fact]
    public void AutoModeStrategy_TransitionsBetweenHeatingIdleAndCooling()
    {
        var thermostat = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Auto, new AutoModeStrategy());
        thermostat.TogglePower();
        thermostat.SetTargetTemperature(75);

        thermostat.Evaluate(70);
        Assert.True(thermostat.IsDeviceOn); // heating

        thermostat.Evaluate(80);
        Assert.True(thermostat.IsDeviceOn); // cooling
    }

    [Fact]
    public void HeatModeStrategy_ReturnsHeatingBelowTargetAndIdleAbove()
    {
        var thermostat = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Heat, new HeatModeStrategy());
        thermostat.TogglePower();
        thermostat.SetTargetTemperature(74);

        thermostat.Evaluate(70);
        Assert.True(thermostat.IsDeviceOn);

        thermostat.Evaluate(80);
        Assert.False(thermostat.IsDeviceOn);
    }

    [Fact]
    public void CoolModeStrategy_ReturnsCoolingAboveTargetAndIdleBelow()
    {
        var thermostat = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Cool, new CoolModeStrategy());
        thermostat.TogglePower();
        thermostat.SetTargetTemperature(72);

        thermostat.Evaluate(80);
        Assert.True(thermostat.IsDeviceOn);

        thermostat.Evaluate(70);
        Assert.False(thermostat.IsDeviceOn);
    }
}
