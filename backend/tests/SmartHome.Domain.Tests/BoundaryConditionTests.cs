using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Commands;
using SmartHome.Api.Devices;
using Xunit;

namespace SmartHome.Domain.Tests;

public class BoundaryConditionTests
{
    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    public void LightDevice_SetLightBrightness_ValidBoundaryValues_DoesNotThrow(int brightness)
    {
        var light = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");
        light.TogglePower();

        light.SetLightBrightness(brightness);

        Assert.Equal(brightness, light.LightBrightness);
    }

    [Theory]
    [InlineData(9)]
    [InlineData(101)]
    [InlineData(0)]
    [InlineData(-1)]
    public void LightDevice_SetLightBrightness_OutOfRange_ThrowsArgumentOutOfRangeException(int brightness)
    {
        var light = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");
        light.TogglePower();

        Assert.Throws<ArgumentOutOfRangeException>(() => light.SetLightBrightness(brightness));
    }

    [Theory]
    [InlineData(60)]
    [InlineData(80)]
    public void ThermostatDevice_SetTargetTemperature_ValidBoundaryValues_DoesNotThrow(int targetTemperature)
    {
        var thermostat = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Auto, new AutoModeStrategy());
        thermostat.TogglePower();

        thermostat.SetTargetTemperature(targetTemperature);

        Assert.Equal(targetTemperature, thermostat.TargetTemperature);
    }

    [Theory]
    [InlineData(59)]
    [InlineData(81)]
    public void ThermostatDevice_SetTargetTemperature_OutOfRange_ThrowsArgumentOutOfRangeException(int targetTemperature)
    {
        var thermostat = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Auto, new AutoModeStrategy());
        thermostat.TogglePower();

        Assert.Throws<ArgumentOutOfRangeException>(() => thermostat.SetTargetTemperature(targetTemperature));
    }

    [Theory]
    [InlineData(FanSpeed.Low)]
    [InlineData(FanSpeed.Medium)]
    [InlineData(FanSpeed.High)]
    public void FanDevice_SetFanSpeed_ValidSpeeds_DoesNotThrow(FanSpeed speed)
    {
        var fan = new FanDevice(Guid.NewGuid(), "DeskFan", "Office");
        fan.TogglePower();

        fan.SetFanSpeed(speed);

        Assert.Equal(speed, fan.Speed);
    }

    [Fact]
    public void ControlDeviceRequestValidator_InvalidFanSpeedValue_RejectsValidation()
    {
        var validator = new ControlDeviceRequestValidator();
        var request = new ControlDeviceRequest
        {
            Command = DeviceCommandType.SetFanSpeed,
            FanSpeed = (FanSpeed)99
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ControlDeviceRequest.FanSpeed));
    }
}
