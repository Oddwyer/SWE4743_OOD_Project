using FluentValidation;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;

namespace SmartHome.Api.Devices;

/// <summary>
/// Validator for ControlDeviceRequest. Ensures required command parameters are present and in valid ranges.
/// </summary>
public class ControlDeviceRequestValidator : AbstractValidator<ControlDeviceRequest>
{
    public ControlDeviceRequestValidator()
    {
        RuleFor(x => x.Command)
            .NotNull()
            .IsInEnum()
            .WithMessage($"Command must be one of: {string.Join(", ", Enum.GetNames(typeof(DeviceCommandType)))}.");

        RuleFor(x => x.Brightness)
            .NotNull()
            .InclusiveBetween(LightDevice.MinBrightness, LightDevice.MaxBrightness)
            .When(x => x.Command == DeviceCommandType.SetBrightness)
            .WithMessage($"Brightness is required and must be between {LightDevice.MinBrightness}% and {LightDevice.MaxBrightness}%.");

        RuleFor(x => x.Color)
            .NotEmpty()
            .Matches("^#([0-9A-Fa-f]{6})$")
            .WithMessage("Color must be a valid HEX value like #FFFFFF.");

        RuleFor(x => x.FanSpeed)
            .NotNull()
            .IsInEnum()
            .When(x => x.Command == DeviceCommandType.SetFanSpeed)
            .WithMessage("Fan speed is required and must be one of: Low, Medium, High.");

        RuleFor(x => x.TargetTemperature)
            .NotNull()
            .InclusiveBetween(ThermostatDevice.MinTemperature, ThermostatDevice.MaxTemperature)
            .When(x => x.Command == DeviceCommandType.SetTargetTemperature)
            .WithMessage($"Desired temperature is required and must be between {ThermostatDevice.MinTemperature}°F and {ThermostatDevice.MaxTemperature}°F.");

        RuleFor(x => x.Mode)
            .NotNull()
            .IsInEnum()
            .When(x => x.Command == DeviceCommandType.SetThermostatMode)
            .WithMessage("Thermostat mode is required and must be one of: Auto, Heat, Cool.");
    }
}