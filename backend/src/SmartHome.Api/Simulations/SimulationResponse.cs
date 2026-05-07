using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Simulations;

namespace SmartHome.Api.Simulations;

/// <summary>
/// DTO used to represent a simulation response returned by the API.
/// </summary>

public class SimulationResponse
{
    public string Message { get; set; } = string.Empty;
    public SimulationSpeed SpeedMultiplier { get; set; } = SimulationSpeed.OneX;

}