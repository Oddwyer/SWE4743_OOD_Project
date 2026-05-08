using SmartHome.Api.Devices;
using SmartHome.Api.Locations;
using SmartHome.Api.Simulations;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Simulations;
using Xunit;

namespace SmartHome.Domain.Tests;

public class ValidationTests
{
    [Fact]
    public void RegisterDeviceRequestValidator_MissingRequiredFields_FailsValidation()
    {
        var validator = new RegisterDeviceRequestValidator();
        var request = new RegisterDeviceRequest();

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterDeviceRequest.DeviceName));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterDeviceRequest.DeviceLocation));
    }

    [Fact]
    public void ControlDeviceRequestValidator_InvalidCommandValue_FailsValidation()
    {
        var validator = new ControlDeviceRequestValidator();
        var request = new ControlDeviceRequest
        {
            Command = (DeviceCommandType)999
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ControlDeviceRequest.Command));
    }


    [Fact]
    public void SetAmbientTemperatureRequestValidator_OutOfRange_FailsValidation()
    {
        var validator = new AmbientTemperatureRequestValidator();
        var request = new AmbientTemperatureRequest
        {
            Temperature = 200
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AmbientTemperatureRequest.Temperature));
    }

    [Fact]
    public void SetSimulationSpeedRequestValidator_InvalidValue_FailsValidation()
    {
        var validator = new SimulationSpeedRequestValidator();
        var request = new SimulationSpeedRequest
        {
            SpeedMultiplier = (SimulationSpeed)999
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(SimulationSpeedRequest.SpeedMultiplier));
    }

    [Fact]
    public void ControlDeviceRequestValidator_MissingBrightness_FailsValidation()
    {
        var validator = new ControlDeviceRequestValidator();
        var request = new ControlDeviceRequest
        {
            Command = DeviceCommandType.SetBrightness,
            Brightness = null
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ControlDeviceRequest.Brightness));
    }
    [Fact]
    public void ControlDeviceRequestValidator_MissingTargetTemperature_FailsValidation()
    {
        var validator = new ControlDeviceRequestValidator();
        var request = new ControlDeviceRequest
        {
            Command = DeviceCommandType.SetTargetTemperature,
            TargetTemperature = null
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ControlDeviceRequest.TargetTemperature));
    }
}
