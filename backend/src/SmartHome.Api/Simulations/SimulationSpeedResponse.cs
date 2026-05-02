namespace SmartHome.Api.Simulations;

/// <summary>
/// DTO used to represent a simulation response returned by the API.
/// </summary>

public class SimulationSpeedResponse
{
    public string Message { get; set; } = string.Empty;
    public int SpeedMultiplier { get; set; }

}