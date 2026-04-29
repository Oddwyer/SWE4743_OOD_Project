using FluentValidation;

namespace SmartHome.Api.Simulations;

/// <summary>
/// Ensures that the simulation speed is a valid enum value.
/// </summary>
public class SetSimulationSpeedRequestValidator : AbstractValidator<SetSimulationSpeedRequest>
{
    public SetSimulationSpeedRequestValidator()
    {
        RuleFor(x => x.SpeedMultiplier)
            .IsInEnum()
            .WithMessage("Simulation speed must be one of: 1x, 2x, 5x, 10x.");
    }
}
