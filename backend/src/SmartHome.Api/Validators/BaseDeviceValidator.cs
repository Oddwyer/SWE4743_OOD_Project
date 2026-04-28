using FluentValidation;
using SmartHome.Api.Devices;

namespace SmartHome.Api.Validators;

/// <summary>
/// Base validator for device-related requests. Validates common properties such as device name, location, and type.
/// </summary>

public class BaseDeviceValidator<T> : AbstractValidator<T> where T : BaseDevice
{
    public BaseDeviceValidator()
    {
        RuleFor(x => x.DeviceName)
            .NotEmpty()
            .MinimumLength(2);

        RuleFor(x => x.DeviceLocation)
            .NotEmpty();

        RuleFor(x => x.Type)
            .NotEmpty();
    }
}