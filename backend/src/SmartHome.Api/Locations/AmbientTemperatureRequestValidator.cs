using FluentValidation;
using SmartHome.Domain.Simulations;

namespace SmartHome.Api.Locations;

/// <summary>
/// Validator for Set ambient temperature request. Ensures that the temperature is within a valid range.
/// </summary>
public class AmbientTemperatureRequestValidator : AbstractValidator<AmbientTemperatureRequest>
{
    public AmbientTemperatureRequestValidator()
    {
        RuleFor(x => x.Temperature)
            .InclusiveBetween(SimulationService.MinAmbientTemperature, SimulationService.MaxAmbientTemperature)
            .WithMessage($"Ambient temperature must be between {SimulationService.MinAmbientTemperature}°F and {SimulationService.MaxAmbientTemperature}°F.");
    }
}
