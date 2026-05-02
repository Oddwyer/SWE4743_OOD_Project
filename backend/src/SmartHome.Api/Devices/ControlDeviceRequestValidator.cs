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
                .WithMessage("Command is required.")
            .IsInEnum()
                .WithMessage($"Command must be one of: {string.Join(", ", Enum.GetNames(typeof(DeviceCommandType)))}.");

        RuleFor(x => x.Brightness)
            .NotNull()
                .WithMessage("Brightness is required.")
            .InclusiveBetween(LightDevice.MinBrightness, LightDevice.MaxBrightness)
                .WithMessage($"Brightness must be between {LightDevice.MinBrightness}% and {LightDevice.MaxBrightness}%.")
            .When(x => x.Command == DeviceCommandType.SetBrightness);

        RuleFor(x => x.Color)
            .NotEmpty()
                .WithMessage("Color is required.")
            .Matches("^#([0-9A-Fa-f]{6})$")
                .WithMessage("Color must be a valid HEX value like #FFFFFF.")
            .When(x => x.Command == DeviceCommandType.SetColor);

        RuleFor(x => x.FanSpeed)
            .NotNull()
                .WithMessage("Fan speed is required.")
            .IsInEnum()
                .WithMessage("Fan speed must be one of: Low, Medium, High.")
            .When(x => x.Command == DeviceCommandType.SetFanSpeed);

        RuleFor(x => x.TargetTemperature)
            .NotNull()
                .WithMessage("Desired temperature is required.")
            .InclusiveBetween(ThermostatDevice.MinTemperature, ThermostatDevice.MaxTemperature)
                .WithMessage($"Desired temperature must be between {ThermostatDevice.MinTemperature}°F and {ThermostatDevice.MaxTemperature}°F.")
            .When(x => x.Command == DeviceCommandType.SetTargetTemperature);

        RuleFor(x => x.Mode)
            .NotNull()
                .WithMessage("Thermostat mode is required.")
            .IsInEnum()
                .WithMessage("Thermostat mode must be one of: Auto, Heat, Cool.")
            .When(x => x.Command == DeviceCommandType.SetThermostatMode);
    }
}