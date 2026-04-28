using FluentValidation;
using SmartHome.Api.Devices;

namespace SmartHome.Api.Validators;

public class RegisterDeviceRequestValidator
    : BaseDeviceValidator<RegisterDeviceRequest>
{
    public RegisterDeviceRequestValidator()
    {
      
    }
}