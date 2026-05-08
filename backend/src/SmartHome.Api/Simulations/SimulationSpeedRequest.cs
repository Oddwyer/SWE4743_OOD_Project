using SmartHome.Domain.Simulations;

namespace SmartHome.Api.Simulations;

public class SimulationSpeedRequest
{
    /// <summary>Desired speed multiplier for the simulation.</summary>
    public SimulationSpeed SpeedMultiplier { get; set; }
}
