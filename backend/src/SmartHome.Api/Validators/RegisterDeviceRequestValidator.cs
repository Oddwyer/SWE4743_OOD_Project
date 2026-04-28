using FluentValidation;
using SmartHome.Api.Devices;

namespace SmartHome.Api.Validators;

/// <summary>
/// Validator for RegisterDeviceRequest. Ensures that the device name, location, and type are valid.
/// </summary>
public class RegisterDeviceRequestValidator : BaseDeviceValidator<RegisterDeviceRequest>
{
    public RegisterDeviceRequestValidator()
    {

    }
}