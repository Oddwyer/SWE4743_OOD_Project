using SmartHome.Domain.Devices;
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
                .WithMessage("Device name is required.")
            .MinimumLength(2)
                .WithMessage("Device name must be at least 2 characters long.");

        RuleFor(x => x.DeviceLocation)
            .NotEmpty()
                .WithMessage("Device location is required.");

        RuleFor(x => x.Type)
            .IsInEnum()
                .WithMessage($"Device type must be one of: {string.Join(", ", Enum.GetNames(typeof(DeviceType)))}.");
    }
}
