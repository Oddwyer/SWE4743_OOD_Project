using FluentValidation;

namespace SmartHome.Api.Simulations;

/// <summary>
/// Validator for SetSimulationSpeedRequest. Ensures that the simulation speed is within a valid range.
/// </summary>
public class SetSimulationSpeedRequestValidator : AbstractValidator<SetSimulationSpeedRequest>
{
    public SetSimulationSpeedRequestValidator()
    {
        // TODO - Amber: Validate that the simulation speed is within a valid range;
        RuleFor(x => x.SpeedMultiplier)
            .IsInEnum()
            .WithMessage("Invalid simulation speed."); // Example range, adjust as needed
    }
}
