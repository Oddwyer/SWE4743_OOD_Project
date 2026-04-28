using FluentValidation;
using SmartHome.Api.Devices;

namespace SmartHome.Api.Validators;

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