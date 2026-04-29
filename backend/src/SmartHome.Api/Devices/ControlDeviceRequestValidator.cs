using FluentValidation;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.Fan;

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
            .WithMessage("Command must be a valid device command.");

        RuleFor(x => x.Brightness)
            .NotNull()
            .InclusiveBetween(10, 100)
            .When(x => x.Command == DeviceCommandType.SetBrightness)
            .WithMessage("Brightness is required and must be between 10 and 100.");

        RuleFor(x => x.Color)
            .NotNull()
            .IsInEnum()
            .When(x => x.Command == DeviceCommandType.SetColor)
            .WithMessage("Color is required and must be a valid light color.");

        RuleFor(x => x.FanSpeed)
            .NotNull()
            .IsInEnum()
            .When(x => x.Command == DeviceCommandType.SetFanSpeed)
            .WithMessage("Fan speed is required and must be valid.");

        RuleFor(x => x.DesiredTemperature)
            .NotNull()
            .InclusiveBetween(60, 80)
            .When(x => x.Command == DeviceCommandType.SetDesiredTemperature)
            .WithMessage("Desired temperature is required and must be between 60 and 80.");

        RuleFor(x => x.Mode)
            .NotNull()
            .IsInEnum()
            .When(x => x.Command == DeviceCommandType.SetThermostatMode)
            .WithMessage("Thermostat mode is required and must be valid.");
    }
}