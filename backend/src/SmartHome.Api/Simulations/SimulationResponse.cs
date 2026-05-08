using SmartHome.Domain.Simulations;

namespace SmartHome.Api.Simulations;

/// <summary>
/// DTO used to represent a simulation response returned by the API.
/// </summary>
public class SimulationResponse
{
    /// <summary>Human-readable result message.</summary>
    public string Message { get; set; } = string.Empty;
    /// <summary>Speed multiplier active after the operation.</summary>
    public SimulationSpeed SpeedMultiplier { get; set; } = SimulationSpeed.OneX;
}