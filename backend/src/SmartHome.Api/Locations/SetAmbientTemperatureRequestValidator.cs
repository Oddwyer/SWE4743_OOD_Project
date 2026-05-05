using FluentValidation;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Simulations;

namespace SmartHome.Api.Locations;

/// <summary>
/// Validator for SetAmbientTemperatureRequest. Ensures that the temperature is within a valid range.
/// </summary>
public class SetAmbientTemperatureRequestValidator : AbstractValidator<SetAmbientTemperatureRequest>
{
    public SetAmbientTemperatureRequestValidator()
    {
        RuleFor(x => x.Temperature)
            .InclusiveBetween(SimulationService.MinAmbientTemperature, SimulationService.MaxAmbientTemperature)
            .WithMessage($"Ambient temperature must be between {SimulationService.MinAmbientTemperature}°F and {SimulationService.MaxAmbientTemperature}°F.");
    }
}
