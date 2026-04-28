using FluentValidation;
using SmartHome.Api.Locations;

namespace SmartHome.Api.Validators;

public class SetAmbientTemperatureRequestValidator
    : AbstractValidator<SetAmbientTemperatureRequest>
{
    public SetAmbientTemperatureRequestValidator()
    {
        // TODO - Amber: Validate that the temperature is within a valid range;
        RuleFor(x => x.Temperature)
            .InclusiveBetween(0, 120); // Example range, adjust as needed
    }
}
