using SmartHome.Domain.Simulations;

namespace SmartHome.Api.Simulations;

public class SetSimulationSpeedRequest
{
    public SimulationSpeed SpeedMultiplier { get; set; }
}