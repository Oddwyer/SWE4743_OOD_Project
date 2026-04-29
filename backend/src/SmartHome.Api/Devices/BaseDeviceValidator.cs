using FluentValidation;

namespace SmartHome.Api.Devices;

/// <summary>
/// Base validator for device-related requests. Validates common properties such as device name, location, and type.
/// </summary>

public class BaseDeviceValidator<T> : AbstractValidator<T> where T : BaseDevice
{
    public BaseDeviceValidator()
    {
        RuleFor(x => x.DeviceName)
            .NotEmpty()
            .MinimumLength(2)
            .WithMessage("Device name is required.");

        RuleFor(x => x.DeviceLocation)
            .NotEmpty()
            .WithMessage("Device location is required.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Device type must be valid.");
    }
}