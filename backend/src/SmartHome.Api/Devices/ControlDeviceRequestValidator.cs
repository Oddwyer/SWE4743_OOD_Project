using FluentValidation;

namespace SmartHome.Api.Devices;

/// <summary>
/// Validator for ControlDeviceRequest. Ensures that the command is valid for the device type.
/// </summary>
public class ControlDeviceRequestValidator : AbstractValidator<ControlDeviceRequest>
{
    public ControlDeviceRequestValidator()
    {
        // TODO - Amber: Validate that the command is valid for the device type; 
        RuleFor(x => x.Command)
           .NotEmpty();
    }
}