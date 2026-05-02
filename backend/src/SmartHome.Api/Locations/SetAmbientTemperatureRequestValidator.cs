using FluentValidation;
using SmartHome.Domain.Devices.Thermostat;

namespace SmartHome.Api.Locations;

/// <summary>
/// Validator for SetAmbientTemperatureRequest. Ensures that the temperature is within a valid range.
/// </summary>
public class SetAmbientTemperatureRequestValidator : AbstractValidator<SetAmbientTemperatureRequest>
{
    public SetAmbientTemperatureRequestValidator()
    {
        RuleFor(x => x.Temperature)
            .InclusiveBetween(ThermostatDevice.MinTemperature, ThermostatDevice.MaxTemperature)
            .WithMessage($"Ambient temperature must be between {ThermostatDevice.MinTemperature}°F and {ThermostatDevice.MaxTemperature}°F.");
    }
}
