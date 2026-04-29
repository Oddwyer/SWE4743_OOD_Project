using FluentValidation;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.Fan;

namespace SmartHome.Api.Devices;

/// <summary>
/// Validator for ControlDeviceRequest. Ensures that the command is valid for the device type.
/// </summary>
public class ControlDeviceRequestValidator : AbstractValidator<ControlDeviceRequest>
{
    public ControlDeviceRequestValidator()
    {
        RuleFor(x => x.Command)
            .NotNull()
            .IsInEnum();

        RuleFor(x => x.Brightness)
            .InclusiveBetween(10, 100)
            .When(x => x.Command == DeviceCommandType.SetBrightness);

        RuleFor(x => x.Color)
            .IsInEnum()
            .When(x => x.Command == DeviceCommandType.SetColor);

        RuleFor(x => x.FanSpeed)
            .IsInEnum()
            .When(x => x.Command == DeviceCommandType.SetFanSpeed);

        RuleFor(x => x.DesiredTemperature)
            .InclusiveBetween(60, 80)
            .When(x => x.Command == DeviceCommandType.SetDesiredTemperature);

        RuleFor(x => x.Mode)
            .IsInEnum()
            .When(x => x.Command == DeviceCommandType.SetThermostatMode);

    }
}