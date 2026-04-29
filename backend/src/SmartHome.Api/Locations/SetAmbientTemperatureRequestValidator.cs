using FluentValidation;

namespace SmartHome.Api.Locations;

/// <summary>
/// Validator for SetAmbientTemperatureRequest. Ensures that the temperature is within a valid range.
/// </summary>
public class SetAmbientTemperatureRequestValidator : AbstractValidator<SetAmbientTemperatureRequest>
{
    public SetAmbientTemperatureRequestValidator()
    {
        // TODO - Amber: Validate that the temperature is within a valid range;
        RuleFor(x => x.Temperature)
            .InclusiveBetween(0, 120)
            .WithMessage("Ambient temperature must be between 0 and 120."); // Example range, adjust as needed
    }
}
