using SmartHome.Domain.Simulations;

namespace SmartHome.Api.Simulations;

/// <summary>
/// DTO for setting the simulation speed multiplier.
/// </summary>
public class SimulationSpeedRequest
{
    /// <summary>Desired speed multiplier for the simulation.</summary>
    public SimulationSpeed SpeedMultiplier { get; set; }
}
